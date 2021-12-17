using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MiniMap : MonoBehaviour
{
    GameObject miniMap;
    Ray ray;
    int mapW, mapH;

    public GameObject zone;
    Vector3 zoneSize, zoneCenter;
    Vector3 _zoneNewOrigin;

    Texture2D _texture;
    GameObject label;
    int _textureW;
    int _textureH;
    Texture _dstTexture;

    public GameObject tower;

    GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;

    Vector2 maxScreen;

    private void Awake()
    {
        miniMap = transform.Find("Map").gameObject;
        label = transform.Find("Label").gameObject;
        _dstTexture = label.GetComponent<RawImage>().texture;

        mapW = (int)miniMap.GetComponent<RectTransform>().rect.width;
        mapH = (int)miniMap.GetComponent<RectTransform>().rect.height;
        _textureW = mapW;
        _textureH = mapH;

        zoneSize = zone.GetComponent<Collider>().bounds.size;
        zoneCenter = zone.GetComponent<Transform>().position;
        _zoneNewOrigin = new Vector3(0 - zoneSize[0] / 2, 0, 0 - zoneSize[2] / 2);

        m_Raycaster = GetComponent<GraphicRaycaster>();
        m_EventSystem = GetComponent<EventSystem>();
        InitTexture();
    }

    void FixedUpdate()
    {
        //ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //if (Physics.Raycast(ray, out RaycastHit rh, 10f, 1 << LayerMask.NameToLayer("MiniMap")))
        //{
        //    Debug.Log($"{rh.point}");
        //}

        if (Input.GetKey(KeyCode.Mouse0))
        {
            m_PointerEventData = new PointerEventData(m_EventSystem);
            m_PointerEventData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();

            m_Raycaster.Raycast(m_PointerEventData, results);

            foreach (RaycastResult result in results)
            {
                Vector2 point = result.screenPosition;
                Vector2 pointInMap = new Vector2((point[0] - Screen.width) / mapW, point[1] / mapH);
                Debug.Log("Hit " + result.gameObject.name + " " + point + " " + pointInMap);
            }
        }
    }

    void InitTexture()
    {
        _texture = new Texture2D(_textureW, _textureH, textureFormat: TextureFormat.ARGB32, mipCount: 3, linear: true);
        _texture.filterMode = FilterMode.Point; // no smooth pixels

        Color _color;
        for (int i = 0; i < _textureW; i++)
        {
            for (int j = 0; j < _textureH; j++)
            {
                if (i < 2 || i > _textureW - 3 || j < 2 || j > _textureH - 3)
                    _color = Color.black;
                else
                    _color = new Color(255f, 255f, 255f, 0f);
                _texture.SetPixel(i, j, _color);
            }
        }

        int[] towerPosInTexture = GetCoordinateInTexture(tower.GetComponent<Transform>().position);
        Debug.Log($"{towerPosInTexture[0]}, {towerPosInTexture[1]}");
        for (int i = -5; i <= 5; i++)
        {
            for (int j = -5; j <= 5; j++)
            {
                int[] fillat = Culling(towerPosInTexture[0] + i, towerPosInTexture[1] + j);
                _texture.SetPixel(fillat[0], fillat[1], Color.red);
            }
        }

        _texture.Apply();
        label.GetComponent<RawImage>().texture = _texture;
    }

    int[] GetCoordinateInTexture(Vector3 pointInWorld)
    {
        int[] result = new int[2];
        Vector3 shift = pointInWorld - _zoneNewOrigin;
        Debug.Log($"shift={shift},zoneSize={zoneSize}");
        result[0] = (int)(shift[0] / zoneSize[0] * _textureW);
        result[1] = (int)(shift[2] / zoneSize[2] * _textureH);
        return result;
    }

    int[] Culling(int x, int y)
    {
        int[] result = new int[2];
        result[0] = (x > 0) ? ((x >= _textureW - 1) ? _textureW - 1 : x) : 0;
        result[1] = (y > 0) ? ((y >= _textureH - 1) ? _textureH - 1 : y) : 0;

        Debug.Log($"{x},{y} ==> {result[0]},{result[1]} (max= {_textureW},{_textureH})");
        return result;
    }
}
