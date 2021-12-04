using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ePathNodeState
{
    NONE = -1,
    OPENED,
    CLOSED
};

public class PathNode
{
    public List<PathNode> neibors = new List<PathNode>();
    public string name;
    public Vector3 Pos;
    public PathNode parent;
    public float fG;
    public float fH;
    public float fT;
    public ePathNodeState eState;
    public PathNode(string name, Vector3 Pos)
    {
        this.name = name;
        this.Pos = Pos;
    }

    public void CalfT() => fT = fG + fH;
    public void Init()
    {
        fG = 0;
        fH = 0;
        fT = 0;
        parent = null;
        eState = ePathNodeState.NONE;
    }
}
