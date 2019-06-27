using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KCalDoll_StartOffset : MonoBehaviour
{

    public int startOffset = 0;

    MeshRenderer renderer;
    MaterialPropertyBlock props;

    // Use this for initialization
    void Start()
    {
        renderer = GetComponent<MeshRenderer>();
        props = new MaterialPropertyBlock();

        props.SetFloat("_startOffset", startOffset);
        
        renderer.SetPropertyBlock(props);
    }
}
