using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChangeSceneManager : MonoBehaviour
{
    public static ChangeSceneManager instance=null;
    private GameObject menuUI;
    private GameObject gameUI;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogErrorFormat(gameObject,
                    "Multiple instances of {0} is not allow", GetType().Name);
            Debug.LogErrorFormat($"{gameObject}");
            DestroyImmediate(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        instance = this;

        menuUI = transform.FindAnyChild<Transform>("MenuUI").gameObject;
        menuUI.SetActive(true);
        gameUI = transform.FindAnyChild<Transform>("GameUI").gameObject;
        gameUI.SetActive(true);
    }
    public static void ChangeScene(string sceneName) => SceneManager.LoadScene(sceneName);
    public void OnEnable()=>SceneManager.sceneLoaded += OnSceneLoaded;
    public void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "Menu")
            menuUI.SetActive(false);
        else
            menuUI.SetActive(true);

        gameUI.SetActive(!menuUI.activeSelf);
    }
}
