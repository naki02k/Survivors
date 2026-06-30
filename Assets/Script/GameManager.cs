using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ゲーム全体の進行（シーン遷移や状態管理）を管理するクラス
/// </summary>
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// シングルトンインスタンス
    /// </summary>
    public static GameManager instance { get; private set; }

    /// <summary>
    /// ゲームオーバー後の待機時間
    /// </summary>
    [SerializeField]
    private float gameOverDelay = 2f;

    /// <summary>
    /// 遷移先のリザルトシーン名
    /// </summary>
    [SerializeField]
    private string resultSceneName = "ResultScene";

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// ゲームオーバー処理を開始する
    /// </summary>
    public void HandleGameOver()
    {
        StopAllCoroutines();
        StartCoroutine(TransitionToResultRoutine());
    }

    /// <summary>
    /// リザルト画面へ遷移する
    /// </summary>
    private IEnumerator TransitionToResultRoutine()
    {
        yield return new WaitForSecondsRealtime(gameOverDelay);

        Time.timeScale = 1f;

        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.SaveFinalScore();
        }
        else
        {
            Debug.LogWarning(
                "GameManager: ScoreManagerが見つかりません。");
        }

        SceneManager.LoadScene(resultSceneName);
    }
}