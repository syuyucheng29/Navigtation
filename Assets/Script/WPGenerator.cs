using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class WPGenerator : MonoBehaviour
{
    public static WPGenerator m_instance;
    public WPGenerator() => m_instance = this;
    List<GameObject> nodeList = new List<GameObject>();
    public GameObject WP;
    public GameObject Zone;
    Vector3 zoneSize, zoneCenter, zoneField;
    bool bLoaded = false;
    int linkLimit = 0;
    int nodeCount = 0;
    float linkRadius = 0.0f;
    GameObject[] wallList;

    void Awake()
    {
        wallList = GameObject.FindGameObjectsWithTag("Wall");
        zoneSize = Zone.GetComponent<Collider>().bounds.size;
        zoneCenter = Zone.GetComponent<Transform>().position;
        zoneField = zoneSize / 2 - zoneCenter;
        linkRadius = 0.5f * Mathf.Sqrt(2 * (zoneSize[0] + zoneSize[1] + zoneSize[2]));
    }

    void Start()
    {
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) //generate wp 
        {
            GenerateWP();
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            OutputWP();
        }
    }
    public void GenerateWP()
    {
        Debug.Log("Generation");
        linkLimit += 2;
        int i = 0;
        while (i < 10)
        {
            //how to discover more?
            bool inBoundingBox = false;
            Vector3 nodePosition = new Vector3(
                Random.Range(-zoneField[0], zoneField[0]),
                0,
                Random.Range(-zoneField[2], zoneField[2]));

            //remove point in collider
            for (int j = 0; j < wallList.Length; j++)
            {
                Debug.Log($"{wallList[j].name}");
                if (wallList[j].GetComponent<Collider>().bounds.Contains(nodePosition))
                {
                    Debug.Log($"point is inside collider {wallList[j].name}");
                    inBoundingBox = true;
                    break;
                }
            }
            if (!inBoundingBox)
            {
                GameObject newNode = Instantiate(WP, nodePosition, Quaternion.identity);
                newNode.name = nodeCount + "";
                nodeList.Add(newNode);
                nodeCount++;
                i++;
            }
        }
        CreateLink();
    }
    void CreateLink()
    {
        for (int i = 0; i < nodeList.Count; i++)
            nodeList[i].GetComponent<WP>().neibors.Clear();

        Vector3 between;
        float distance;
        for (int i = 0; i < nodeList.Count; i++)
        {
            int count = 0;
            for (int j = 0; j < nodeList.Count; j++)
            {
                if (i == j) continue;
                between = nodeList[j].transform.position - nodeList[i].transform.position;
                distance = between.magnitude;
                if (distance > linkRadius) 
                    continue;
                //On visual?
                Ray r = new Ray(nodeList[i].transform.position, between);
                RaycastHit rh;
                if (Physics.Raycast(r, out rh, distance, 1 << LayerMask.NameToLayer("Wall")))
                {
                    continue;
                }
                else
                {
                    //linked
                    nodeList[i].GetComponent<WP>().neibors.Add(nodeList[j]);
                    count++;
                    if (count > linkLimit) 
                        break;
                }
            }
        }
    }
    public void OutputWP()
    {
        Debug.Log("Output WP");
        StreamWriter sw = new StreamWriter("Assets/WP.txt", false);
        sw.WriteLine("WP: (WP name) (Position)");
        foreach (GameObject wp in nodeList)
        {
            string s = "";
            s += wp.name;
            s += ":";
            s += wp.GetComponent<Transform>().position;
            sw.WriteLine(s);
        }
        sw.WriteLine("WP relation");
        foreach (GameObject wp in nodeList)
        {
            string s = "";
            s += wp.name;
            for (int j = 0; j < wp.GetComponent<WP>().neibors.Count(); j++)
                s += " " + wp.GetComponent<WP>().neibors[j].name;
            sw.WriteLine(s);
        }
        sw.Close();

        for (int i = 0; i < nodeList.Count; i++)
            Destroy(nodeList[i]);
        nodeList.Clear();
    }
}