using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Change
{
    public class Scale : MonoBehaviour
    {
        [Header("Scale")]
        [SerializeField] private bool _registerFoodInHand = false;

        [Header("Measurements")]
        public float WaterConsumption = 0f; // { get; private set; }
        public float GasConsumption = 0f;

        private List<Food> _connectedFood = new List<Food>();


        private void Start()
        {
        }

        private void OnTriggerStay(Collider other)
        {
            Food f = other.GetComponent<Food>();
            if (f != null)
            {
                if (!_connectedFood.Contains(f))
                {
                    // if wanted, only add the food if its not connected to a hand
                    if (!_registerFoodInHand && f.m_ActiveHand == null)
                        _connectedFood.Add(f);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            Food f = other.GetComponent<Food>();
            if (f != null)
            {
                if (_connectedFood.Contains(f))
                {
                    _connectedFood.Remove(f);
                }
            }
        }

        private void UpdateConsumption()
        {
            float water = 0f;
            float gas = 0f;

            foreach(Food f in _connectedFood)
            {
                water += f.waterConsumption;
                gas += f.gasConsumption;
            }

            WaterConsumption = water;
            GasConsumption = gas;
        }

        private void Update()
        {
            UpdateConsumption();
        }
    }
}

