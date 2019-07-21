using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// siwtch light on and off depending on the length of the acreage area. 
// => only have lights illuminate the acreage, have them off when there is no acreage.

namespace Change
{
    public class AcreageLight : MonoBehaviour
    {
        public enum Interpolation { EaseIn, EaseOut, SmoothStep}

        [Header("Fading")]
        [SerializeField] private float _fadeInDuration = 0.075f;
        [SerializeField] private float _fadeOutDuration = 0.175f;
        [SerializeField] private bool _fadeRange = true;
        [SerializeField] private bool _fadeIntensity = false;

        [Header("Offsets (don't touch, lol)")]
        [SerializeField] private float _xToleranceBack = 0f;
        [SerializeField] private float _xToleranceFront = 0f;

       
        private Light _light;
        private float _initialIntensity;
        private float _initialRange;

        private Coroutine _lerpCoroutine = null;
        private bool _withinArea = false;

        // Start is called before the first frame update
        void Start()
        {
            _light = GetComponent<Light>();

            _initialRange = _light.range;
            _initialIntensity = _light.intensity;

            if(_fadeIntensity)
                _light.intensity = 0f;
            if(_fadeRange)
                _light.range = 0f;
        }

        // Update is called once per frame
        void Update()
        {
            if (IsWithinArea())
            {
                if (!_withinArea)
                {
                    _withinArea = true;
                    LerpIntensity(_initialIntensity, _initialRange);
                }
            }
            else 
            {
                if (_withinArea)
                {
                    _withinArea = false;
                    LerpIntensity(0f, 0f);
                }
            }
        }

        private bool IsWithinArea()
        {
            // to the left of the starting position => could be within the area
            if (DissolveArea.StartX - _xToleranceFront > transform.position.x)
            {
                // the the right of the end position => is within area
                if (DissolveArea.StartX - DissolveArea.SideLength - _xToleranceBack < transform.position.x)
                    return true;
            }
            return false;
        }

        private void LerpIntensity(float endIntensity, float endRange)
        {
            if (_lerpCoroutine != null)
            {
                StopCoroutine(_lerpCoroutine);
                _lerpCoroutine = null;
            }

            if(endIntensity > _light.intensity)
                _lerpCoroutine = StartCoroutine(C_LerpIntensity(endIntensity, endRange, true));
            else
                _lerpCoroutine = StartCoroutine(C_LerpIntensity(endIntensity, endRange, false));
        }

        IEnumerator C_LerpIntensity(float endIntensity, float endRange, bool fadeIn)
        {
            float t = 0f;
            float startIntensity = _light.intensity;
            float startRange = _light.range;

            //duration
            float duration;
            if (fadeIn)
                duration = _fadeInDuration;
            else
                duration = _fadeOutDuration;

            // interpolation
            Interpolation interpolation;
            if (endIntensity > startIntensity)
                interpolation = Interpolation.EaseIn;
            else
                interpolation = Interpolation.EaseOut;


            while (t < duration)
            {
                t += Time.deltaTime;

                switch (interpolation)
                {
                    case Interpolation.EaseIn:
                        if(_fadeIntensity)
                            _light.intensity = Mathfx.Sinerp(startIntensity, endIntensity, (t / duration));
                        if(_fadeRange)
                            _light.range = Mathfx.Sinerp(startRange, endRange, (t / duration));
                        break;
                    case Interpolation.EaseOut:
                        if (_fadeIntensity)
                            _light.intensity = Mathfx.Coserp(startIntensity, endIntensity, (t / duration));
                        if (_fadeRange)
                            _light.range = Mathfx.Coserp(startRange, endRange, (t / duration));
                        break;
                    case Interpolation.SmoothStep:
                        if (_fadeIntensity)
                            _light.intensity = Mathfx.SmoothStep(startIntensity, endIntensity, (t / duration));
                        if (_fadeRange)
                            _light.range = Mathfx.SmoothStep(startRange, endRange, (t / duration));
                        break;
                }
                yield return null;
            }
            yield return null;
        }
    }
}

