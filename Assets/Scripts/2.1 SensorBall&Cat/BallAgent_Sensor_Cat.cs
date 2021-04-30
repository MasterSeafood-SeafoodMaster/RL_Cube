using System.Collections;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class BallAgent_Sensor_Cat: Agent
{
    public Rigidbody rBody;
    public Transform tTarget, tBody, tSensor;
    public float speed = 10f;
    public float BtT;
    private float oldBtT = 0;
    public bool selfreset = false;
    public override void Initialize()
    {
        rBody = GetComponent<Rigidbody>();
        tBody = GetComponent<Transform>();
    }
    public override void OnEpisodeBegin()
    {
        if (selfreset == true) { SelfReset(); }
        TargetReset();
        MaxStep = 10000;
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(rBody.angularVelocity);
        sensor.AddObservation(tBody.localPosition);
        sensor.AddObservation(tTarget.localPosition);
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
        if (tTarget.localPosition.y <= -1)
        {
            TargetReset();
        }
        if (oldBtT > BtT)
        {
            SetReward(0.01f);
        }

        if (BtT < 1.5f)
        {
            SetReward(100f);
            TargetReset();
        }
        if (tBody.localPosition.y < -1)
        {
            SelfReset();
            EndEpisode();
        }
        if (MaxStep<-1)
        {
            SetReward(50f);
            EndEpisode();
        }
        oldBtT = BtT;
        MaxStep -= 1;

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
        tBody.localPosition = new Vector3(Random.Range(-8, 8), 0.5f, Random.Range(-8, 8));
        rBody.angularVelocity = Vector3.zero;
    }
    void TargetReset()
    {
        tTarget.localPosition = new Vector3(Random.Range(-8, 8), 0.5f, Random.Range(-8, 8));
    }
}
