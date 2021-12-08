using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public MotionData motionData = new MotionData();

    public float maxSpeed;
    public float maxAngularSpeed;
    public float mass;
    public float slowingRadius;
    void Awake()
    {
        motionData.go = this.gameObject;
        motionData.target = this.GetComponent<Transform>().position;
        motionData.maxSpeed = maxSpeed;
        motionData.maxAngularSpeed = maxAngularSpeed;
        motionData.mass = mass;
        motionData.slowingRadius = slowingRadius;
    }
    void Update()
    {
        Motion.Seek(motionData);
    }
    void OnDrawGizmos()
    {
        try
        {
            if (motionData.distance > 0.1f)
            {
                Gizmos.color = Color.blue; //destination
                Gizmos.DrawWireSphere(motionData.target, 0.5f);
                Gizmos.color = Color.green;
                Gizmos.DrawLine(motionData.transform.position, motionData.target);
            }
        }
        catch (Exception e)
        {
            //Debug.Log(e);
        }
    }
}
