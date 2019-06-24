using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class SSAASuperSampling : MonoBehaviour
{
    [SerializeField] [Range(1,5)] private float renderScale = 1f;

    void Start()
    {
        XRSettings.eyeTextureResolutionScale = renderScale;
    }
}
