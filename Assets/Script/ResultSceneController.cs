using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// リザルト画面の表示を管理するクラス
/// </summary>
public class ResultSceneController : MonoBehaviour
{
    /// <summary>
    /// スコア表示
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI scoreText;

    /// <summary>
    /// 敵撃破数表示
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI enemiesText;

    /// <summary>
    /// ボス撃破数表示
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI bossesText;

    /// <summary>
    /// コイン取得数表示
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI coinText;

    /// <summary>
    /// ステージ名表示
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI stageNameText;

    private void Start()
    {
        UpdateResultUI();
    }

    /// <summary>
    /// リザルト情報をUIへ反映
    /// </summary>
    private void UpdateResultUI()
    {
        if (scoreText != null)
        {
            scoreText.text =
                $"Score : {ScoreManager.finalScore}";
        }

        if (enemiesText != null)
        {
            enemiesText.text =
                $"Enemies Defeated : {ScoreManager.enemiesDefeated}";
        }

        if (bossesText != null)
        {
            bossesText.text =
                $"Bosses Defeated : {ScoreManager.bossesDefeated}";
        }

        if (coinText != null)
        {
            coinText.text =
                $"Coins Collected : {ScoreManager.coinsCollected}";
        }

        if (stageNameText != null)
        {
            stageNameText.text =
                $"Stage : {ModeSelector.CurrentStage}";
        }
    }
}