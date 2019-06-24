using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementScalerVR : MonoBehaviour
{
    [SerializeField] private Transform _playerCamTrans; 
    [SerializeField] private float _movementMulti = 1.1f;
    [SerializeField] private float _movemetnThreshhold = 0.2f;

    private Vector3 _lastPositon;
    private Vector3 _lastDelta = Vector3.zero;
    private Vector3 _dir;

    void Update()
    {
        // mirror player movement 
        //_lastDelta = _playerCamTrans.position - _lastPositon;

        /*
        _lastDelta = (((_playerCamTrans.position - _lastDelta) - _lastPositon).normalized * _movementMulti);
        transform.position += _lastDelta; 
        _lastPositon = _playerCamTrans.position;
        */

        Vector3 _dir = (_playerCamTrans.localPosition - _lastPositon).normalized;

        if (_dir.sqrMagnitude > _movemetnThreshhold * _movemetnThreshhold)
        {
            _lastDelta = _dir * _movementMulti;
            _lastPositon = _playerCamTrans.localPosition;
            transform.position += _lastDelta;
        }
    }
}
