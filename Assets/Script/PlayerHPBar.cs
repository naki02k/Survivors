using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// プレイヤーのHP管理クラス
/// </summary>
public class PlayerHPBar : MonoBehaviour
{
    /// <summary>
    /// 最大HP
    /// </summary>
    [SerializeField]
    private int maxHp = 75;

    /// <summary>
    /// 現在HP
    /// </summary>
    private int currentHp;

    /// <summary>
    /// HPスライダー
    /// </summary>
    [SerializeField]
    private Slider slider;

    /// <summary>
    /// ゲームオーバーUI
    /// </summary>
    [SerializeField]
    private GameObject gameOverUi;

    /// <summary>
    /// ダメージ表示プレハブ
    /// </summary>
    [SerializeField]
    private GameObject damageTextPrefab;

    /// <summary>
    /// ダメージ表示位置
    /// </summary>
    [SerializeField]
    private Transform damageTextPosition;

    /// <summary>
    /// ダメージテキストの親Canvas
    /// </summary>
    [SerializeField]
    private Canvas canvas;

    /// <summary>
    /// 回復エフェクト
    /// </summary>
    [SerializeField]
    private GameObject healEffectPrefab;

    /// <summary>
    /// 回復SE
    /// </summary>
    [SerializeField]
    private AudioClip soundClip;

    /// <summary>
    /// AudioSource
    /// </summary>
    private AudioSource audioSource;

    private void Start()
    {
        currentHp = maxHp;

        if (slider != null)
        {
            slider.maxValue = maxHp;
            slider.value = currentHp;
        }

        if (gameOverUi != null)
        {
            gameOverUi.SetActive(false);
        }

        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// HP回復
    /// </summary>
    public void Heal(int amount)
    {
        currentHp = Mathf.Min(currentHp + amount, maxHp);

        UpdateHPBar();

        Debug.Log($"HPを{amount}回復しました。現在HP:{currentHp}");

        TriggerHealEffect();
    }

    /// <summary>
    /// 回復エフェクト表示
    /// </summary>
    private void TriggerHealEffect()
    {
        if (healEffectPrefab != null)
        {
            GameObject effect =
                Instantiate(
                    healEffectPrefab,
                    transform.position,
                    Quaternion.identity);

            Destroy(effect, 2f);
        }

        PlaySound();
    }

    /// <summary>
    /// HPバー更新
    /// </summary>
    private void UpdateHPBar()
    {
        if (slider != null)
        {
            slider.value = currentHp;
        }
    }

    /// <summary>
    /// ダメージ適用
    /// </summary>
    public void ApplyDamage(int damage)
    {
        currentHp -= damage;

        if (currentHp < 0)
        {
            currentHp = 0;
        }

        UpdateHPBar();
        ShowDamage(damage);

        Debug.Log($"現在HP : {currentHp}");

        if (currentHp <= 0)
        {
            GameOver();
        }
    }

    /// <summary>
    /// ゲームオーバー処理
    /// </summary>
    private void GameOver()
    {
        if (gameOverUi != null)
        {
            gameOverUi.SetActive(true);
        }

        Time.timeScale = 0;

        if (GameManager.instance != null)
        {
            GameManager.instance.HandleGameOver();
        }

        gameObject.SetActive(false);
    }

    /// <summary>
    /// ダメージ表示
    /// </summary>
    private void ShowDamage(int damage)
    {
        if (damageTextPrefab == null ||
            damageTextPosition == null ||
            canvas == null)
        {
            return;
        }

        GameObject damageTextObj =
            Instantiate(
                damageTextPrefab,
                damageTextPosition.position,
                Quaternion.identity);

        damageTextObj.transform.SetParent(
            canvas.transform,
            false);

        Text damageText =
            damageTextObj.GetComponent<Text>();

        if (damageText != null)
        {
            damageText.text = damage.ToString();
        }

        RectTransform rectTransform =
            damageTextObj.GetComponent<RectTransform>();

        if (rectTransform != null)
        {
            rectTransform.anchoredPosition =
                new Vector2(0, 100);
        }

        StartCoroutine(FadeOutDamageText(damageTextObj));
    }

    /// <summary>
    /// ダメージテキストのフェードアウト
    /// </summary>
    private IEnumerator FadeOutDamageText(
        GameObject damageTextObj)
    {
        Text damageText =
            damageTextObj.GetComponent<Text>();

        RectTransform rectTransform =
            damageTextObj.GetComponent<RectTransform>();

        if (damageText == null ||
            rectTransform == null)
        {
            yield break;
        }

        Color color = damageText.color;

        float fadeDuration = 1f;
        float moveSpeed = 50f;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;

            color.a =
                Mathf.Lerp(
                    1f,
                    0f,
                    elapsedTime / fadeDuration);

            damageText.color = color;

            rectTransform.anchoredPosition +=
                new Vector2(
                    0,
                    moveSpeed * Time.deltaTime);

            yield return null;
        }

        Destroy(damageTextObj);
    }

    /// <summary>
    /// 回復SE再生
    /// </summary>
    private void PlaySound()
    {
        if (soundClip != null &&
            audioSource != null)
        {
            audioSource.PlayOneShot(soundClip);
        }
    }
}