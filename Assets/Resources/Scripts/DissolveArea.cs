using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Change
{
    public class DissolveArea : MonoBehaviour
    {
        public static float FixedWidth { get; } = 10f;
        public enum Side { Right, Left, Front, Back }

        [Header("Target")]
        public List<Material> materials = new List<Material>();

        [Header("Scaling")]
        [SerializeField] private float _scaleDuration = 1f;
        [SerializeField] private Vector4 _fixedBounds = Vector4.zero;

        [Header("Debugging")]
        [SerializeField] private Vector4 _bounds = Vector4.zero; // "strech" of the sides of a horizontal plane centering in transform.position into the four directions
        // right, left, front, back

        private Coroutine _scaleCoroutine = null;
        private Side _sideToScale = Side.Left; // not fully implemeted.


        // Start is called before the first frame update
        void Start()
        {
            Scale.onScaleEvent.AddListener(OnScaleEvent);

            // set starting bounds
            if(_sideToScale == Side.Right || _sideToScale == Side.Left)
                _bounds = new Vector4(0, 0, FixedWidth, FixedWidth); // set front and back of the area plane (width of the area)
            else // front back
                _bounds = new Vector4(FixedWidth, FixedWidth, 0, 0); // set front and back of the area plane (width of the area)

            _bounds = new Vector4(0, 0, FixedWidth, FixedWidth); // set front and back of the area plane (width of the area)
            SetBounds(_bounds);
        }

        private void OnScaleEvent(Food.Emission emission)
        {
            /*
            float sideLength = Mathf.CeilToInt(emission.areaSqrMeters / FixedWidth);

            if (emission.areaSqrMeters > 0)
                sideLength += 0.1f;
                */

            float sideLength = emission.areaSqrMeters / FixedWidth;

            // old: Vector4 nextBounds = new Vector4(sideLength + _offsetBounds.x, 0, FixedWidth + _offsetBounds.z, FixedWidth + _offsetBounds.w);
            Vector4 nextBounds = _bounds;

            switch (_sideToScale)
            {
                case Side.Right:
                    nextBounds = new Vector4(0, sideLength, FixedWidth, FixedWidth);
                    break;
                case Side.Left:
                    nextBounds = new Vector4(sideLength, 0, FixedWidth, FixedWidth);
                    break;
                case Side.Front:
                    nextBounds = new Vector4(FixedWidth, FixedWidth, FixedWidth, FixedWidth);
                    break;
                case Side.Back:
                    nextBounds = new Vector4(FixedWidth, FixedWidth, FixedWidth, FixedWidth);
                    break;
            }

            if (_scaleCoroutine != null)
            {
                StopCoroutine(_scaleCoroutine);
                _scaleCoroutine = null;
            }

            _scaleCoroutine = StartCoroutine(C_ScaleBounds(nextBounds, _scaleDuration));
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
            Plane planeRight = new Plane(transform.right, transform.position);
            Plane planeFront = new Plane(transform.forward, transform.position);

            Vector4 planeRightVector;
            Vector4 planeLeftVector;
            Vector4 planeFrontVector;
            Vector4 planeBackVector;

            // x
            if (_fixedBounds.x == 0)
                planeRightVector = new Vector4(planeRight.normal.x, planeRight.normal.y, planeRight.normal.z, planeRight.distance + bounds.x);
           else
               planeRightVector = new Vector4(planeRight.normal.x, planeRight.normal.y, planeRight.normal.z, planeRight.distance + _fixedBounds.x);

            // y
           if (_fixedBounds.y == 0)
                planeLeftVector = new Vector4(-planeRight.normal.x, planeRight.normal.y, planeRight.normal.z, planeRight.distance + bounds.y);
           else
                planeLeftVector = new Vector4(-planeRight.normal.x, planeRight.normal.y, planeRight.normal.z, planeRight.distance + _fixedBounds.y);

            // z
           if (_fixedBounds.z == 0)
                planeFrontVector = new Vector4(planeFront.normal.x, planeFront.normal.y, planeFront.normal.z, planeFront.distance + bounds.z);
           else
                planeFrontVector = new Vector4(planeFront.normal.x, planeFront.normal.y, planeFront.normal.z, planeFront.distance + _fixedBounds.z);

            // w
           if (_fixedBounds.w == 0)
                planeBackVector = new Vector4(planeFront.normal.x, planeFront.normal.y, -planeFront.normal.z, planeFront.distance + bounds.w);
           else
                planeBackVector = new Vector4(planeFront.normal.x, planeFront.normal.y, -planeFront.normal.z, planeFront.distance + _fixedBounds.w);


           foreach(Material material in materials)
            {
                material.SetVector("_PlaneRight", planeRightVector);
                material.SetVector("_PlaneLeft", planeLeftVector);
                material.SetVector("_PlaneFront", planeFrontVector);
                material.SetVector("_PlaneBack", planeBackVector);
            }
        }

        //private void Update()
        //{
        //    SetBounds(_bounds);
        //}

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

