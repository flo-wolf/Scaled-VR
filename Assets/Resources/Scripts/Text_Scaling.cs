using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Text_Scaling : MonoBehaviour
{
    [SerializeField] private Camera _cam;
    [SerializeField] private float _rotationSpeed = 2f;
    [SerializeField] private float _minRotAngleX = 40f;
    [SerializeField] private float _maxRotAngleX = 140f;
    private Vector3 _initialScale;
    private Quaternion _rotation;
    

    // Start is called before the first frame update
    void Start()
    {
        //cam = Camera.main;
        Plane plane = new Plane(_cam.transform.forward, _cam.transform.position);
        //_initialDistance = plane.GetDistanceToPoint(transform.position);
        _initialScale = transform.localScale;  // record initial scale, use this as a basis
        _rotation = GetComponent<RectTransform>().rotation;
    }

    void Update()
    {
        Plane plane = new Plane(_cam.transform.forward, _cam.transform.position);
        float dist = plane.GetDistanceToPoint(transform.position);

        dist = (_cam.transform.position - transform.position).magnitude;
        transform.localScale = _initialScale * Mathf.Abs(dist);

        Vector3 scale = transform.localScale;
        //scale.y *= -1;
        //scale.x *= -1;
        transform.localScale = scale;

        //_rotation = Quaternion.Euler(_cam.transform.rotation.x, _cam.transform.rotation.y, _rekt.rotation.z);
        //transform.LookAt(transform.position + _rotation * Vector3.forward, _rotation * Vector3.up);

        var lookPos = _cam.transform.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(lookPos);

        Vector3 rotationAngles = rotation.eulerAngles;
        rotationAngles.z = 0;
        //rotationAngles.x = Mathf.Clamp(rotationAngles.x, _minRotAngleX, _maxRotAngleX);
        rotation = Quaternion.Euler(rotationAngles);

        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * _rotationSpeed);
    }
}
