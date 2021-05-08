using System.Collections;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class BallAgent_Sensor_Road : Agent
{
    public Rigidbody rBody;
    public Transform tBody;
    public Transform tTarget, tSensor;
    public float speed = 10f;
    public float BtT, Checkpoint;
    private float oldBtT = 0;
    public bool selfreset = true;
    public override void Initialize()
    {
        rBody = GetComponent<Rigidbody>();
        tBody = GetComponent<Transform>();
    }
    public override void OnEpisodeBegin()
    {
        if (selfreset == true) { SelfReset(); }
        Checkpoint = 0;
        TargetReset();
        MaxStep = 40000;
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(tBody.localPosition);
        sensor.AddObservation(tTarget.localPosition);
        sensor.AddObservation(rBody.angularVelocity);
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

        if (BtT < oldBtT)
        {
            SetReward((oldBtT-BtT)*0.01f);
        }

        if (BtT < 1.5f)
        {
            if (Checkpoint == 7)
            {
                SetReward(10f);
                SetReward(100f);
                EndEpisode();
            }
            else
            {
                SetReward(10f);
                Checkpoint += 1;
                TargetReset();
            }
        }
        if (tBody.localPosition.y < -1)
        {
            EndEpisode();
        }
        if ((Checkpoint == 3)&(tBody.localPosition.y>1))
        {
            SetReward(0.01f);
        }


        if (MaxStep<-1)
        {
            EndEpisode();
        }

        if(tTarget.localPosition.y < -1)
        {
            TargetReset();
        }
        MaxStep -= 1;
        oldBtT = BtT;

    }
    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = -Input.GetAxisRaw("Vertical");
        actionsOut[1] = Input.GetAxisRaw("Horizontal");
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
        tBody.localPosition = new Vector3(0, 1, 0);
        rBody.angularVelocity = Vector3.zero;
    }
    void TargetReset()
    {
        if (Checkpoint == 0)
        { tTarget.localPosition = new Vector3(45, 0.5f, 30); }

        else if (Checkpoint == 1)
        { tTarget.localPosition = new Vector3(-25, 0.5f, 45); }

        else if (Checkpoint == 2)
        { tTarget.localPosition = new Vector3(-30, 7, 25); }

        else if (Checkpoint == 3)
        { tTarget.localPosition = new Vector3(-25, 0.5f, -15); }

        else if (Checkpoint == 4)
        { tTarget.localPosition = new Vector3(40, 0.5f, -25); }

        else if (Checkpoint == 5)
        { tTarget.localPosition = new Vector3(40, 0.5f, -55); }

        else if (Checkpoint == 6)
        { tTarget.localPosition = new Vector3(4, 0.5f, -55); }

        else if (Checkpoint == 7)
        { tTarget.localPosition = new Vector3(4, 0.5f, 8); }
    }
}
