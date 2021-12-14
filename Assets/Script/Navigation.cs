using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
public class Navigation
{
    public static Navigation m_Instance;
    public List<PathNode> nodeList = new List<PathNode>();
    public bool isSmoothing = true;
    public int intermediatePoints=3;
    string txtPath = "Assets/WP.txt";
    CRSpline crspline = new CRSpline();

    public Navigation()
    {
        m_Instance = this;
        ReadWP();
    }

    public void ReadWP()
    {
        nodeList.Clear();
        StreamReader sr = new StreamReader(txtPath);
        Debug.Log(sr.ReadLine());
        string sline = "";
        //Read WP
        sline = sr.ReadLine();
        while (!sline.Contains("relation"))
        {
            string[] tokens = sline.Split(':');
            PathNode newNode = new PathNode(tokens[0], stringToVector3(tokens[1]));
            nodeList.Add(newNode);
            sline = sr.ReadLine();
        }
        //Read WP relation
        while (sr.Peek() != -1)
        {
            sline = sr.ReadLine();
            string[] slink = sline.Split(new char[0]);
            int[] myInts = Array.ConvertAll(slink, int.Parse);
            for (int i = 1; i < myInts.Length; i++)
                nodeList[myInts[0]].neibors.Add(nodeList[myInts[i]]);
        }
        if (nodeList.Count() == 0)
            Debug.LogError("Way points exported failed, Regenerate again!");
    }
    public void SearchPath(Vector3 end, MotionData md)
    {
        Vector3 start = md.transform.position;
        Debug.Log($"{end}");
        InitNode(md);
        if (CheckVisuality(start, end))
        {
            Debug.Log("On visual");
            BuildPath(start, end, md);
        }
        else
        {
            Debug.Log("Astar activated");
            Astar(start, end, md);
            BuildPath(start, end, md);
            if (isSmoothing)
            {   
                SmoothingPath(md);
                md.path = md.smoothPath;
                return;
            }
                
        }
        md.path = md.samplePath;
    }
    public bool CheckVisuality(Vector3 start, Vector3 end)
    {
        Ray ray = new Ray(start, end - start);
        if (!Physics.Raycast(ray, out RaycastHit rh, (end - start).magnitude, 1 << LayerMask.NameToLayer("Wall")))
            return true;
        return false;
    }
    public void Astar(Vector3 start, Vector3 end, MotionData md)
    {
        Debug.Log("Astar Solving");
        (md.sNode, md.eNode) = GetCloestToES(start, end);
        md.openList.Add(md.sNode);
        //search open nodes
        while (md.openList.Count > 0)
        {
            PathNode cNode = GetSmallestGNode(md);

            if (cNode.fH < 0.05f)
            {
                md.eNode = cNode;
                break;
            }
            else if (CheckVisuality(cNode.Pos, end))
            {
                md.eNode = cNode;
                break;
            }
            else
            {
                //search neighbor nodes
                for (int i = 0; i < cNode.neibors.Count; i++)
                {
                    PathNode nNode = cNode.neibors[i];
                    if (nNode.eState == ePathNodeState.CLOSED)
                    {
                        continue;
                    }
                    else
                    {
                        float cton = GetDistanceSquare(nNode.Pos, cNode.Pos) + cNode.fG;
                        if (nNode.eState == ePathNodeState.OPENED)
                        {
                            if (cton < nNode.fG)
                            {
                                nNode.parent = cNode;
                                nNode.fG = cton;
                                nNode.CalfT();
                            }
                        }
                        else
                        {
                            nNode.eState = ePathNodeState.OPENED;
                            md.openList.Add(nNode);
                            nNode.fG = cton;
                            nNode.CalfT();
                            nNode.parent = cNode;
                        }
                    }
                    cNode.eState = ePathNodeState.CLOSED;
                }
            }
            md.openList.Remove(cNode);
        }
    }
    void InitNode(MotionData md)
    {
        md.eNode = null;
        md.sNode = null;
        for (int i = 0; i < nodeList.Count(); i++)
            nodeList[i].Init();
        md.openList.Clear();
    }
    public void BuildPath(Vector3 start, Vector3 end, MotionData md)
    {
        md.samplePath.Clear();
        md.samplePath.Push(end);
        if (md.eNode == null)
        {
            md.samplePath.Push(start);
        }
        else
        {
            PathNode cNode = md.eNode;

            Vector3 nextPos = cNode.Pos;
            Vector3 extraPos = Vector3.LerpUnclamped(end, nextPos, 0.5f);
            md.samplePath.Push(extraPos);
            md.samplePath.Push(nextPos);
            while (cNode.parent != null)
            {
                cNode = cNode.parent;
                md.samplePath.Push(cNode.Pos);
            }
            md.samplePath.Push(start);
        }
    }
    (PathNode sNode, PathNode eNode) GetCloestToES(Vector3 start, Vector3 end)
    {
        float minDE = int.MaxValue, minDS = int.MaxValue;
        int iE = 0, iS = 0;

        for (int i = 0; i < nodeList.Count(); i++)
        {
            nodeList[i].fH = GetDistanceSquare(end, nodeList[i].Pos);
            nodeList[i].fG = GetDistanceSquare(start, nodeList[i].Pos);
            if (minDE > nodeList[i].fH)
            {
                minDE = nodeList[i].fH;
                iE = i;
            }
            if (minDS > nodeList[i].fG)
            {
                minDS = nodeList[i].fG;
                iS = i;
            }
        }
        return (nodeList[iS], nodeList[iE]);
    }
    PathNode GetSmallestGNode(MotionData md) => md.openList.Where(a => a.fG == md.openList.Min(x => x.fG)).Take(1).SingleOrDefault();
    Vector3 stringToVector3(string sVector)
    {
        // Remove the parentheses
        if (sVector.StartsWith("(") && sVector.EndsWith(")"))
            sVector = sVector.Substring(1, sVector.Length - 2);

        // split the items
        string[] sArray = sVector.Split(',');

        // store as a Vector3
        Vector3 result = new Vector3(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]),
            float.Parse(sArray[2]));

        return result;
    }
    float GetDistanceSquare(Vector3 point1, Vector3 point2)
    {
        Vector3 interval = point1 - point2;
        return Vector3.Dot(interval, interval);
    }
    void SmoothingPath(MotionData md)
    {
        List<Vector3> totalPath = new List<Vector3>();
        int num = intermediatePoints;
        Vector3[] point = new Vector3[4];
        Vector3[] newPath;
        Vector3[] oldPath= new Vector3[md.samplePath.Count()];

        for (int i = 0; i < oldPath.Length; i++)
            oldPath[i] = md.samplePath.Pop();

        totalPath.Add(oldPath[0]);

        int j = 0;
        for(int i=0; i< (oldPath.Length - 4 + 1); i++)
        {
            for(int k=0; k<4; k++)
            {
                point[k] = oldPath[k + j];
            }
            j++;
            newPath = crspline.GetCRSpline(num, point, 0.5f);
            for (int h = 0; h < newPath.Length; h++)
                totalPath.Add(newPath[h]);
        }
        totalPath.Add(oldPath[oldPath.Length-1]);

        for (int i = totalPath.Count() - 1; i >= 0; i--)
        {
            md.smoothPath.Push(totalPath[i]);
            md.record.Add(totalPath[i]);
        }
        totalPath.Clear();
    }
}
