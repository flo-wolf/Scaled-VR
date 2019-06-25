using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
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
                    newValue = emission.waterConsumption;
                    Debug.Log("Water" + emission.waterConsumption);
                    break;


                case Infotype.ground:
                    newValue = emission.areaConsumption;
                    break;

                case Infotype.gas:
                    newValue = emission.gasConsumption;
                    break;

                case Infotype.calories:
                    newValue = emission.waterConsumption;
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
                screentext.text = value + "";

                yield return null;
            }
            yield return null;
        }
    }

}
