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

    [HideInInspector]
    public bool isRequested = false;

    void Awake()
    {
        motionData.go = this.gameObject;
        motionData.target = motionData.transform.position;
        motionData.maxSpeed = maxSpeed;
        motionData.maxAngularSpeed = maxAngularSpeed;
        motionData.mass = mass;
        motionData.momI = 0.004f;
        motionData.slowingRadius = slowingRadius;
        motionData.GetInitHeight();
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

                Gizmos.color = Color.red; //next way point
                Vector3 a = motionData.path.Peek();
                Gizmos.DrawWireSphere(a, 0.8f);
            }
        }
        catch (Exception e)
        {
            //Debug.Log(e);
        }
    }
}
