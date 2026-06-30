using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// プレイヤーが取得できるコイン
/// </summary>
public class Coin : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (CoinManager.instance != null)
        {
            CoinManager.instance.AddCoin();
        }

        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.AddCoin();
        }

        Destroy(gameObject);
    }
}