using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Change
{
    public class Scale : MonoBehaviour
    {
        public class ScaleEvent : UnityEvent<Food.Emission> { }
        public static ScaleEvent onScaleEvent = new ScaleEvent();

        [Header("Scale")]
        [SerializeField] private bool _registerFoodInHand = false;
        public Food.Emission m_Emission;
        [SerializeField] private List<Food> _connectedFood = new List<Food>();


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

        private void UpdateEmissions()
        {
            float startWater = m_Emission.waterLiters;
            float startGas = m_Emission.gasGrams;
            float startArea = m_Emission.areaSqrMeters;
            float startKcal = m_Emission.kcal;
            float startWeight = m_Emission.weight;

            float water = 0f;
            float gas = 0f;
            float area = 0f;
            float kcal = 0f;
            float weight = 0f;

            foreach(Food f in _connectedFood)
            {
                water += f.emission.waterLiters;
                gas += f.emission.gasGrams;
                area += f.emission.areaSqrMeters;
                kcal += f.emission.kcal;
                weight += f.emission.weight;

            }

            // emissions have changed
            if(water != startWater || gas != startGas || area != startArea || kcal != startKcal || weight != startWeight)
            {
                m_Emission.waterLiters = water;
                m_Emission.gasGrams = gas;
                m_Emission.areaSqrMeters = area;
                m_Emission.kcal = kcal;
                m_Emission.weight = weight;

                // fire the event, passing the updated emissions
                onScaleEvent.Invoke(m_Emission);
            }
        }

        private void Update()
        {
            UpdateEmissions();
        }
    }
}

