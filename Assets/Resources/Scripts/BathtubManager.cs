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
        [SerializeField] private GameObject _bathtubPB;         // prefab reference

        [Header("Spawning")]
        [SerializeField] private float _spawnDelay = 0.1f;      // time between spawning two bathtubs
        [SerializeField] private int _amountPerLayerX = 10;     // how many bathtubs in a row
        [SerializeField] private int _amountPerLayerZ = 10;     // how many bathtubs in a column
        [SerializeField] private float _stepX = 2f;             // x distance between bathtubs
        [SerializeField] private float _stepY = 1f;             // y distance between layers
        [SerializeField] private float _stepZ = 1f;             // z distance between bathtubs
        [SerializeField] private float _offsetY = -2f;          // lowerst layer height (offset)
        [SerializeField] private float _offsetX = -9f;          // left-right offset (negative to center rows)
        [SerializeField] private float _offsetZ = 3f;           // z offset to offset rows away from the player

        [Header("Testing")]
        public int test_spawnAmount = 35;

        private Dictionary<GridPosition, GameObject> _spawnedBathtubs = new Dictionary<GridPosition, GameObject>();
        private int _AmountPerLayer { get { return _amountPerLayerX * _amountPerLayerZ; } }
        private Coroutine _currentUpdateCoroutine = null;

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

            _currentUpdateCoroutine = StartCoroutine(C_UpdateBathtubs(endAmount, _spawnDelay));
        }

        /*
        private void UpdateBathtubsOld(int endAmount)
        {
            Debug.Log("Update Bathtubs to: " + endAmount);
            // testing, remove to allow multiple layers
            if (endAmount < _AmountPerLayer)
            {
                int z = 0;
                int x = 0;
                int y = 0;

                // not enough bathtubs => spawn more
                if (endAmount > _spawnedBathtubs.Count)
                {
                    // iterate through the grid incrementally, row per row, and add tubs
                    for (z = 0; z < _amountPerLayerZ; z++)
                    {

                        for (x = 0; x < _amountPerLayerX; x++)
                        {
                            GridPosition gridPos = new GridPosition
                            {
                                x = x,
                                y = y,
                                z = z,
                            };

                            if(!_spawnedBathtubs.ContainsKey(gridPos))
                            {
                                Vector3 spawnPos = new Vector3(_stepX * x + _offsetX, _offsetY, transform.position.z + _stepZ * z);
                                GameObject bathtubGO = Instantiate(_bathtubPB, spawnPos, Quaternion.identity);
                                bathtubGO.transform.parent = this.transform;
                                _spawnedBathtubs.Add(gridPos, bathtubGO);
                            }

                            // we spanwed enough bathtubs, stop this sorcery!!
                            if (_spawnedBathtubs.Count >= endAmount)
                                goto End;
                        }
                    }
                }

                // too many bathtubs (help) => despawn
                else
                {
                    Debug.Log("Despawn -- _z: " + _z + " _x: " + _x);

                    // iterate through the grid decrementally, row per row, and remove tubs
                    for (z = _amountPerLayerZ; z >= 0; z--)
                    {
                        for (x = _amountPerLayerX; x >= 0; x--)
                        {

                            GridPosition gridPos = new GridPosition
                            {
                                x = x,
                                z = z,
                                y = y
                            };

                            if (_spawnedBathtubs.ContainsKey(gridPos))
                            {
                                GameObject bathTub = _spawnedBathtubs[gridPos];
                                Debug.Log("Despawn -- z: " + z + " x: " + x + " bathTub: " + bathTub);

                                // remove the reference from our dictionary
                                if (bathTub != null)
                                    _spawnedBathtubs.Remove(gridPos);

                                // kill it
                                Destroy(bathTub.gameObject);
                            }

                            // we despanwed enough bathtubs, stop this sorcery!!
                            if (_spawnedBathtubs.Count <= endAmount)
                                goto End;
                        }
                    }
                }

            End:
                // save last grid position to continue spawning/despawning from here
                _z = z;
                _x = x;
            }
        }
        */

        IEnumerator C_UpdateBathtubs(int endAmount, float delay)
        {
            if (endAmount == _spawnedBathtubs.Count)
                yield break;

            int x = 1;
            int y = 0;
            int z = 0;

            // iterate through the grid incrementally, row per row, and add tubs
            for (z = 0; z < _amountPerLayerZ; z++)
            {
                for (x = 1; x <= _amountPerLayerX/2; x++)
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


                    // left side first
                    if (TrySpawnBathtubAt(gridPosLeft))
                    {
                        // we spanwed enough bathtubs, stop this sorcery!!
                        if (_spawnedBathtubs.Count >= endAmount)
                            yield break;

                        TrySpawnBathtubAt(gridPosRight);


                        yield return new WaitForSeconds(delay);
                    }

                    if (_spawnedBathtubs.Count >= endAmount)
                        yield break;

                    yield return null;
                }
                yield return null;
            }
            yield return null;
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

                return true;
            }
            return false;
        }


        private bool TryDespawnBathtubAt(GridPosition gridPos)
        {
            if (!_spawnedBathtubs.ContainsKey(gridPos))
            {
                GameObject bathTub = _spawnedBathtubs[gridPos];

                
                if (bathTub != null)
                {
                    // remove the reference from our dictionary
                    _spawnedBathtubs.Remove(gridPos);

                    // kill it
                    Destroy(bathTub.gameObject);

                    return true;
                }
            }
            return false;
        }
    }
}
