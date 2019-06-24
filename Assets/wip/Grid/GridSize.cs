using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSize : MonoBehaviour
{
    public Vector3[] Points;
    private LineRenderer _line;

    // Start is called before the first frame update
    void Start()
    {
        _line = GetComponent<LineRenderer>();
        _line.enabled = true;

        _line.SetPosition(0, Points[0]);
        _line.SetPosition(1, Points[1]);
        _line.SetPosition(2, Points[2]);
        _line.SetPosition(3, Points[3]);
    }

    // Update is called once per frame
    void Update()
    {
        _line.SetPosition(0, Points[0]);
        _line.SetPosition(1, Points[1]);
        _line.SetPosition(2, Points[2]);
        _line.SetPosition(3, Points[3]);
    }
}
