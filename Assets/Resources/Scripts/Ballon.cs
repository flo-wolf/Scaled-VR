using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Change
{
    public class Ballon : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Rigidbody _rb;

        [Header("Scaling")]
        [SerializeField] private float _scaleDuration = 3f;
        [SerializeField] private float _minSize = 0.1f;

        [Header("Updraft")]
        [SerializeField] private float _minUpdraft = 8f;
        [SerializeField] private float _maxUpdraft = 12f;
        [SerializeField] private float _minUpdraftWeight = 0f;
        [SerializeField] private float _maxUpdraftWeight = 40f;

       
        private Coroutine _scaleCoroutine = null;
        private float _currentWeight = 0f;


        void Start()
        {

            SetScale(_minSize);
            Scale.onScaleEvent.AddListener(OnScaleEvent);
        }

        private void OnScaleEvent(Food.Emission emission)
        {
            if (_scaleCoroutine != null)
            {
                StopCoroutine(_scaleCoroutine);
                _scaleCoroutine = null;
            }

            _scaleCoroutine = StartCoroutine(C_Scale(emission.gasGrams, _scaleDuration));
        }

        IEnumerator C_Scale(float endWeight, float duration)
        {
            float t = 0f;
            float startWeight = _currentWeight;

            while (t < duration)
            {
                t += Time.deltaTime;

                _currentWeight = Mathf.Lerp(startWeight, endWeight, (t / duration));

                SetScale(_currentWeight);

                yield return null;
            }

            yield return null;
        }


        private void SetScale(float gasWeight)
        {
            float sqrMeterVolume = gasWeight * 0.001836f; // 1.836 kilogram [kg] of Carbon dioxide fits into 1 cubic meter

            float d = Mathf.Pow(6 * (0.51f * Mathf.Abs(sqrMeterVolume)) / 3.14f, 0.333f);     // 6 * (Volumen von Co2 in Liter * Gewicht) / Pi  und davon dann über Mathf.Pow die Potenz von 1/3  ????
            
            // other attempts:
            //float d = Mathf.Pow(Mathf.PI, 2 / 3) * Mathf.Pow((6 * (sqrMeterVolume)), 1 / 3);    // diameter = π^2/3 (6v)^1/3 , where v is the volume in m^2
            //float d = sqrMeterVolume / Mathf.PI;

            d = Mathf.Clamp(d, _minSize, Mathf.Infinity);
            transform.localScale = new Vector3(d, d, d);

            
        }

        private void Update()
        {
            // uplift
            if (_currentWeight > 0)
                _rb.AddForce(Vector3.up * CalcUpdraft(_currentWeight), ForceMode.Acceleration);

            else
            {
                _rb.velocity = Vector3.zero;
                transform.localPosition = new Vector3(0f, 0.3f, 0f);
            }
        }


        private float CalcUpdraft(float gasWeight)
        {
            return Mathf.Clamp(gasWeight.Remap(_minUpdraftWeight, _maxUpdraftWeight, _minUpdraft, _maxUpdraft), _minUpdraft, _maxUpdraft);
        }
    }
}

