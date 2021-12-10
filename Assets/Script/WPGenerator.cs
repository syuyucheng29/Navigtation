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
    GameObject[] wallList;
    Vector3 zoneSize, zoneCenter, zoneField;
    int linkLimit = 0;
    int nodeCount = 0;
    int level = 2;
    

    void Awake()
    {
        wallList = GameObject.FindGameObjectsWithTag("Wall");
        zoneSize = Zone.GetComponent<Collider>().bounds.size;
        zoneCenter = Zone.GetComponent<Transform>().position;
        zoneField = zoneSize / 2 - zoneCenter;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) //generate wp 
        {
            GenerateRandomWP();
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            OutputWP();
        }
    }
    public void GenerateRandomWP()
    {
        Debug.Log("Generate Random Distribution Way Points");
        linkLimit += 2;
        int i = 0;
        while (i < 10)
        {
            Vector3 nodePosition = new Vector3(
                Random.Range(-zoneField[0], zoneField[0]),
                0,
                Random.Range(-zoneField[2], zoneField[2]));

            if (!DetectInBound(nodePosition))
            {
                GenerateNode(nodePosition);
                i++;
            }
        }
        CreateLink();
    }
    public void GenerateUniformWP()
    {
        linkLimit += 1;
        linkLimit = (linkLimit > 8) ? 8 : linkLimit;
        nodeCount = 0;
        Debug.Log("Generate Uniform Distribution Way Points");
        DestoryWP();
        level++;
        float ds = (float)1 / (level - 1);
        for (int i = 0; i < level; i++)
        {
            for (int j = 0; j < level; j++)
            {
                Vector3 nodePosition = new Vector3(
                    (-zoneField[0] + (float)zoneSize[0] * ds * i),
                    0,
                    (-zoneField[2] + (float)zoneSize[2] * ds * j));
                if (!DetectInBound(nodePosition))
                    GenerateNode(nodePosition);
            }
        }
        CreateLink();
    }
    bool DetectInBound(Vector3 position)
    {
        for (int j = 0; j < wallList.Length; j++)
        {
            Debug.Log($"{wallList[j].name}");
            if (wallList[j].GetComponent<Collider>().bounds.Contains(position))
            {
                Debug.Log($"point is inside collider {wallList[j].name}");
                return true;
            }
        }
        return false;
    }
    void GenerateNode(Vector3 position)
    {
        GameObject newNode = Instantiate(WP, position, Quaternion.identity);
        newNode.name = nodeCount + "";
        nodeList.Add(newNode);
        nodeCount++;
    }
    void CreateLink()
    {
        for (int i = 0; i < nodeList.Count; i++)
            nodeList[i].GetComponent<WP>().neibors.Clear();

        Vector3 between;
        float interval;
        for (int i = 0; i < nodeList.Count; i++)
        {
            for (int j = 0; j < nodeList.Count; j++)
            {
                if (i == j) {
                    nodeList[j].GetComponent<WP>().distance = float.MaxValue;
                    continue;
                }
                
                between = nodeList[j].transform.position - nodeList[i].transform.position;
                interval = between.magnitude;
                
                //On visual?
                Ray r = new Ray(nodeList[i].transform.position, between);
                RaycastHit rh;
                if (Physics.Raycast(r, out rh, interval, 1 << LayerMask.NameToLayer("Wall")))
                {
                    nodeList[j].GetComponent<WP>().distance = float.MaxValue;
                    continue;
                }
                else
                {
                    Debug.Log($"interval==={interval}");
                    nodeList[j].GetComponent<WP>().distance = interval;
                }
            }

            List<GameObject> goList=nodeList.OrderBy(m => m.GetComponent<WP>().distance).Take(linkLimit).ToList();
            foreach(GameObject k in goList)
            {
                nodeList[i].GetComponent<WP>().neibors.Add(k);
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

        DestoryWP();
    }
    void DestoryWP()
    {
        for (int i = 0; i < nodeList.Count; i++)
            Destroy(nodeList[i]);
        nodeList.Clear();
    }
}