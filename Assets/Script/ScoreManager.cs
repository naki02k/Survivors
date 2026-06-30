using UnityEngine;

/// <summary>
/// ゲームのスコアと統計情報を管理するクラス
/// </summary>
public class ScoreManager : MonoBehaviour
{
    /// <summary>
    /// シングルトンインスタンス
    /// </summary>
    public static ScoreManager instance { get; private set; }

    /// <summary>
    /// 現在のスコア
    /// </summary>
    private int score = 0;

    /// <summary>
    /// 最終スコア
    /// </summary>
    public static int finalScore = 0;

    /// <summary>
    /// 倒した敵の総数
    /// </summary>
    public static int enemiesDefeated = 0;

    /// <summary>
    /// 倒したボスの総数
    /// </summary>
    public static int bossesDefeated = 0;

    /// <summary>
    /// 集めたコインの総数
    /// </summary>
    public static int coinsCollected = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// スコアを加算する
    /// </summary>
    /// <param name="amount">加算するスコア</param>
    public void AddScore(int amount)
    {
        score += amount;
    }

    /// <summary>
    /// コイン獲得数を加算する
    /// </summary>
    public void AddCoin()
    {
        coinsCollected++;
    }

    /// <summary>
    /// 現在スコアを最終スコアとして保存する
    /// </summary>
    public void SaveFinalScore()
    {
        finalScore = score;
    }

    /// <summary>
    /// 現在スコアを取得する
    /// </summary>
    public int GetScore() => score;

    /// <summary>
    /// 現在のコイン獲得数を取得する
    /// </summary>
    public int GetCoin() => coinsCollected;

    /// <summary>
    /// 倒した敵の数を加算する
    /// </summary>
    public void IncrementEnemyCount()
    {
        enemiesDefeated++;
    }

    /// <summary>
    /// 倒したボスの数を加算する
    /// </summary>
    public void IncrementBossCount()
    {
        bossesDefeated++;
    }
}