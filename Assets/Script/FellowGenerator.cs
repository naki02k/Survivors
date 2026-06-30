using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 一定時間ごとに仲間キャラクターを生成するクラス
/// </summary>
public class FellowGenerator : MonoBehaviour
{
    /// <summary>
    /// 仲間キャラクターのプレハブ
    /// </summary>
    [SerializeField]
    private GameObject fellowPrefab;

    /// <summary>
    /// 仲間生成間隔
    /// </summary>
    [SerializeField]
    private float spawnInterval = 3f;

    /// <summary>
    /// プレイヤー参照
    /// </summary>
    private Player player;

    /// <summary>
    /// 経過時間
    /// </summary>
    private float currentTime;

    private void Start()
    {
        player = Player.instance;
    }

    private void Update()
    {
        if (player == null)
        {
            return;
        }

        currentTime += Time.deltaTime;

        if (currentTime >= spawnInterval)
        {
            FellowGenerate(player.transform.position);
            currentTime = 0f;
        }
    }

    /// <summary>
    /// プレイヤー周辺に仲間を生成する
    /// </summary>
    /// <param name="targetPosition">
    /// プレイヤーの位置
    /// </param>
    public void FellowGenerate(Vector2 targetPosition)
    {
        if (fellowPrefab == null)
        {
            return;
        }

        Vector2 randomOffset = new Vector2(
            Random.Range(-3f, 3f),
            Random.Range(-3f, 3f));

        Vector2 fellowSpawnPosition =
            targetPosition + randomOffset;

        GameObject newFellow =
            Instantiate(
                fellowPrefab,
                fellowSpawnPosition,
                Quaternion.identity);

        Fellow fellow = newFellow.GetComponent<Fellow>();

        if (fellow != null)
        {
            fellow.SetPlayerTransform(player.transform);
        }
    }
}