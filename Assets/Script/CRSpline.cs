using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CRSpline
{
    public static CRSpline instance;
    Vector3[] point;
    float[] knot;
    Vector3[] A;
    Vector3[] B;
    Vector3 C;
    float alpha;
    float[] intervalforA;
    float[] intervalforB;

    //reference: https://en.wikipedia.org/wiki/Centripetal_Catmull%E2%80%93Rom_spline
    public CRSpline()
    {
        instance = this;
        point = new Vector3[4];
        knot = new float[4];
        A = new Vector3[3];
        B = new Vector3[2];
        intervalforA = new float[3];
        intervalforB = new float[2];
        C = new Vector3();
        for (int i = 0; i < 4; i++)
            knot[i] = 0f;
    }
    public Vector3[] GetCRSpline(int num, Vector3[] npoint, float alpha)
    {
        Vector3[] result = new Vector3[num];
        for (int i = 0; i < 4; i++)
            point[i] = npoint[i];
        this.alpha = alpha;
        float[] t = new float[num];

        if (alpha > 1 || alpha < 0) Debug.LogError($"alpha={alpha} must be within [0-1]");

        CalKnot();
        CalInterval();
        CalSeq(num, t);

        for (int i = 0; i < num; i++)
        {
            CalA(t[i]);
            CalB(t[i]);
            result[i] = CalC(t[i]);
        }
        return result;
    }
    void CalKnot()
    {
        for (int i = 1; i < 4; i++)
            knot[i] = Mathf.Pow((point[i] - point[i - 1]).magnitude, alpha) + knot[i - 1];
    }
    void CalInterval()
    {
        for (int i = 0; i < 3; i++)
            intervalforA[i] = 1 / (knot[i + 1] - knot[i]);
        for (int i = 0; i < 2; i++)
            intervalforB[i] = 1 / (knot[i + 2] - knot[i]);
    }
    void CalSeq(int num, float[] t)
    {
        float d = (float)1 / (num - 1);
        for (int i = 0; i < num; i++)
            t[i] = knot[1] + (knot[2] - knot[1]) * (float)d * i;
    }
    void CalA(float t)
    {
        for (int i = 0; i < 3; i++)
            A[i] = (knot[i + 1] - t) * intervalforA[i] * point[i] + (t - knot[i]) * intervalforA[i] * point[i + 1];
    }
    void CalB(float t)
    {
        for (int i = 0; i < 2; i++)
            B[i] = (knot[i + 2] - t) * intervalforB[i] * A[i] + (t - knot[i]) * intervalforB[i] * A[i + 1];
    }
    Vector3 CalC(float t)
    {
        float interval = knot[2] - knot[1];
        interval = 1 / interval;
        return (knot[2] - t) * interval * B[0] + (t - knot[1]) * interval * B[1];
    }
}
