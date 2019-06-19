using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Change
{
    // somewhere, "Bathtub Manager" is someones job title...
    public class BathtubManager : MonoBehaviour
    {
        struct GridPosition
        {
            public int x;
            public int y; // layer
            public int z;
        }

        [Header("References")]
        [SerializeField] private GameObject _bathtubPB;                 // prefab reference

        [Header("Spawn Timing")]
        [SerializeField] private bool _useFixedSpawnDuration = false;   // time between spawning two bathtubs
        [SerializeField] private float _spawnDelay = 0.1f;              // if !_useFixedSpawnDuration use this as a delay between spawns
        [SerializeField] private float _fixedSpawnDuration = 1f;      // if _useFixedSpawnDuration use this as a total duration in which all spawns should happen

        [Header("Spawn Location Settings")]
        [SerializeField] private int _amountPerLayerX = 10;             // how many bathtubs in a row
        [SerializeField] private int _amountPerLayerZ = 10;             // how many bathtubs in a column
        [SerializeField] private int _maxLayers = 20;                   // how many bathtubs in a column
        [SerializeField] private float _stepX = 2f;                     // x distance between bathtubs
        [SerializeField] private float _stepY = 1f;                     // y distance between layers
        [SerializeField] private float _stepZ = 1f;                     // z distance between bathtubs
        [SerializeField] private float _offsetY = -2f;                  // lowerst layer height (offset)
        [SerializeField] private float _offsetX = -9f;                  // left-right offset (negative to center rows)
        [SerializeField] private float _offsetZ = 3f;                   // z offset to offset rows away from the player

        [Header("Testing")]
        public int test_spawnAmount = 35;

        private Dictionary<GridPosition, GameObject> _spawnedBathtubs = new Dictionary<GridPosition, GameObject>();
        private int _AmountPerLayer { get { return _amountPerLayerX * _amountPerLayerZ; } }
        private Coroutine _currentUpdateCoroutine = null;

        // we need to keep track of the last added positions in order to create shortcuts for the algorithm. The dictionary doesn't do that.
        private List<GridPosition> _positionHistory = new List<GridPosition>();

        private void Start()
        {
            Scale.onScaleEvent.AddListener(OnScaleEvent);

            // testing
            UpdateBathtubs(test_spawnAmount);
        }

        private void OnScaleEvent(Food.Emission emission)
        {
            Debug.Log("Scale Event Registered - emissions: water: " + emission.waterConsumption + " gas: " + emission.gasConsumption);
            int bathtubAmount = (int) emission.waterConsumption;
            UpdateBathtubs(bathtubAmount);
        }

        private void UpdateBathtubs(int endAmount)
        {
            if (_currentUpdateCoroutine != null)
            {
                StopCoroutine(_currentUpdateCoroutine);
                _currentUpdateCoroutine = null;
            }

            _currentUpdateCoroutine = StartCoroutine(C_UpdateBathtubs(endAmount));
        }

        IEnumerator C_UpdateBathtubs(int endAmount)
        {
            if (endAmount == _spawnedBathtubs.Count)
                yield break;

            int x = 1;
            int y = 0;
            int z = 0;
            float delay = 0f;

            if (!_useFixedSpawnDuration)
                delay = _spawnDelay;
            else
                delay = _fixedSpawnDuration / Mathf.Abs(_spawnedBathtubs.Count - endAmount);
            

            // not enough bathtubs => spawn more
            if (endAmount > _spawnedBathtubs.Count)
            {
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
                            if (TrySpawnBathtubAt(gridPosLeft))
                            {

                                // we spanwed enough bathtubs, stop this sorcery!!
                                if (_spawnedBathtubs.Count >= endAmount)
                                    yield break;

                                // add right side
                                TrySpawnBathtubAt(gridPosRight);

                                // we spanwed enough bathtubs, stop this sorcery!!
                                if (_spawnedBathtubs.Count >= endAmount)
                                    yield break;

                                yield return new WaitForSeconds(delay); // THIS CAUSES PROBLEMS when the delay is smaller than the deltatime. 1 bathtub per frame, min.
                            }
                        }
                    }
                }
                yield return null;
            }

            // too many bathtubs => despawn
            else
            {
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
                            if (TryDespawnBathtubAt(gridPosLeft))
                            {
                                // we removed enough bathtubs, stop this sorcery!!
                                if (_spawnedBathtubs.Count <= endAmount)
                                    yield break;

                                // remove right side
                                TryDespawnBathtubAt(gridPosRight);

                                // we removed enough bathtubs, stop this sorcery!!
                                if (_spawnedBathtubs.Count <= endAmount)
                                    yield break;

                                yield return new WaitForSeconds(delay);
                            }
                        }
                    }
                }
                yield return null;
            }
        }


        private bool TrySpawnBathtubAt(GridPosition gridPos)
        {
            if (!_spawnedBathtubs.ContainsKey(gridPos))
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

                GameObject bathtubGO = Instantiate(_bathtubPB, spawnPos, Quaternion.identity);
                bathtubGO.transform.parent = this.transform;
                _spawnedBathtubs.Add(gridPos, bathtubGO);

                // memorize the last added position
                _positionHistory.Add(gridPos);

                return true;
            }
            return false;
        }


        private bool TryDespawnBathtubAt(GridPosition gridPos)
        {
            if (_spawnedBathtubs.ContainsKey(gridPos))
            {
                GameObject bathTub = _spawnedBathtubs[gridPos];

                
                if (bathTub != null)
                {
                    // remove the reference from our dictionary and history
                    _spawnedBathtubs.Remove(gridPos);
                    _positionHistory.Remove(gridPos);

                    // kill it
                    Destroy(bathTub.gameObject);

                    return true;
                }
            }
            return false;
        }
    }
}
