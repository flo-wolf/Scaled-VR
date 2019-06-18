using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityControl : MonoBehaviour
{
    public Vector3 gravity = new Vector3 (0, 9.81f ,0);

    private Rigidbody _rigid;

    // Start is called before the first frame update
    void Start()
    {
        _rigid = GetComponent<Rigidbody>();    
    }

    // Update is called once per frame
    void Update()
    {
        _rigid.velocity = _rigid.velocity * 0 + gravity * Time.deltaTime;
    }
}
