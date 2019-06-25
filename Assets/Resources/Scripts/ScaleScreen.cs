using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

namespace Change
{


    public class ScaleScreen : MonoBehaviour
    {

        [System.Serializable]
        private enum Infotype { water, gas, ground, calories };
        [SerializeField] private Infotype infotype;
        [SerializeField] private float _countDuration = 1f;
        [SerializeField] private TMPro.TextMeshPro screentext;
        private float currentValue;
        private float newValue;

        private Coroutine _countCoroutine = null;

        private void Start()
        {
            Scale.onScaleEvent.AddListener(OnScaleEvent);
            screentext = transform.GetComponentInChildren<TMPro.TextMeshPro>();
          

        }
        private void OnScaleEvent(Food.Emission emission)
        {
            currentValue = newValue;
            switch (infotype)
            {
                case Infotype.water:
                    newValue = emission.waterLiters;
                    Debug.Log("Water" + emission.waterLiters);
                    break;


                case Infotype.ground:
                    newValue = emission.areaSqrMeters;
                    break;

                case Infotype.gas:
                    newValue = emission.gasGrams;
                    break;

                case Infotype.calories:
                    newValue = emission.waterLiters;
                    break;

                default:
                    Debug.Log("Choose info type for the " + name);
                    break;
            }

            if (_countCoroutine != null)
            {
                StopCoroutine(_countCoroutine);
                _countCoroutine = null;
            }

            _countCoroutine = StartCoroutine(C_CountingValue( _countDuration));
        }


        IEnumerator C_CountingValue(float duration)
        {
           
            float t = 0f;
            float lerpT = 0f;
            float value;
            while (t < duration)
            {
                t += Time.deltaTime;
                lerpT = Mathf.SmoothStep(0, 1, t / duration);

                value = Mathf.Lerp(currentValue, newValue, lerpT);
                screentext.text = String.Format("{0:#,0}", (int)value) + " liters";

                yield return null;
            }
            yield return null;
        }
    }

}
