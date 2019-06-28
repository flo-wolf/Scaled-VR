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
        [SerializeField] private float _intensitiyFadeInDuration = 0.25f;
        [SerializeField] private float _intensitiyFadeOutDuration = 0.5f;

        [Header("Offsets (don't touch, lol)")]
        [SerializeField] private float _xToleranceBack = 5f;
        [SerializeField] private float _xToleranceFront = 0f;

       
        private Light _light;
        private float _initialIntensity;
        private Coroutine _lerpCoroutine = null;

        private bool _withinArea = false;

        // Start is called before the first frame update
        void Start()
        {
            _light = GetComponent<Light>();
            
            _initialIntensity = _light.intensity;
            _light.intensity = 0f;
        }

        // Update is called once per frame
        void Update()
        {
            if (IsWithinArea())
            {
                if (!_withinArea)
                {
                    _withinArea = true;
                    LerpIntensity(_initialIntensity);
                }
            }
            else 
            {
                if (_withinArea)
                {
                    _withinArea = false;
                    LerpIntensity(0f);
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

        private void LerpIntensity(float endIntensity)
        {
            if (_lerpCoroutine != null)
            {
                StopCoroutine(_lerpCoroutine);
                _lerpCoroutine = null;
            }

            if(endIntensity > _light.intensity)
                _lerpCoroutine = StartCoroutine(C_LerpIntensity(endIntensity, true));
            else
                _lerpCoroutine = StartCoroutine(C_LerpIntensity(endIntensity, false));
        }

        IEnumerator C_LerpIntensity(float endIntensity, bool fadeIn)
        {
            float t = 0f;
            float startIntensity = _light.intensity;

            //duration
            float duration;
            if (fadeIn)
                duration = _intensitiyFadeInDuration;
            else
                duration = _intensitiyFadeOutDuration;

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
                        _light.intensity = Mathfx.Sinerp(startIntensity, endIntensity, (t / duration));
                        break;
                    case Interpolation.EaseOut:
                        _light.intensity = Mathfx.Coserp(startIntensity, endIntensity, (t / duration));
                        break;
                    case Interpolation.SmoothStep:
                        _light.intensity = Mathfx.SmoothStep(startIntensity, endIntensity, (t / duration));
                        break;
                }
                yield return null;
            }
            yield return null;
        }
    }
}

