using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
    private Transform tMw;
    public float Speed = 1.5f;

    private void Start()
    {
        tMw = GetComponent<Transform>();
    }
    void Update()
    {
        tMw.Rotate(Vector3.forward * Speed);
    }
}
