using System.Collections;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class BallAgent_Slope : Agent
{
    public Rigidbody rBody;
    public Transform tTarget, tBody;
    public Renderer cTarget;
    public float speed = 10f;
    public float BtT;
    public bool selfreset = false;
    private bool isSuccess = false;
    private float oldBtT = 0;
    private float T = 8;
    public override void Initialize()
    {
        rBody = GetComponent<Rigidbody>();
        tBody = GetComponent<Transform>();
    }
    public override void OnEpisodeBegin()
    {
        MaxStep = 5000;
        if (selfreset == true) { SelfReset(); }
        TargetReset();
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(tBody.localPosition);
        sensor.AddObservation(tTarget.localPosition);
    }
    public override void OnActionReceived(float[] vectorAction)
    {
        float FS, RS = 0f;

        if (vectorAction[0] == 2) { FS = 1; }
        else { FS = -vectorAction[0]; }
        if (vectorAction[1] == 2) { RS = 1; }
        else { RS = -vectorAction[1]; }

        rBody.AddForce(FS * speed, 0, RS * speed);

        BtT = Vector3.Distance(tBody.localPosition, tTarget.localPosition);

        if ( (tBody.localPosition.y>0.5) & (oldBtT>BtT) )
        {
            SetReward(0.01f);
        }

        if (BtT < 0.5f)
        {
            isSuccess = true;
        }
        if (tBody.localPosition.y < -1)
        {
            SelfReset();
            SetReward(-50f);
            EndEpisode();
        }
        if ( (tBody.localPosition.y<0.5f) & (isSuccess == true))
        {
            isSuccess = false;
            SetReward(100f);
            //SelfReset();
            EndEpisode();
        }
        if (MaxStep<=-1)
        {
            SelfReset();
            EndEpisode();
        }
        oldBtT = BtT;
        //MaxStep -= 1;

    }
    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = -Input.GetAxisRaw("Vertical");
        actionsOut[1] = -Input.GetAxisRaw("Horizontal");
    }
    
    void OnCollisionEnter(Collision collision)
   {
        if (collision.gameObject.tag == "Cat")
        {
            SelfReset();
            EndEpisode();
        }
    }
    
    void SelfReset()
    {
        tBody.localPosition = new Vector3(T, 0.5f, 0);
        rBody.angularVelocity = Vector3.zero;
        rBody.velocity = Vector3.zero;
        T = -T;
    }
    void TargetReset()
    {
        tTarget.localPosition = new Vector3(0, Random.Range(3f, 3.5f), Random.Range(-0.5f, 0.5f));
    }

}
