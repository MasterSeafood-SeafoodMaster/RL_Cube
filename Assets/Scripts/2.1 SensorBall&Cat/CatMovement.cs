using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatMovement : MonoBehaviour
{
    public Transform Food;
    private Vector3 oldDir = Vector3.zero;
    void Start()
    {
        this.transform.localPosition = new Vector3(Random.Range(8, -8), 0.25f, Random.Range(8, -8));
        ResetFood();
    }
    void FixedUpdate()
    {
        float Cat2Food = Vector3.Distance(this.transform.localPosition, Food.localPosition);
        if ( Cat2Food<=0.5 )
        {
            ResetFood();
        }
        this.transform.LookAt(Food);
        this.transform.position += transform.forward * Time.deltaTime * 1f;

    }
    void ResetFood()
    {
        Food.localPosition = new Vector3(Random.Range(8, -8), 0.5f, Random.Range(8, -8));
    }
}
