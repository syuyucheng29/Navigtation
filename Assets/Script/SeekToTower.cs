using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekToTower : MonoBehaviour
{
    Navigation navigation = new Navigation();
    List<GameObject> live = new List<GameObject>();
    public GameObject Zone;
    Vector3 zoneSize, zoneCenter, zoneField;
    public int maxNumNPC;
    public GameObject tower;
    Vector3 towerPos;
    float tol;
    int cNumNPC = 0;

    void Awake()
    {
        zoneSize = Zone.GetComponent<Collider>().bounds.size;
        zoneCenter = Zone.GetComponent<Transform>().position;
        zoneField = zoneSize / 2 - zoneCenter;
    }
    void Start()
    {
        towerPos = tower.GetComponent<Transform>().position;
        tol = Mathf.Abs(towerPos[1]);
        //for (int i = 0; i < numNPC; i++)
        //{
        //    StartCoroutine(LoadGO("Npc2", SetGO));
        //}
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (cNumNPC < maxNumNPC && Physics.Raycast(r, out RaycastHit rh, 1000f, 1 << LayerMask.NameToLayer("Terrain")))
            {
                cNumNPC++;
                Debug.Log($"{rh.point}");
                StartCoroutine(LoadGO("Npc2", new Vector2(rh.point[0], rh.point[2]), SetGO));
            }
        }

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
    IEnumerator LoadGO(string sPath, Vector2 inputPoint, System.Action<GameObject, Vector2> Act)
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
            Act(go, inputPoint);
        }
    }
    void SetGO(GameObject go)
    {
        SetPos(go);
        SearchPath(go);
        live.Add(go);
    }

    void SetGO(GameObject go, Vector2 inputPoint)
    {
        SetPos(go, inputPoint);
        SearchPath(go);
        live.Add(go);
    }

    void SetPos(GameObject go)
    {
        Vector3 pos = new Vector3(
                Random.Range(-zoneField[0], zoneField[0]),
                0,
                Random.Range(-zoneField[2], zoneField[2]));
        go.GetComponent<Transform>().position = pos;
        go.GetComponent<NPC>().motionData.target = pos;
    }

    void SetPos(GameObject go, Vector2 inputPoint)
    {
        Vector3 pos = new Vector3(
                inputPoint[0],
                0,
                inputPoint[1]);
        go.GetComponent<Transform>().position = pos;
        go.GetComponent<NPC>().motionData.target = pos;
    }

    void SearchPath(GameObject go) => navigation.SearchPath(towerPos, go.GetComponent<NPC>().motionData);
    void Reach(GameObject go)
    {
        float distance = (towerPos - go.GetComponent<Transform>().position).magnitude;
        if (distance < tol + go.GetComponent<NPC>().motionData.initH)
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

    public void ChangeScene(string sceneName) => ChangeSceneManager.ChangeScene(sceneName);
}
