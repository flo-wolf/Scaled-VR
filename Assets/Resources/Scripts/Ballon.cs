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

        [Header("Updraft")]
        [SerializeField] private float _minUpdraft = 8f;
        [SerializeField] private float _maxUpdraft = 12f;
        [SerializeField] private float _minUpdraftWeight = 0f;
        [SerializeField] private float _maxUpdraftWeight = 40f;

       
        private Coroutine _scaleCoroutine = null;
        private float _currentWeight = 0f;


        void Start()
        {

            SetScale(0);
            Scale.onScaleEvent.AddListener(OnScaleEvent);
        }

        private void OnScaleEvent(Food.Emission emission)
        {
            if (_scaleCoroutine != null)
            {
                StopCoroutine(_scaleCoroutine);
                _scaleCoroutine = null;
            }

            _scaleCoroutine = StartCoroutine(C_Scale(emission.gasConsumption, _scaleDuration));
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
            // size
            float d = Mathf.Pow(6 * (0.51f * Mathf.Abs(gasWeight)) / 3.14f, 0.333f);     // 6 * (Volumen von Co2 in Liter * Gewicht) / Pi  und davon dann über Mathf.Pow die Potenz von 1/3 
            transform.localScale = new Vector3(d, d, d);
        }

        private void Update()
        {
            // uplift
            _rb.AddForce(Vector3.up * CalcUpdraft(_currentWeight));
        }

        private float CalcUpdraft(float gasWeight)
        {
            return gasWeight.Remap(_minUpdraftWeight, _maxUpdraftWeight, _minUpdraft, _maxUpdraft);
        }
    }
}

