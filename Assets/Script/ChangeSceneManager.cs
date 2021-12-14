using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneManager : MonoBehaviour
{
    public static ChangeSceneManager instance=null;
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
    }

    void Update()
    {
    }

    public static void ChangeScene(string sceneName) => SceneManager.LoadScene(sceneName);
}
