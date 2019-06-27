using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Change
{
    public class KcalDoll : MonoBehaviour
    {
        [Header("Target")]
        public List<Material> materials = new List<Material>();

        [Header("Scaling")]
        [SerializeField] private float _scaleDuration = 2f;
        [SerializeField] private float _maxKcal = 2500f;

        private Coroutine _scaleCoroutine = null;
        private float _currentHeight = 0f;


        void Start()
        {
            Scale.onScaleEvent.AddListener(OnScaleEvent);
        }

        private void OnScaleEvent(Food.Emission emission)
        {
            float goalHeight = Mathf.Clamp01(emission.kcal.Remap(0, _maxKcal, 0, 1));

            if (_scaleCoroutine != null)
            {
                StopCoroutine(_scaleCoroutine);
                _scaleCoroutine = null;
            }

            _scaleCoroutine = StartCoroutine(C_ScaleHeight(goalHeight, _scaleDuration));
        }

        IEnumerator C_ScaleHeight(float endHeight, float duration)
        {
            float t = 0f;
            float startHeight = _currentHeight;

            while (t < duration)
            {
                t += Time.deltaTime;

                _currentHeight = Mathf.SmoothStep(startHeight, endHeight, t);

                SetHeight(_currentHeight);

                yield return null;
            }
            yield return null;
        }

        private void SetHeight(float height)
        {
            foreach (Material material in materials)
            {
                material.SetFloat("_Height", height);
            }
        }
    }
}

