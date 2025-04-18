using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> Children of this class become single, global.  </summary>
/// <typeparam name="T"></typeparam>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }

    protected void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        Instance = this as T;
    }

    protected void OnApplicationQuit()
    {
        Instance = null;
        Destroy(gameObject);
    }
}
