using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionData
{
    public Stack<Vector3> path=new Stack<Vector3>();
    public GameObject go;
    public Transform transform { get => go.GetComponent<Transform>(); }
    public float maxSpeed;
    public float maxAngularSpeed;
    public float mass;
    public float momI;
    public float slowingRadius;
    public float initH;
    public float tolerence { get => 0.05f + transform.position[1]; }
    private Vector3 _target;
    public Vector3 target
    {
        set => _target = value;
        get => _target;
    }
    public Vector3 targetOffset { get => _target - go.GetComponent<Transform>().position; }
    public Vector3 nTargetOffset { get => Vector3.Normalize(targetOffset); }

    //Motion
    public float distance { get => targetOffset.magnitude; }
    public float rampedSpeed
    {
        get
        {
            try
            {
                if (path.Count > 0)
                    return maxSpeed;
            }
            catch (Exception e)
            {
                return maxSpeed * (distance / slowingRadius);
            }
            return maxSpeed;
        }
    }
    public float surgeForce { get => Vector3.Dot(go.GetComponent<Transform>().forward, targetOffset); }
    public float yawMoment { get => Vector3.Dot(go.GetComponent<Transform>().right, targetOffset); }
    public Vector3 surgeVelocity { get => transform.forward * Mathf.Min(rampedSpeed, Mathf.Abs(surgeForce / mass * Time.deltaTime), maxSpeed); }

    public void GetInitHeight()
    {
        Vector3 currentPosition = go.GetComponent<Transform>().position;
        Physics.Raycast(new Ray(currentPosition, -Vector3.up), out RaycastHit rh, 1000.0f, 1 << LayerMask.NameToLayer("Terrain"));
        initH = currentPosition[1] - rh.point[1];
    }
}
