using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ウェーブ進行、タイマー、クリア条件を管理するクラス
/// </summary>
public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;

    [Header("Wave Settings")]
    [SerializeField]
    int maxWaveCount = 10;

    [SerializeField]
    float initialWaveTime = 30f;

    [SerializeField]
    float initialSpawnInterval = 1.0f;

    [SerializeField]
    float minSpawnInterval = 0.1f;

    [SerializeField]
    float intervalDecreaseStep = 0.05f;

    [Header("Timings")]
    [SerializeField]
    float clearTextDuration = 2.0f;

    [SerializeField]
    float shopOpenDelay = 1.0f;

    // プロパティ
    public int MaxWaveCount => maxWaveCount;

    public int CurrentWaveCount { get; private set; } = 1;

    public float CurrentWaveTime { get; private set; }

    public bool IsBossWave { get; private set; } = false;

    private bool canSpawnEnemies = true;
    private bool isTimeRunning = false;

    /// <summary>
    /// 現在の敵出現間隔
    /// </summary>
    private float currentSpawnInterval;

    private GameUIManager uiManager;
    private Coroutine spawnRoutine;

    void Awake()
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
    }

    void Start()
    {
        uiManager = GameUIManager.instance;

        currentSpawnInterval = initialSpawnInterval;

        if (uiManager != null)
        {
            uiManager.UpdateWaveText(
                CurrentWaveCount,
                maxWaveCount
            );
        }
        Debug.Log($"<color=cyan>【Wave開始】Wave{CurrentWaveCount}がスタートしました！</color>");

        StartWave();
    }

    void Update()
    {
        UpdateTimer();
    }

    /// <summary>
    /// ウェーブ開始
    /// </summary>
    void StartWave()
    {
        CurrentWaveTime = initialWaveTime;
        isTimeRunning = true;
        canSpawnEnemies = true;

        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
        }

        spawnRoutine =
            StartCoroutine(SpawnEnemyRoutine());
    }

    /// <summary>
    /// タイマー更新
    /// </summary>
    void UpdateTimer()
    {
        if (!isTimeRunning)
        {
            return;
        }

        CurrentWaveTime -= Time.deltaTime;

        if (uiManager != null)
        {
            uiManager.UpdateTimerText(
                CurrentWaveTime
            );
        }

        if (CurrentWaveTime <= 0)
        {
            CurrentWaveTime = 0;
            isTimeRunning = false;

            OnWaveTimeEnd();
        }
    }

    /// <summary>
    /// 敵スポーン処理
    /// </summary>
    IEnumerator SpawnEnemyRoutine()
    {
        while (true)
        {
            if (
                canSpawnEnemies &&
                EnemyManager.Instance != null
            )
            {
                EnemyManager.Instance.SpawnNormalEnemy();
            }

            yield return new WaitForSeconds(
                currentSpawnInterval
            );
        }
    }

    /// <summary>
    /// ウェーブ終了時処理
    /// </summary>
    void OnWaveTimeEnd()
    {
        canSpawnEnemies = false;

        ClearStage();

        StartCoroutine(
            HandleWaveCompletion()
        );
    }

    /// <summary>
    /// ウェーブクリア処理
    /// </summary>
    public IEnumerator HandleWaveCompletion()
    {
        if (uiManager != null)
        {
            yield return StartCoroutine(
                uiManager.ShowWaveCompleteText(
                    "Wave Clear!",
                    clearTextDuration
                )
            );
        }

        // ボスウェーブ判定
        if (
            CurrentWaveCount % 5 == 0 &&
            CurrentWaveCount != maxWaveCount
        )
        {
            yield return StartCoroutine(
                HandleBossSpawn()
            );

            yield break;
        }

        // 最終ウェーブ判定
        if (CurrentWaveCount >= maxWaveCount)
        {
            GameClear();
            yield break;
        }

        PrepareNextWave();

        yield return new WaitForSeconds(
            shopOpenDelay
        );

        if (ShopManager.instance != null)
        {
            ShopManager.instance.OpenShop();
        }
    }

    /// <summary>
    /// ボス出現処理
    /// </summary>
    IEnumerator HandleBossSpawn()
    {
        IsBossWave = true;

        if (uiManager != null)
        {
            yield return StartCoroutine(
                uiManager.ShowBossWarning(2.0f)
            );
        }

        if (EnemyManager.Instance != null)
        {
            EnemyManager.Instance
                .SpawnBossEnemy();
        }
    }

    /// <summary>
    /// ボス撃破時処理
    /// </summary>
    public void HandleBossDefeated()
    {
        IsBossWave = false;
        canSpawnEnemies = false;

        PrepareNextWave();

        if (CurrentWaveCount > maxWaveCount)
        {
            GameClear();
            return;
        }

        StartCoroutine(
            OpenShopAfterBoss()
        );
    }

    IEnumerator OpenShopAfterBoss()
    {
        yield return new WaitForSeconds(
            shopOpenDelay
        );

        if (ShopManager.instance != null)
        {
            ShopManager.instance.OpenShop();
        }
    }

    /// <summary>
    /// ショップ終了後
    /// </summary>
    public void ResumeAfterShop()
    {
        StartWave();
    }

    /// <summary>
    /// 次ウェーブ準備
    /// </summary>
    private void PrepareNextWave()
    {
        CurrentWaveCount++;

        if (uiManager != null)
        {
            uiManager.UpdateWaveText(
                CurrentWaveCount,
                maxWaveCount
            );
        }

        currentSpawnInterval =
            Mathf.Max(
                currentSpawnInterval -
                intervalDecreaseStep,
                minSpawnInterval
            );

        float sampleGrawth = 0.2f;
        float nextMultiplier = 1f + (CurrentWaveCount - 1) * sampleGrawth;
        Debug.Log($"<color=orange>【Waveアップ】次ウェーブは Wave {CurrentWaveCount} です。以降に湧く敵のHP倍率は 【{nextMultiplier}倍】 になります！</color>");
    }

    /// <summary>
    /// ゲームクリア
    /// </summary>
    void GameClear()
    {
        canSpawnEnemies = false;
        isTimeRunning = false;

        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
        }

        if (uiManager != null)
        {
            uiManager.ShowGameClearUI(true);
        }

        StartCoroutine(
            TransitionToResultScene()
        );
    }

    /// <summary>
    /// リザルト画面へ遷移
    /// </summary>
    IEnumerator TransitionToResultScene()
    {
        yield return new WaitForSeconds(2.0f);

        if (ScoreManager.instance != null)
        {
            ScoreManager.instance
                .SaveFinalScore();
        }

        SceneManager.LoadScene(
            "ResultScene"
        );
    }

    /// <summary>
    /// ステージ上の敵・コインを削除
    /// </summary>
    void ClearStage()
    {
        RemoveAll("Enemy");
        RemoveAll("Coin");
    }

    /// <summary>
    /// 指定タグのオブジェクトを全削除
    /// </summary>
    void RemoveAll(string tag)
    {
        GameObject[] objects =
            GameObject.FindGameObjectsWithTag(
                tag
            );

        foreach (GameObject obj in objects)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }
    }
}