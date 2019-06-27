using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Change
{
    // somewhere, "Bathtub Manager" is someones job title...
    public class ProgressiveGridSpawner : MonoBehaviour
    {
        
        struct GridPosition                                             // position of an object inside the grid (custom coordinate system)
        {
            public int x; // column
            public int y; // layer
            public int z; // row
        }

        [Header("References")]
        [SerializeField] private GameObject _spawnPrefab;                             // prefab reference

        [Header("Timings")]
        [SerializeField] private float _spawnDelay = 0.1f;                          // if !_useFixedSpawnDuration use this as a delay between spawns
        [SerializeField] private float _despawnDelay = 0.1f;                        // if !_useFixedSpawnDuration use this as a delay between spawns
        [SerializeField] private float _fixedSpawnDuration = 1f;                    // if _useFixedSpawnDuration use this as a total duration in which all spawns should happen
        [SerializeField] private float _fixedDespawnDuration = 1f;                  // if _useFixedSpawnDuration use this as a total duration in which all spawns should happen
        [SerializeField] private bool _useSpawnDurationInsteadOfDelay = false;      // time between spawning two bathtubs
        [SerializeField] private bool _useDespawnDurationInsteadOfDelay = false;    // time between spawning two bathtubs
        [SerializeField] private bool _instantDespawn = false;                      // dont decrementally despawn the objects, despawn them instantly.

        [Header("Grid Settings")]
        [SerializeField] private int _amountPerLayerX = 10;                         // how many bathtubs in a row
        [SerializeField] private int _amountPerLayerZ = 10;                         // how many bathtubs in a column
        [SerializeField] private int _maxLayers = 20;                               // how many bathtubs in a column
        [SerializeField] private float _stepX = 2f;                                 // x distance between bathtubs
        [SerializeField] private float _stepY = 1f;                                 // y distance between layers
        [SerializeField] private float _stepZ = 1f;                                 // z distance between bathtubs
        [SerializeField] private float _offsetY = -2f;                              // lowerst layer height (offset)
        [SerializeField] private float _offsetX = -9f;                              // left-right offset (negative to center rows)
        [SerializeField] private float _offsetZ = 3f;                               // z offset to offset rows away from the player

        
        private int _AmountPerLayer { get { return _amountPerLayerX * _amountPerLayerZ; } }
        private Coroutine _currentUpdateCoroutine = null;
        private Dictionary<GridPosition, GameObject> _spawnedObjects = new Dictionary<GridPosition, GameObject>();

        // we need to keep track of the last added positions in order to create shortcuts for the algorithm. The dictionary doesn't do that.
        private List<GridPosition> _positionHistory = new List<GridPosition>();

        // in case our deltaTime is bigger than our spawn/despawn delay, we need to keep track of the deltaTime. Otherwise we can only spawn a minimum of 1 bathtub per frame, which is too slow.
        private float _delayDelta = 0f;


        private void Start()
        {
            Scale.onScaleEvent.AddListener(OnScaleEvent);
        }

        private void OnScaleEvent(Food.Emission emission)
        {
            Debug.Log("Scale Event Registered - emissions: water: " + emission.waterLiters + " gas: " + emission.gasGrams);
            int bathtubAmount = Mathf.CeilToInt((emission.waterLiters / 150f));
            UpdateGrid(bathtubAmount);
        }

        private void UpdateGrid(int endAmount)
        {
            if (_currentUpdateCoroutine != null)
            {
                StopCoroutine(_currentUpdateCoroutine);
                _currentUpdateCoroutine = null;
            }

            _currentUpdateCoroutine = StartCoroutine(C_UpdateGrid(endAmount));
        }

        IEnumerator C_UpdateGrid(int endAmount)
        {
            if (endAmount == _spawnedObjects.Count)
                yield break;

            int x = 1;
            int y = 0;
            int z = 0;
            float fixedDelay = 0f;
            float delayDelta = 0f;
            bool lastFrameDelayed = false;

            // not enough objects => spawn more
            if (endAmount > _spawnedObjects.Count)
            {
                // calc delay
                if (!_useSpawnDurationInsteadOfDelay)
                    fixedDelay = _spawnDelay;
                else
                    fixedDelay = _fixedSpawnDuration / Mathf.Abs(_spawnedObjects.Count - endAmount);

               delayDelta = fixedDelay;
               

                // start at the buttom layer, move through that grid, then move a layer up and repeat.
                for (y = 0; y < _maxLayers; y++)
                {
                    // iterate through the grid incrementally, row per row, and add tubs from the center to the sides.
                    for (z = 0; z < _amountPerLayerZ; z++)
                    {
                        for (x = 1; x <= _amountPerLayerX / 2; x++)
                        {
                            GridPosition gridPosLeft = new GridPosition
                            {
                                x = -x,
                                y = y,
                                z = z,
                            };

                            GridPosition gridPosRight = new GridPosition
                            {
                                x = x,
                                y = y,
                                z = z,
                            };

                            // add left side first
                            if (TrySpawnObjectAt(gridPosLeft))
                            {
                                // we spanwed enough bathtubs, stop this sorcery!!
                                if (_spawnedObjects.Count >= endAmount)
                                    yield break;

                                // add right side
                                TrySpawnObjectAt(gridPosRight);

                                // we spanwed enough bathtubs, stop this sorcery!!
                                if (_spawnedObjects.Count >= endAmount)
                                    yield break;


                                /* FLO
                                // no delay racing, spawn
                                if (delayDelta == 0)
                                {
                                    yield return new WaitForSeconds(fixedDelay);
                                }
                                else
                                {
                                    if (Time.deltaTime - fixedDelay > fixedDelay)
                                    {
                                        delayDelta = Time.deltaTime - fixedDelay;
                                    }
                                    else
                                    {
                                        yield return new WaitForSeconds(fixedDelay - delayDelta);
                                    }
                                }*/

                                
                                // allow for more than one bathtub to spawn per frame when the delay is really low
                                if (Time.deltaTime > delayDelta)
                                {
                                    delayDelta += Time.deltaTime;
                                    lastFrameDelayed = true;
                                    //if (delayDelta <= 0)
                                    //    delayDelta = fixedDelay;
                                }
                                else
                                {
                                    if (lastFrameDelayed)
                                        delayDelta -= Time.deltaTime;
                                    yield return new WaitForSeconds(delayDelta); // THIS CAUSES PROBLEMS when the delay is smaller than the deltatime. 1 bathtub per frame, min.
                                }
                                
                            }
                        }
                    }
                }
                yield return null;
            }

            // too many objects => despawn
            else
            {
                // calc delay
                if (!_useSpawnDurationInsteadOfDelay)
                    fixedDelay = _despawnDelay;
                else
                    fixedDelay = _fixedDespawnDuration / Mathf.Abs(_spawnedObjects.Count - endAmount);

                delayDelta = fixedDelay;

                // find out which layer to start from (get lastly added bathtub's layer, which gives the top layer), so we can avoid iterating from the very very top.
                int startLayer = 0;
                if(_positionHistory.Count >= 1)
                    startLayer = _positionHistory[_positionHistory.Count - 1].y;

                // start at the buttom layer, move through that grid, then move a layer up and repeat.
                for (y = startLayer; y >= 0; y--)
                {
                    // iterate through the grid incrementally, row per row
                    for (z = _amountPerLayerZ - 1; z >= 0; z--)
                    {
                        // remove the tubs from the sides to the center
                        for (x = _amountPerLayerX / 2; x >= 1 / 2; x--)
                        {
                            GridPosition gridPosLeft = new GridPosition
                            {
                                x = -x,
                                y = y,
                                z = z,
                            };

                            GridPosition gridPosRight = new GridPosition
                            {
                                x = x,
                                y = y,
                                z = z,
                            };

                            // remove left side first
                            if (TryDespawnObjectAt(gridPosLeft))
                            {
                                // we removed enough bathtubs, stop this sorcery!!
                                if (_spawnedObjects.Count <= endAmount)
                                    yield break;

                                // remove right side
                                TryDespawnObjectAt(gridPosRight);

                                // we removed enough bathtubs, stop this sorcery!!
                                if (_spawnedObjects.Count <= endAmount)
                                    yield break;

                                if(!_instantDespawn)
                                    yield return new WaitForSeconds(fixedDelay);
                            }
                        }
                    }
                }
                yield return null;
            }
        }


        private bool TrySpawnObjectAt(GridPosition gridPos)
        {
            if (!_spawnedObjects.ContainsKey(gridPos))
            {
                Vector3 spawnPos = new Vector3(
                    _stepX * gridPos.x + _offsetX, 
                    _stepY * gridPos.y + _offsetY, 
                    _stepZ * gridPos.z + _offsetZ);

                // compensate for wide center 
                if (gridPos.x > 0)
                    spawnPos.x -= _stepX / 2;
                else
                    spawnPos.x += _stepX / 2;

                GameObject bathtubGO = Instantiate(_spawnPrefab, spawnPos, Quaternion.identity);
                bathtubGO.transform.parent = this.transform;
                _spawnedObjects.Add(gridPos, bathtubGO);

                // memorize the last added position
                _positionHistory.Add(gridPos);

                return true;
            }
            return false;
        }


        private bool TryDespawnObjectAt(GridPosition gridPos)
        {
            if (_spawnedObjects.ContainsKey(gridPos))
            {
                GameObject go = _spawnedObjects[gridPos];

                
                if (go != null)
                {
                    // remove the reference from our dictionary and history
                    _spawnedObjects.Remove(gridPos);
                    _positionHistory.Remove(gridPos);

                    // kill it
                    Bathtub tub = go.GetComponent<Bathtub>();
                    if(tub != null)
                    {
                        tub.FadeOut();
                    }
                    //Destroy(go.gameObject);

                    return true;
                }
            }
            return false;
        }
    }
}
