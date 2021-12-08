using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Motion : MonoBehaviour
{
    static public void Seek(MotionData md)
    {
        float tolerence = 0.1f+md.transform.position[1];

        try
        {
               md.target = md.path.Pop();
        }
        catch (Exception e)
        {
            //Debug.Log("fail");
        }
        md.transform.position += md.surgeVelocity * Time.deltaTime;
        if (md.distance > tolerence)
        {
            Vector3 toward = new Vector3(md.targetOffset.x, 0f, md.targetOffset.z);
            Quaternion targetBaseQuaterian = Quaternion.FromToRotation(Vector3.forward, toward);
            md.transform.localRotation = Quaternion.RotateTowards(
                                         md.transform.localRotation,
                                         targetBaseQuaterian,
                                         md.maxAngularSpeed * Time.deltaTime);
        }
    }
}
