using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    Navigation navigation = new Navigation();
    NPC npc;
    public GameObject controlled;
    public GameObject target;
    public bool isSmoothing;
    Vector3 currentTarget;
    Vector3 previousTarget;
    Vector3 currentPosition;
    private void Awake()
    {
        npc = controlled.GetComponent<NPC>();
    }
    void Start()
    {
    }
    // Update is called once per frame
    void Update()
    {
        currentPosition = controlled.GetComponent<Transform>().position;
        currentTarget = target.GetComponent<Transform>().position;
        if ((previousTarget - currentTarget).magnitude > 0.1f)
        {
            npc.motionData.path.Clear();
            npc.motionData.path = navigation.SearchPath(currentPosition, currentTarget);
            previousTarget= currentTarget;
        }
    }
}
