using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Motion : MonoBehaviour
{
    static public void Seek(MotionData md)
    {
        float tolerence = 0.2f+md.initH;

        try
        {
            if (md.distance < tolerence)
            {
                md.target = md.path.Pop();
            }
        }
        catch (Exception e)
        {
        }
        md.transform.position += md.surgeVelocity * Time.deltaTime;
        if (md.distance > 0.1f)
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
