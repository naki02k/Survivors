using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敵の出現管理とWave進行を管理するクラス
/// </summary>
public class EnemyManager : MonoBehaviour
{
    /// <summary>
    /// EnemyManagerのシングルトンインスタンス
    /// </summary>
    public static EnemyManager Instance { get; private set; }

    [Header("Prefabs")]
    [SerializeField]
    private GameObject[] enemyPrefabs;

    [SerializeField]
    private GameObject bossPrefab;

    [Header("Spawn Settings")]
    [SerializeField]
    private Transform spawnArea;

    [Header("Enemy Count Settings")]

    [SerializeField]
    private int baseEnemyCount = 5;

    [SerializeField]
    private int maxEnemyLimit = 100;

    /// <summary>
    /// プレイヤーから敵を生成する最小距離
    /// </summary>
    private const float EnemyMinDistance = 8f;

    /// <summary>
    /// プレイヤーから敵を生成する最大距離
    /// </summary>
    private const float EnemyMaxDistance = 15f;

    /// <summary>
    /// プレイヤーからボスを生成する最小距離
    /// </summary>
    private const float BossMinDistance = 10f;

    /// <summary>
    /// プレイヤーからボスを生成する最大距離
    /// </summary>
    private const float BossMaxDistance = 18f;

    /// <summary>
    /// スポーン位置探索の最大試行回数
    /// </summary>
    [SerializeField]
    private int maxSpawnAttempts = 50;

    private BoxCollider2D areaCollider;

    /// <summary>
    /// 現在のWaveで倒した敵数
    /// </summary>
    private int defeatedEnemyCount;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (spawnArea != null)
        {
            areaCollider = spawnArea.GetComponent<BoxCollider2D>();
        }
    }

    /// <summary>
    /// 敵撃破時に呼び出す
    /// </summary>
    public void EnemyDefeated()
    {
        defeatedEnemyCount++;

        if (WaveManager.Instance != null &&
            WaveManager.Instance.IsBossWave &&
            defeatedEnemyCount >= 1)
        {
            WaveManager.Instance.HandleBossDefeated();
        }
    }

    /// <summary>
    /// 現在シーン上に存在する敵数を取得
    /// </summary>
    private int GetCurrentEnemyCount()
    {
        return GameObject
            .FindGameObjectsWithTag("Enemy")
            .Length;
    }

    /// <summary>
    /// Waveごとの最大敵数
    /// </summary>
    private int GetMaxEnemyCount()
    {
        if(WaveManager.Instance==null)
        {
            return baseEnemyCount;
        }

        int wave = WaveManager.Instance.CurrentWaveCount;

        int additionalCount = (1 << (wave - 1))-1;

        int maxCount = baseEnemyCount + additionalCount;

        return Mathf.Min(maxCount, maxEnemyLimit);
    }

    /// <summary>
    /// さらに敵を生成可能か
    /// </summary>
    public bool CanSpawnMoreEnemies()
    {
        return GetCurrentEnemyCount()
               < GetMaxEnemyCount();
    }

    /// <summary>
    /// 通常敵を出現させる
    /// </summary>
    public void SpawnNormalEnemy()
    {
        if (enemyPrefabs == null ||
            enemyPrefabs.Length == 0)
        {
            Debug.LogWarning("[EnemyManager] enemyPrefabsがインスペクターで設定されていない、または要素が空です。");
            return;
        }

        if (!CanSpawnMoreEnemies())
        {
            Debug.Log($"[EnemyManager] 生成上限に達しているためスキップ: 現在 {GetCurrentEnemyCount()}体 / 最大 {GetMaxEnemyCount()}体");
            return;
        }

        Vector2 spawnPosition =
            GetSafeRandomPosition(
                GetPlayerPosition(),
                EnemyMinDistance,
                EnemyMaxDistance);

        if (areaCollider != null && spawnPosition == (Vector2)areaCollider.bounds.center)
        {
            Debug.LogWarning("[EnemyManager] プレイヤーとの距離条件を満たす安全なスポーン位置が見つかりませんでした（試行回数上限）。");
        }

        int randomIndex =
            Random.Range(
                0,
                enemyPrefabs.Length);

        Instantiate(
            enemyPrefabs[randomIndex],
            spawnPosition,
            Quaternion.identity);
        Debug.Log($"[EnemyManager] 通常敵を生成しました: {enemyPrefabs[randomIndex].name} (位置: {spawnPosition})");
    }

    /// <summary>
    /// ボス敵を出現させる
    /// </summary>
    public void SpawnBossEnemy()
    {
        if (bossPrefab == null)
        {
            return;
        }

        defeatedEnemyCount = 0;

        Vector2 spawnPosition =
            GetSafeRandomPosition(
                GetPlayerPosition(),
                BossMinDistance,
                BossMaxDistance);

        Instantiate(
            bossPrefab,
            spawnPosition,
            Quaternion.identity);
    }

    /// <summary>
    /// プレイヤーの現在位置を取得する
    /// </summary>
    private Vector2 GetPlayerPosition()
    {
        if (Player.instance != null)
        {
            return Player.instance.transform.position;
        }

        if (areaCollider != null)
        {
            return areaCollider.bounds.center;
        }

        return Vector2.zero;
    }

    /// <summary>
    /// 指定距離内の安全なスポーン位置を取得する
    /// </summary>
    private Vector2 GetSafeRandomPosition(
        Vector2 targetPosition,
        float minDistance,
        float maxDistance)
    {
        if (areaCollider == null)
        {
            return targetPosition;
        }

        Bounds bounds = areaCollider.bounds;

        float minDistanceSquared =
            minDistance * minDistance;

        float maxDistanceSquared =
            maxDistance * maxDistance;

        for (int i = 0; i < maxSpawnAttempts; i++)
        {
            Vector2 candidatePosition =
                new Vector2(
                    Random.Range(
                        bounds.min.x,
                        bounds.max.x),
                    Random.Range(
                        bounds.min.y,
                        bounds.max.y));

            float distanceSquared =
                (candidatePosition -
                 targetPosition).sqrMagnitude;

            if (distanceSquared >= minDistanceSquared &&
                distanceSquared <= maxDistanceSquared)
            {
                return candidatePosition;
            }
        }

        // 条件を満たす座標が見つからなかった場合
        return bounds.center;
    }
}