using System.Collections;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class BallAgent_Sensor: Agent
{
    public Rigidbody rBody;
    public Transform tTarget, tBody, tSensor;
    public float speed = 10f;
    public float BtT;
    public bool selfreset = false;
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
    }
    public override void OnActionReceived(float[] vectorAction)
    {
        tSensor.eulerAngles = new Vector3(0, 0, 0);
        float FS, RS = 0f;

        if (vectorAction[0] == 2) { FS = 1; }
        else { FS = -vectorAction[0]; }
        if (vectorAction[1] == 2) { RS = 1; }
        else { RS = -vectorAction[1]; }

        rBody.AddForce(FS * speed, 0, RS * speed);

        BtT = Vector3.Distance(tBody.localPosition, tTarget.localPosition);

        if (BtT < 1.5f)
        {
            SetReward(1f);
            TargetReset();
        }
        if (tBody.localPosition.y < -1)
        {
            SelfReset();
            EndEpisode();
        }
        if(MaxStep<=-1)
        {
            EndEpisode();
        }
        MaxStep -= 1;
    }
    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = -Input.GetAxisRaw("Vertical");
        actionsOut[1] = -Input.GetAxisRaw("Horizontal");
    }
    void SelfReset()
    {
        tBody.localPosition = new Vector3(Random.Range(-8, 8), 0.5f, Random.Range(-8, 8));
        rBody.angularVelocity = Vector3.zero;
    }
    void TargetReset()
    {
        tTarget.localPosition = new Vector3(Random.Range(-8, 8), 0.5f, Random.Range(-8, 8));
    }

}
