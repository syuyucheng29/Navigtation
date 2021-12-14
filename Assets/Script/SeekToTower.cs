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
        tol = Mathf.Abs(towerPos[1]);
        for (int i = 0; i < numNPC; i++)
        {
            StartCoroutine(LoadGO("Npc2", SetGO));
        }
        new WaitForSeconds(1f);
    }

    void Update()
    {
        for (int i = 0; i < live.Count; i++)
        {
            if (live[i].activeSelf)
            {
                Reach(live[i]);
            }
            else
            {
                if (!live[i].GetComponent<NPC>().isRequested)
                {
                    StartCoroutine(ResetGO(live[i]));
                    live[i].GetComponent<NPC>().isRequested = true;
                }
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
        Vector3 pos = new Vector3(Random.Range(-5f, 5f),
                        0,
                        Random.Range(-5f, 5f));
        go.GetComponent<Transform>().position = pos;
        go.GetComponent<NPC>().motionData.target = pos;
    }
    void SearchPath(GameObject go)=>navigation.SearchPath(towerPos, go.GetComponent<NPC>().motionData);
    void Reach(GameObject go)
    {
        float distance = (towerPos - go.GetComponent<Transform>().position).magnitude;
        if (distance < tol+go.GetComponent<NPC>().motionData.initH)
        {
            go.SetActive(false);
            go.GetComponent<NPC>().isRequested = false;
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
