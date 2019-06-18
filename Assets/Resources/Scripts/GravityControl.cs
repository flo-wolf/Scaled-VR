using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityControl : MonoBehaviour
{
    Rigidbody rigid;

    public Vector3 gravity = new Vector3 (0, 9.81f ,0);

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();    
    }

    // Update is called once per frame
    void Update()
    {
        rigid.velocity = rigid.velocity * 0 + gravity * Time.deltaTime;
    }
}
