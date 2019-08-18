using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOffset : MonoBehaviour
{
    [SerializeField] private float _offsetIncrement = 0.1f;

    private float _offsetY;
    private float _startY;

    private float _offsetYmin = -4;
    private float _offsetYmax = 4;


    // Start is called before the first frame update
    void Start()
    {
        _startY = transform.position.y;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            _offsetY += _offsetIncrement;
            _offsetY = Mathf.Clamp(_offsetY, _offsetYmin, _offsetYmax);
            UpdateOffset();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            _offsetY -= _offsetIncrement;
            _offsetY = Mathf.Clamp(_offsetY, _offsetYmin, _offsetYmax);
            UpdateOffset();
        }
    }

    private void UpdateOffset()
    {
        transform.position = new Vector3(transform.position.x, _startY + _offsetY, transform.position.z);
    }
}
