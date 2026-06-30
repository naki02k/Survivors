using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// コインをドロップする処理を管理するクラス
/// </summary>
public class CoinDropper : MonoBehaviour
{
    [SerializeField]
    private GameObject coinPrefab;

    [SerializeField]
    private int minCoins = 1;
    [SerializeField]
    private int maxCoins = 5;

    [SerializeField]
    private float dropRadius = 1f;

    /// <summary>
    /// 指定位置にコインをドロップする
    /// </summary>
    public void DropCoins(Vector3 dropPosition)
    {
        if (coinPrefab == null)
        {
            Debug.LogError("Coin Prefab is not assigned.");
            return;
        }

        int coinCount = Random.Range(minCoins, maxCoins + 1);

        for (int i = 0; i < coinCount; i++)
        {
            Vector3 randomOffset =
                new Vector3(
                    Random.Range(-dropRadius, dropRadius),
                    Random.Range(-dropRadius, dropRadius),
                    0f);

            Vector3 finalDropPosition =
                dropPosition + randomOffset;

            Instantiate(
                coinPrefab,
                finalDropPosition,
                Quaternion.identity);
        }
    }
}