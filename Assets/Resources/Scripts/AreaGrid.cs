using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Change
{
    public class AreaGrid : MonoBehaviour
    {

        [Header("Scaling")]
        [SerializeField] private float _scaleDuration = 2f;
        [SerializeField] private Vector3[] _startPoints;

        private LineRenderer _line;
        private Coroutine _scaleCoroutine = null;
        private Vector3[] _currentPoints;


        void Start()
        {
            _line = GetComponent<LineRenderer>();
            _line.enabled = true;

            _currentPoints = _startPoints;

            SetPoints(_startPoints);

            Scale.onScaleEvent.AddListener(OnScaleEvent);
        }

        // something was put onto or removed from the scale. Emissions have changed. This event is fired by the scale. All objects can subscribe (listen) to it.
        private void OnScaleEvent(Food.Emission emission)
        {
            // disable the line when there is nothing on the scale causing area consumption.
            if (emission.areaSqrMeters == 0)
            {
                _line.enabled = false;
                return;
            }
            _line.enabled = true;

            // calc new points based on emission
            float sideLength = emission.areaSqrMeters / DissolveArea.FixedWidth;

            Vector3[] newPoints = _currentPoints;
            // 3rd point (far left)
            newPoints[2] = _startPoints[1];
            newPoints[2].x = newPoints[2].x - sideLength;

            // 4rd point (far right)
            newPoints[3] = _startPoints[0];
            newPoints[3].x = newPoints[3].x - sideLength;

            if (_scaleCoroutine != null)
            {
                StopCoroutine(_scaleCoroutine);
                _scaleCoroutine = null;
            }

            _scaleCoroutine = StartCoroutine(C_ScaleGrid(newPoints, _scaleDuration));
        }

        IEnumerator C_ScaleGrid(Vector3[] endPoints, float duration)
        {
            float t = 0f;
            float lerpT = 0f;
            Vector3[] startPoints = _currentPoints;

            while (t < duration)
            {
                t += Time.deltaTime;

                lerpT = Mathf.SmoothStep(0, 1, t / duration);

                // lerp 3rd point (far left)
                _currentPoints[2] = Vector3.Lerp(startPoints[2], endPoints[2], lerpT);

                // lerp 4rd point (far right)
                _currentPoints[3] = Vector3.Lerp(startPoints[3], endPoints[3], lerpT);


                SetPoints(_currentPoints);

                yield return null;
            }
            yield return null;
        }

        //private void Update()
        //{
        //    SetBounds(_bounds);
        //}

        private void SetPoints(Vector3[] points)
        {
            _line.SetPosition(0, points[0]);
            _line.SetPosition(1, points[1]);
            _line.SetPosition(2, points[2]);
            _line.SetPosition(3, points[3]);
        }


        // Update is called once per frame
        void Update()
        {

        }
    }

}
