using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    Navigation navigation = new Navigation();
    public GameObject controlled;
    public GameObject point;
    public bool isSmoothing;
    NPC npc;
    Vector3 currentTarget;
    Vector3 currentPosition;
    float distanceToGoal;
    Ray r;
    private void Awake()
    {
        npc = controlled.GetComponent<NPC>();
        for (int i = 0; i < navigation.nodeList.Count; i++)
        {
            GameObject gn = Instantiate(point, navigation.nodeList[i].Pos, Quaternion.identity);
            gn.name = i + "";
        }
    }
    void Start()
    {
    }
    // Update is called once per frame
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
                if (distanceToGoal > 0f) 
                    npc.motionData.path.Clear();
                distanceToGoal = (currentPosition - currentTarget).magnitude;
                npc.motionData.path = navigation.SearchPath(currentPosition, currentTarget);
            }
        }
        r = new Ray(currentPosition, currentTarget);
        if (distanceToGoal > npc.motionData.tolerence)
        {
            distanceToGoal = (currentPosition - currentTarget).magnitude;
            if (!Physics.Raycast(r, out RaycastHit rh, 1000.0f, 1 << LayerMask.NameToLayer("Wall")))
            {
                Debug.Log("Go straightly");
                //why penetrate the wall?
                npc.motionData.path = navigation.Reach(currentTarget);
            }
        }

    }
}
