using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Change
{
    public class DissolveBounds : MonoBehaviour
    {
        public static float FixedWidth { get; } = 8f;

        public Material material;

        [Header("Scaling")]
        [SerializeField] private float _scaleDuration = 1f;
        [SerializeField] private Vector4 _offsetBounds = Vector4.zero;
        [SerializeField] private Vector4 _fixedBounds = Vector4.zero;

        [Header("Debugging")]
        [SerializeField] private Vector4 _bounds = Vector4.zero;

        private Coroutine _scaleCoroutine = null;

        // Start is called before the first frame update
        void Start()
        {
            Scale.onScaleEvent.AddListener(OnScaleEvent);

            SetBounds(_bounds);
        }

        private void OnScaleEvent(Food.Emission emission)
        {
            float sideLength = (emission.areaConsumption / FixedWidth) / 2;
            Vector4 bounds = new Vector4(sideLength, sideLength, sideLength, sideLength);

            if(_scaleCoroutine != null)
            {
                StopCoroutine(_scaleCoroutine);
                _scaleCoroutine = null;
            }

            _scaleCoroutine = StartCoroutine(C_ScaleBounds(bounds, _scaleDuration));
        }



        IEnumerator C_ScaleBounds(Vector4 endBounds, float duration)
        {
            float t = 0f;
            float lerpT = 0f;
            Vector4 startBounds = _bounds;

            while(t < duration)
            {
                t += Time.deltaTime;

                lerpT = Mathf.SmoothStep(0, 1, t / duration);

                _bounds = Vector4.Lerp(startBounds, endBounds, lerpT);

                SetBounds(_bounds * 4);

                yield return null;
            }
            yield return null;
        }

        private void Update()
        {
            SetBounds(_bounds);
        }

        private void SetBounds(Vector4 bounds)
        {
            Plane planeRight = new Plane(transform.right, transform.position);
            Plane planeFront = new Plane(transform.forward, transform.position);

            Vector4 planeRightVector;
            Vector4 planeLeftVector;
            Vector4 planeFrontVector;
            Vector4 planeBackVector;

            // x
            //if (_fixedBounds.x != 0)
                planeRightVector = new Vector4(planeRight.normal.x, planeRight.normal.y, planeRight.normal.z, planeRight.distance + bounds.x + _offsetBounds.x);
           // else
           //    planeRightVector = new Vector4(planeRight.normal.x, planeRight.normal.y, planeRight.normal.z, planeRight.distance + _fixedBounds.x + _offsetBounds.x);

            // y
           // if (_fixedBounds.y != 0)
                planeLeftVector = new Vector4(-planeRight.normal.x, planeRight.normal.y, planeRight.normal.z, planeRight.distance + bounds.y + _offsetBounds.y);
           // else
           //     planeLeftVector = new Vector4(-planeRight.normal.x, planeRight.normal.y, planeRight.normal.z, planeRight.distance + _fixedBounds.y + _offsetBounds.y);

            // z
           // if (_fixedBounds.z != 0)
                planeFrontVector = new Vector4(planeFront.normal.x, planeFront.normal.y, planeFront.normal.z, planeFront.distance + bounds.z + _offsetBounds.z);
           // else
           //     planeFrontVector = new Vector4(planeFront.normal.x, planeFront.normal.y, planeFront.normal.z, planeFront.distance + _fixedBounds.z + _offsetBounds.z);

            // w
           // if (_fixedBounds.w != 0)
                planeBackVector = new Vector4(planeFront.normal.x, planeFront.normal.y, -planeFront.normal.z, planeFront.distance + bounds.w + _offsetBounds.w);
           // else
           //     planeBackVector = new Vector4(planeFront.normal.x, planeFront.normal.y, -planeFront.normal.z, planeFront.distance + _fixedBounds.w + _offsetBounds.w);


            material.SetVector("_PlaneRight", planeRightVector);
            material.SetVector("_PlaneLeft", planeLeftVector);
            material.SetVector("_PlaneFront", planeFrontVector);
            material.SetVector("_PlaneBack", planeBackVector);
        }
    }
}

/*
 * 
 * // x
            if (_fixedBounds.x != 0)
                planeRightVector = new Vector4(planeRight.normal.x, planeRight.normal.y, planeRight.normal.z, planeRight.distance + bounds.x + _offsetBounds.x);
            else
               planeRightVector = new Vector4(planeRight.normal.x, planeRight.normal.y, planeRight.normal.z, planeRight.distance + _fixedBounds.x + _offsetBounds.x);

            // y
            if (_fixedBounds.y != 0)
                planeRightVector = new Vector4(planeRight.normal.x, planeRight.normal.y, planeRight.normal.z, planeRight.distance + bounds.y + _offsetBounds.y);
            else
                planeRightVector = new Vector4(planeRight.normal.x, planeRight.normal.y, planeRight.normal.z, planeRight.distance + _fixedBounds.y + _offsetBounds.y);

            // z
            if (_fixedBounds.z != 0)
                planeRightVector = new Vector4(planeRight.normal.x, planeRight.normal.y, planeRight.normal.z, planeRight.distance + bounds.z + _offsetBounds.z);
            else
                planeRightVector = new Vector4(planeRight.normal.x, planeRight.normal.y, planeRight.normal.z, planeRight.distance + _fixedBounds.z + _offsetBounds.z);

            // w
            if (_fixedBounds.w != 0)
                planeRightVector = new Vector4(planeRight.normal.x, planeRight.normal.y, planeRight.normal.z, planeRight.distance + bounds.w + _offsetBounds.w);
            else
                planeRightVector = new Vector4(planeRight.normal.x, planeRight.normal.y, planeRight.normal.z, planeRight.distance + _fixedBounds.w + _offsetBounds.w);

 * */

