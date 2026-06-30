using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// MonoBehaviourを継承していないクラスから
/// コルーチンを実行するための管理クラス
/// </summary>
public class CoroutineManager : MonoBehaviour
{
    public static CoroutineManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// コルーチンを実行する
    /// </summary>
    public Coroutine RunCoroutine(IEnumerator coroutine)
    {
        if (coroutine == null)
        {
            Debug.LogWarning("Coroutine is null.");
            return null;
        }

        return StartCoroutine(coroutine);
    }
}