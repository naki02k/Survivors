using UnityEngine;

/// <summary>
/// 戻るボタン処理
/// </summary>
public class PushReturnButton : MonoBehaviour
{
    /// <summary>
    /// タイトルパネル
    /// </summary>
    [SerializeField]
    private GameObject titlePanel;

    /// <summary>
    /// 遊び方パネル
    /// </summary>
    [SerializeField]
    private GameObject howToPlayPanel;

    /// <summary>
    /// 戻るボタン押下
    /// </summary>
    public void OnClick()
    {
        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(false);
        }

        if (titlePanel != null)
        {
            titlePanel.SetActive(true);
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (titlePanel == null)
        {
            Debug.LogWarning($"{name}: TitlePanel が設定されていません");
        }

        if (howToPlayPanel == null)
        {
            Debug.LogWarning($"{name}: HowToPlayPanel が設定されていません");
        }
    }
#endif
}