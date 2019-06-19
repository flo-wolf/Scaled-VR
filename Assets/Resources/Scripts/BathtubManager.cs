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
            public int z;
            public int layer;
        }

        [Header("References")]
        [SerializeField] private GameObject _bathtubPB;         // prefab reference

        [Header("Spawning")]
        [SerializeField] private float _spawnDelay = 0.1f;      // time between spawning two bathtubs
        [SerializeField] private int _amountPerLayerX = 10;     // how many bathtubs in a row
        [SerializeField] private int _amountPerLayerZ = 10;     // how many bathtubs in a column
        [SerializeField] private float _stepX = 2f;             // x distance between bathtubs
        [SerializeField] private float _stepZ = 1f;             // z distance between bathtubs
        [SerializeField] private float _spawnHeight = -2f;      // y height
        [SerializeField] private float _offsetX = -9f;      // y height

        [Header("Testing")]
        public int test_spawnAmount = 35;

        private Dictionary<GridPosition, GameObject> _spawnedBathtubs = new Dictionary<GridPosition, GameObject>();
        private int _AmountPerLayer { get { return _amountPerLayerX * _amountPerLayerZ; } }
        private Coroutine currentUpdateCoroutine = null;

        // grid iteration variables
        private int _x = 0;
        private int _z = 0;

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
            if (currentUpdateCoroutine != null)
                StopCoroutine(currentUpdateCoroutine);

            StartCoroutine(C_UpdateBathtubs(endAmount, _spawnDelay));
        }

        private void UpdateBathtubsOld(int endAmount)
        {
            Debug.Log("Update Bathtubs to: " + endAmount);
            // testing, remove to allow multiple layers
            if (endAmount < _AmountPerLayer)
            {

                int layer = 0;
                int z = 0;
                int x = 0;

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
                                z = z,
                                layer = layer
                            };

                            if(!_spawnedBathtubs.ContainsKey(gridPos))
                            {
                                Vector3 spawnPos = new Vector3(_stepX * x + _offsetX, _spawnHeight, transform.position.z + _stepZ * z);
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
                                layer = layer
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


        IEnumerator C_UpdateBathtubs(int endAmount, float delay)
        {

            int layer = 0;
            int z = 0;
            int x = 0;

            // iterate through the grid incrementally, row per row, and add tubs
            for (z = 0; z < _amountPerLayerZ; z++)
            {

                for (x = 0; x < _amountPerLayerX; x++)
                {
                    GridPosition gridPos = new GridPosition
                    {
                        x = x,
                        z = z,
                        layer = layer
                    };

                    if (!_spawnedBathtubs.ContainsKey(gridPos))
                    {
                        Vector3 spawnPos = new Vector3(_stepX * x + _offsetX, _spawnHeight, transform.position.z + _stepZ * z);
                        GameObject bathtubGO = Instantiate(_bathtubPB, spawnPos, Quaternion.identity);
                        bathtubGO.transform.parent = this.transform;
                        _spawnedBathtubs.Add(gridPos, bathtubGO);
                    }

                    // we spanwed enough bathtubs, stop this sorcery!!
                    if (_spawnedBathtubs.Count >= endAmount)
                        goto End;
                }
            }


            yield return null;
        }
    }
}
