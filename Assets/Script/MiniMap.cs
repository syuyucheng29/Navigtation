using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DrawInfo
{
    public Vector3 point;
    public int cubeSize;
    public Color color;
}

public class MiniMap : MonoBehaviour
{
    GameObject miniMap;
    Ray ray;
    int mapW, mapH;

    public GameObject zone;
    Vector3 zoneSize;
    Vector3 _zoneNewOrigin;

    public GameObject camera;
    Vector3 cameraPos;

    Texture2D _texture;
    GameObject label;
    int _textureW;
    int _textureH;
    Texture _dstTexture;

    GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;

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
        _zoneNewOrigin = new Vector3(0 - zoneSize[0] / 2, 0, 0 - zoneSize[2] / 2);

        cameraPos = camera.GetComponent<Transform>().position;

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
                Vector2 pointInTexture = GetV2ScreenToTexture(point);
                Vector3 pointInWorld = GetV3TextureToWorld(pointInTexture, cameraPos[1]);
                Debug.Log($"In texture: {pointInTexture}|| In world: {pointInWorld}.");
                camera.GetComponent<Transform>().position = pointInWorld;
            }
        }

        if (Input.GetKey(KeyCode.Mouse1))
        {
            ResetCamera();
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
    }
    public void DrawOnMiniMap(DrawInfo box) => DrawOnMiniMap(box.point, box.cubeSize, box.color);
    /// <summary>
    /// Draw a cube to miniMap corresponding to location in world coordinate
    /// </summary>
    /// <param name="point">position in world coordinate</param>
    /// <param name="cubeSize">how many pixel at side of cube</param>
    /// <param name="color">color of cube in miniMap</param>
    public void DrawOnMiniMap(Vector3 point, int cubeSize, Color color)
    {
        cubeSize = (cubeSize % 2 == 1) ? cubeSize : cubeSize + 1;
        Vector2 towerPosInTexture = GetV2WorldToTexture(point);
        for (int i = -cubeSize / 2; i <= cubeSize / 2; i++)
        {
            for (int j = -cubeSize / 2; j <= cubeSize / 2; j++)
            {
                int[] fillat = Culling((int)towerPosInTexture[0] + i, (int)towerPosInTexture[1] + j);
                _texture.SetPixel(fillat[0], fillat[1], color);
            }
        }
    }
    public void DrawDone()
    {
        _texture.Apply();
        label.GetComponent<RawImage>().texture = _texture;
    }
    Vector2 GetV2WorldToTexture(Vector3 pointInWorld)
    {
        Vector2 result = new Vector2();
        Vector3 shift = pointInWorld - _zoneNewOrigin;
        //Debug.Log($"shift={shift},pointInWorld={pointInWorld}");
        result[0] = (int)(shift[0] / zoneSize[0] * _textureW);
        result[1] = (int)(shift[2] / zoneSize[2] * _textureH);
        return result;
    }
    Vector2 GetV2ScreenToTexture(Vector2 pointInScreen) => pointInScreen - new Vector2(Screen.width - _textureW, 0);
    Vector3 GetV3TextureToWorld(Vector2 pointInTexture, float y)
    {
        Vector3 result = new Vector3();
        Vector2 shift = pointInTexture - new Vector2(_textureW / 2, _textureH / 2);
        result[0] = shift[0] / (_textureW / 2) * (zoneSize[0] / 2);
        result[1] = y;
        result[2] = shift[1] / (_textureH / 2) * (zoneSize[2] / 2);
        return result;
    }
    public void ResetCamera() => camera.GetComponent<Transform>().position = cameraPos;
    /// <summary>
    /// Check pixel position is on miniMap field
    /// </summary>
    /// <param name="x">x-position of pixel at miniMap</param>
    /// <param name="y">y-position of pixel at miniMap</param>
    /// <returns></returns>
    int[] Culling(int x, int y)
    {
        int[] result = new int[2];
        result[0] = (x > 0) ? ((x >= _textureW - 1) ? _textureW - 1 : x) : 0;
        result[1] = (y > 0) ? ((y >= _textureH - 1) ? _textureH - 1 : y) : 0;
        return result;
    }
}
