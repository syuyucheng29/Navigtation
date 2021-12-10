using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekToTower : MonoBehaviour
{
    Navigation navigation = new Navigation();
    public GameObject controlled;
    public GameObject point;
    List<GameObject> store = new List<GameObject>();
    NPC npc;
    Vector3 currentTarget;
    Vector3 currentPosition;
    float distanceToGoal;
    float initH = 0;
    Ray r;
    private void Awake()
    {
        npc = controlled.GetComponent<NPC>();
        GetInitHeight();
        navigation.intermediatePoints = 5;
    }
    void Start()
    {
    }
    void Update()
    {
        currentPosition = controlled.GetComponent<Transform>().position;

        if (Input.GetMouseButtonDown(0))
        {
            r = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(r, out RaycastHit rh, 1000.0f, 1 << LayerMask.NameToLayer("Terrain")))
            {
                currentTarget = rh.point;
                currentTarget[1] += initH;
                if (distanceToGoal > 0f)
                    npc.motionData.path.Clear();
                distanceToGoal = (currentPosition - currentTarget).magnitude;
                npc.motionData.path = navigation.SearchPath(currentPosition, currentTarget);
            }
        }
    }
    void OnDrawGizmos()
    {
        try
        {
            if (navigation.record.Count > 0)
            {
                for (int i = 0; i < navigation.record.Count; i++)
                {
                    Gizmos.color = Color.black;
                    Gizmos.DrawWireSphere(navigation.record[i], 0.2f);
                }
            }
        }
        catch (Exception e)
        {
        }
    }

    public void ReadWP()
    {
        for (int i = 0; i < store.Count; i++)
        {
            Destroy(store[i]);
        }
        store.Clear();
        navigation.ReadWP();
        for (int i = 0; i < navigation.nodeList.Count; i++)
        {
            GameObject gn = Instantiate(point, navigation.nodeList[i].Pos, Quaternion.identity);
            store.Add(gn);
            gn.name = i + "";
        }
    }
    public void ResetGizmos()
    {
        navigation.record.Clear();
    }
    void GetInitHeight()
    {
        Vector3 currentPosition = controlled.GetComponent<Transform>().position;
        Physics.Raycast(new Ray(currentPosition, -Vector3.up), out RaycastHit rh, 1000.0f, 1 << LayerMask.NameToLayer("Terrain"));
        initH = currentPosition[1] - rh.point[1];
        npc.motionData.initH = initH;
    }
}
