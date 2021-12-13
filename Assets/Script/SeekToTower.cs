using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekToTower : MonoBehaviour
{
    Navigation navigation = new Navigation();
    List<GameObject> live = new List<GameObject>();
    public int numNPC;
    public GameObject tower;
    Vector3 towerPos;
    float tol;

    void Start()
    {
        towerPos = tower.GetComponent<Transform>().position;
        tol = towerPos[1]*towerPos[1];
        for (int i = 0; i < numNPC; i++)
        {
            StartCoroutine(LoadGO("Npc2", SetGO));
        }
            
    }

    private void Update()
    {
        for (int i = 0; i < numNPC; i++)
        {
            if (live[i].activeSelf)
            {
                Reach(live[i]);
            }
            else
            {
                StartCoroutine(ResetGO(live[i]));
            }
        }
    }
    IEnumerator LoadGO(string sPath, System.Action<GameObject> Act)
    {
        GameObject go = null;
        ResourceRequest rr = Resources.LoadAsync(sPath);
        if (rr == null)
        {
            yield break;
        }
        yield return rr;
        if (rr.isDone && rr.asset != null)
        {
            go = Instantiate(rr.asset) as GameObject;
            Act(go);
        }
    }
    void SetGO(GameObject go)
    {
        SetPos(go);
        SearchPath(go);
        live.Add(go);
    }
    void SetPos(GameObject go)
    {
        Vector3 pos = new Vector3(Random.Range(-1f, 1f),
                        0,
                        Random.Range(-1f, 1f));
        go.GetComponent<Transform>().position = pos;
        go.GetComponent<NPC>().motionData.target = pos;
    }
    void SearchPath(GameObject go)=>navigation.SearchPath(towerPos, go.GetComponent<NPC>().motionData);
    void Reach(GameObject go)
    {
        float distance = (towerPos - go.GetComponent<Transform>().position).magnitude;
        if (distance < tol)
        {
            go.SetActive(false);
        }
    }
    IEnumerator ResetGO(GameObject go)
    {
        yield return new WaitForSeconds(2.0f);
        SetPos(go);
        SearchPath(go);
        go.SetActive(true);
        
    }
}
