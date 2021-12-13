using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    Navigation navigation = new Navigation();
    public GameObject controlled;
    public GameObject point;
    public bool isSmoothing;
    public int intermediatePoints;
    public Toggle toggle;
    List<GameObject> store = new List<GameObject>();
    NPC npc;
    Vector3 currentTarget;
    Vector3 currentPosition;
    float distanceToGoal;
    Ray r;
    private void Awake()
    {
        npc = controlled.GetComponent<NPC>();
        navigation.intermediatePoints = intermediatePoints;
        //show pathNode
        for (int i = 0; i < navigation.nodeList.Count; i++)
        {
            GameObject gn = Instantiate(point, navigation.nodeList[i].Pos, Quaternion.identity);
            store.Add(gn);
            gn.name = i + "";
        }
    }
    void Start()
    {
        toggle.onValueChanged.AddListener(delegate {
            ToggleSmooth();
        });
    }
    void Update()
    {
        navigation.isSmoothing = isSmoothing;
        currentPosition = controlled.GetComponent<Transform>().position;

        if (Input.GetMouseButtonDown(0))
        {
            r = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(r, out RaycastHit rh, 1000.0f, 1 << LayerMask.NameToLayer("Terrain")))
            {
                currentTarget = rh.point;
                currentTarget[1] += npc.motionData.initH;
                if (distanceToGoal > 0f) 
                    npc.motionData.path.Clear();
                distanceToGoal = (currentPosition - currentTarget).magnitude;
                navigation.SearchPath(currentTarget, npc.motionData);
            }
        }
    }
    void OnDrawGizmos()
    {
        try
        {
            int count = npc.motionData.record.Count;
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    Gizmos.color = Color.black;
                    Gizmos.DrawWireSphere(npc.motionData.record[i], 0.2f);
                }
            }
        }
        catch (Exception e)
        {
        }
    }
    /// <summary>
    /// To detect interval between model and ground
    /// </summary>
    
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
        npc.motionData.record.Clear();
    }
    void ToggleSmooth()=>isSmoothing = toggle.isOn;
}
