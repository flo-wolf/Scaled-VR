using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Change
{
    public class Dissolve_Bounds : MonoBehaviour
    {
        public Material material;

        [SerializeField] private Vector4 _bounds = Vector4.zero;
        [SerializeField] private float _scaleDuration = 1f;

        private Coroutine _scaleCoroutine = null;

        // Start is called before the first frame update
        void Start()
        {
            Scale.onScaleEvent.AddListener(OnScaleEvent);

            SetBounds(_bounds);
        }

        private void OnScaleEvent(Food.Emission emission)
        {
            Vector4 bounds = new Vector4(emission.areaConsumption, emission.areaConsumption, emission.areaConsumption, emission.areaConsumption);

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

                SetBounds(_bounds);

                yield return null;
            }
            yield return null;
        }

        private void SetBounds(Vector4 bounds)
        {
            Plane planeRight = new Plane(Vector3.right, transform.position);
            Plane planeFront = new Plane(Vector3.forward, transform.position);

            Vector4 planeRightVector = new Vector4(planeRight.normal.x, planeRight.normal.y, planeRight.normal.z, planeRight.distance + bounds.x);
            Vector4 planeLeftVector = new Vector4(-planeRight.normal.x, planeRight.normal.y, planeRight.normal.z, planeRight.distance + bounds.y);
            Vector4 planeFrontVector = new Vector4(planeFront.normal.x, planeFront.normal.y, planeFront.normal.z, planeFront.distance + bounds.z);
            Vector4 planeBackVector = new Vector4(planeFront.normal.x, planeFront.normal.y, -planeFront.normal.z, planeFront.distance + bounds.w);

            material.SetVector("_PlaneRight", planeRightVector);
            material.SetVector("_PlaneLeft", planeLeftVector);
            material.SetVector("_PlaneFront", planeFrontVector);
            material.SetVector("_PlaneBack", planeBackVector);
        }
    }
}

