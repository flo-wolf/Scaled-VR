using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// outline around the area needed to produce the food on the scale.

namespace Change
{
    public class AreaOutline : MonoBehaviour
    {

        [Header("Scaling")]
        [SerializeField] private float _scaleDuration = 2f;
        [SerializeField] private Vector3[] _startPoints;


        private LineRenderer _line;
        private Coroutine _scaleCoroutine = null;
        private Vector3[] _currentPoints;
        private float _sideLength = 0f;


        void Start()
        {
            _line = GetComponent<LineRenderer>();
            //_line.enabled = true;

            _currentPoints = new Vector3[4];
            _startPoints.CopyTo(_currentPoints, 0);

            SetPoints(_startPoints);

            Scale.onScaleEvent.AddListener(OnScaleEvent);
        }

        // something was put onto or removed from the scale. Emissions have changed. This event is fired by the scale. All objects can subscribe (listen) to it.
        private void OnScaleEvent(Food.Emission emission)
        {
            // calc the sidelength of our strip of grass that we try to outline.
            _sideLength = emission.areaSqrMeters / DissolveArea.FixedWidth;

            // disable the line when there is nothing on the scale causing area consumption.
            


            // calc new points based on area sidelength calculated by the square meter emission data
            Vector3[] newPoints = new Vector3[4];
            _currentPoints.CopyTo(newPoints, 0);

            // 3rd point (far left)
            newPoints[2].x = _startPoints[2].x - _sideLength;

            // 4rd point (far right)
            newPoints[3].x = _startPoints[3].x - _sideLength;


            // start scaling the line renderer
            if (_scaleCoroutine != null)
            {
                StopCoroutine(_scaleCoroutine);
                _scaleCoroutine = null;
            }

            _scaleCoroutine = StartCoroutine(C_ScaleGrid(newPoints, _scaleDuration));
        }

        IEnumerator C_ScaleGrid(Vector3[] endPoints, float duration)
        {
            if (!_line.enabled)
                _line.enabled = true;

            float t = 0f;
            float lerpT = 0f;

            Vector3[] startPoints = new Vector3[4];
            _currentPoints.CopyTo(startPoints, 0);

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

            if (_sideLength == 0)
            {
                _line.enabled = false;
            }
            else if (!_line.enabled)
                _line.enabled = true;

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
    }
}
