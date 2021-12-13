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

    void Start()
    {
        towerPos = tower.GetComponent<Transform>().position;
        for (int i = 0; i < numNPC; i++)
            StartCoroutine(LoadGO("Npc2", SetGO));
    }

    private void Update()
    {
        for (int i = 0; i < numNPC; i++)
        {
            if (live[i].active)
            {
                Reach(live[i]);
            }
            else
            {
                StartCoroutine(Reset(live[i]));
            }
        }
    }
    IEnumerator LoadGO(string sPath, System.Action<GameObject> Act)
    {
        Debug.Log("LoadGameObjectAsync -1" + Time.deltaTime);
        GameObject go = null;
        //ResourceRequest rr = Resources.LoadAsync(sPath);
        //if (rr == null)
        //{
        //    yield break;
        //}
        //if (rr.isDone && rr.asset != null)
        //{
        //    go = Instantiate(rr.asset) as GameObject;
        //    Act(go);
        //}
        Object o = Resources.Load(sPath);
        go = Instantiate(o) as GameObject;
        Act(go);
        yield return 0;
    }
    void SetGO(GameObject go)
    {
        SetPos(go);
        SearchPath(go);
        live.Add(go);
    }
    void SetPos(GameObject go) => go.GetComponent<Transform>().position =
            new Vector3(Random.Range(-1f, 1f),
                        0,
                        Random.Range(-1f, 1f));
    void SearchPath(GameObject go)=>go.GetComponent<NPC>().motionData.path = navigation.SearchPath(go.GetComponent<Transform>().position,towerPos);
    void Reach(GameObject go)
    {
        float distance = (towerPos - go.GetComponent<Transform>().position).magnitude;
        if (distance < 0.1f)
        {
            go.SetActive(false);
        }
    }
    IEnumerator Reset(GameObject go)
    {
        yield return new WaitForSeconds(2.0f);
        SetGO(go);
        SearchPath(go);
        go.SetActive(true);
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
        catch (System.Exception e)
        {
        }
    }
}
