using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class BallAgent_Dragons : Agent
{
    public Transform tTarget, tHead;
    private Transform tMe;

    public float Fspeed = 0.25f, Rspeed = 5f;
    public float UP, RIGHT, FOWARD, M2T;
    public override void Initialize()
    {
        tMe = GetComponent<Transform>();
    }

    public override void OnEpisodeBegin()
    {
        MaxStep = 5000;
        selfReset();
        targetReset();
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(tMe.localPosition);
        sensor.AddObservation(tTarget.localPosition);
        sensor.AddObservation(tMe.localRotation);
        //10
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        if (vectorAction[0] == 2) { UP = 1; }
        else { UP = -vectorAction[0]; }
        if (vectorAction[1] == 2) { RIGHT = 1; }
        else { RIGHT = -vectorAction[1]; }
        if (vectorAction[2] == 2) { FOWARD = 1; }
        else { FOWARD = Mathf.Clamp(-vectorAction[2], 0, 1); }

        Moving();
        Check();
    }
    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = Input.GetAxis("Horizontal");
        actionsOut[2] = Input.GetAxis("Vertical");
    }
    void Moving()
    {
        tMe.Translate(Vector3.forward * FOWARD * Fspeed );
        tMe.Rotate(Vector3.right * RIGHT * Rspeed );
        tMe.Rotate(Vector3.up * UP * Rspeed);
    }
    void Check()
    {
        M2T = Vector3.Distance(tHead.position, tTarget.position);
        if (M2T < 1f)
        {
            SetReward(1.0f);
            //targetReset();
            EndEpisode();
        }
        if ( (Mathf.Abs(tMe.localPosition.x) > 12) | (Mathf.Abs(tMe.localPosition.y) > 12) | (Mathf.Abs(tMe.localPosition.z) > 12) )
        {
            EndEpisode();
        }

        if (MaxStep < 0) { EndEpisode(); }
        //else { MaxStep -= 1; }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Cat")
        {
            EndEpisode();
        }
    }
    float[] getRandom()
    {
        float[] r = new float[3];
        for(int i=0; i<3; i++)
        {
            r[i] = Random.Range(-8f, 8f);
        }
        return r;
    }
    void selfReset()
    {
        float[] r = getRandom();
        tMe.localPosition = new Vector3(r[0], r[1], r[2]);
    }
    void targetReset()
    {
        float[] r = getRandom();
        tTarget.localPosition = new Vector3(r[0], r[1], r[2]);
    }
}
