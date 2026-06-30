using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ゲーム内のショップ機能を管理するクラス。
/// アイテムの表示、購入、リロール、および購入した効果の適用を制御します
/// </summary>
public class ShopManager : MonoBehaviour
{
    /// <summary>
    /// シングルトンインスタンス
    /// </summary>
    public static ShopManager instance { get; private set; }

    #region UI

    [Header("UI Objects")]

    /// <summary>
    /// ショップUI
    /// </summary>
    [SerializeField]
    private GameObject shopUi;

    /// <summary>
    /// メインUI
    /// </summary>
    [SerializeField]
    private GameObject mainUi;

    /// <summary>
    /// カウントダウン表示テキスト
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI countdownText;

    /// <summary>
    /// アイテム配置先
    /// </summary>
    [SerializeField]
    private Transform itemParent;

    /// <summary>
    /// ショップアイテムUIプレハブ
    /// </summary>
    [SerializeField]
    private GameObject itemPrefab;

    #endregion

    #region Buttons

    [SerializeField]
    private Button closeButton;

    [SerializeField]
    private Button rerollButton;

    [SerializeField]
    private TextMeshProUGUI rerollPriceText;

    #endregion

    #region Shop Settings

    /// <summary>
    /// 出現可能な全ショップアイテム
    /// </summary>
    [SerializeField]
    private List<ShopItem> allItems;

    /// <summary>
    /// 現在のリロール価格
    /// </summary>
    [SerializeField]
    private int rerollPrice = 5;

    /// <summary>
    /// リロール価格上限
    /// </summary>
    [SerializeField]
    private int maxRerollPrice = 20;

    #endregion

    #region Summon

    [SerializeField]
    private Transform playerTransform;

    [SerializeField]
    private GameObject wizardPrefab;

    [SerializeField]
    private GameObject warriorPrefab;

    [SerializeField]
    private Transform summonLocation;

    #endregion

    #region Managers

    [SerializeField]
    private RunetracerManager runetracerManager;

    [SerializeField]
    private MagicBookManager magicBookManager;

    [SerializeField]
    private IntervalTime bulletIntervalTime;

    #endregion

    #region Player

    [SerializeField]
    private PlayerHPBar playerHpBar;

    [SerializeField]
    private Player player;

    #endregion

    #region Runtime Data

    /// <summary>
    /// 現在ショップに並んでいるアイテム
    /// </summary>
    private List<ShopItem> currentItems =
        new List<ShopItem>();

    /// <summary>
    /// HP回復アイテム購入数
    /// </summary>
    private int healItemCount;

    /// <summary>
    /// 攻撃力アップ購入数
    /// </summary>
    private int attackBoostCount;

    /// <summary>
    /// 移動速度アップ購入数
    /// </summary>
    private int speedBoostCount;

    #endregion

    /// <summary>
    /// ショップが開いているか
    /// </summary>
    public bool IsShopOpen { get; private set; }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        rerollButton.onClick.AddListener(RerollItems);
        closeButton.onClick.AddListener(CloseShop);

        if (shopUi != null)
        {
            shopUi.SetActive(false);
        }
    }

    /// <summary>
    /// ショップを開く
    /// </summary>
    public void OpenShop()
    {
        if (mainUi != null)
        {
            mainUi.SetActive(false);
        }

        if (shopUi != null)
        {
            shopUi.SetActive(true);
        }

        IsShopOpen = true;

        GenerateRandomItems();
        UpdateRerollPriceDisplay();
    }

    /// <summary>
    /// ショップを閉じる
    /// </summary>
    public void CloseShop()
    {
        IsShopOpen = false;

        if (mainUi != null)
        {
            mainUi.SetActive(true);
        }

        if (shopUi != null)
        {
            shopUi.SetActive(false);
        }

        ApplyPurchasedEffects();

        StartCoroutine(StartCountDownAndResumeSpawning());
    }

    /// <summary>
    /// カウントダウン後に戦闘を再開する
    /// </summary>
    IEnumerator StartCountDownAndResumeSpawning()
    {
        countdownText.gameObject.SetActive(true);

        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

        countdownText.text = "Go!";
        yield return new WaitForSeconds(1f);

        countdownText.gameObject.SetActive(false);

        if (WaveManager.Instance != null)
        {
            WaveManager.Instance.ResumeAfterShop();
        }
    }

    /// <summary>
    /// ランダムに3つのアイテムを生成する
    /// </summary>
    void GenerateRandomItems()
    {
        ClearItems();

        List<ShopItem> randomItems = new List<ShopItem>();
        List<ShopItem> itemPool = new List<ShopItem>(allItems);

        for (int i = 0; i < 3 && itemPool.Count > 0; i++)
        {
            int randomIndex = Random.Range(0, itemPool.Count);

            randomItems.Add(itemPool[randomIndex]);
            itemPool.RemoveAt(randomIndex);
        }

        currentItems = randomItems;

        foreach (ShopItem item in currentItems)
        {
            GameObject itemObject =
                Instantiate(itemPrefab, itemParent);

            ShopItemUI uiScript =
                itemObject.GetComponent<ShopItemUI>();

            if (uiScript != null)
            {
                uiScript.Setup(item, () => BuyItem(item, itemObject));
            }
        }
    }

    /// <summary>
    /// アイテム購入処理
    /// </summary>
    void BuyItem(ShopItem item,GameObject itemObject)
    {
        if (!CoinManager.instance.SpendCoins(item.price))
        {
            Debug.LogWarning(
                $"コインが不足しています。必要コイン数 : {item.price}");
            return;
        }

        Debug.Log($"{item.itemName} を購入しました。");

        if (itemObject != null) Destroy(itemObject);
        if (currentItems.Contains(item)) currentItems.Remove(item);

        switch (item.effectType)
        {
            case ItemEffectType.Heal:
                healItemCount++;
                break;

            case ItemEffectType.Attack:
                attackBoostCount++;
                break;

            case ItemEffectType.Speed:
                speedBoostCount++;
                break;

            case ItemEffectType.Wizard:
                SummonFollow(wizardPrefab);
                break;

            case ItemEffectType.Warrior:
                SummonFollow(warriorPrefab);
                break;

            case ItemEffectType.Runetracer:
                SpawnRunetracer();
                break;

            case ItemEffectType.MagicBook:
                SpawnMagicBook();
                break;

            case ItemEffectType.Bullet:
                SpawnBullet();
                break;

            default:
                Debug.LogWarning("未対応のアイテムタイプです。");
                break;
        }
    }

    /// <summary>
    /// 仲間を召喚する
    /// </summary>
    void SummonFollow(GameObject prefab)
    {
        if (prefab == null ||
            summonLocation == null)
        {
            return;
        }

        GameObject fellowObject =
            Instantiate(prefab,summonLocation.position,summonLocation.rotation);

        if (fellowObject.TryGetComponent(out Fellow fellow))
        {
            if (playerTransform != null)
            {
                fellow.SetPlayerTransform(playerTransform);
            }
        }
        else if(fellowObject.TryGetComponent(out Warrior warrior))
        {
            if(playerTransform!=null)
            {
                warrior.SetPlayerTransform(playerTransform);
            }
        }
    }

    /// <summary>
    /// Runetracerを生成する
    /// </summary>
    void SpawnRunetracer()
    {
        if (runetracerManager == null)
        {
            Debug.LogWarning(
                "RunetracerManagerが設定されていません。");
            return;
        }

        runetracerManager.SpawnRunetracer();
    }

    /// <summary>
    /// MagicBookを生成する
    /// </summary>
    void SpawnMagicBook()
    {
        if (magicBookManager != null)
        {
            magicBookManager.SpawnAndRotate();
        }
    }

    /// <summary>
    /// 弾幕武器を有効化する
    /// </summary>
    void SpawnBullet()
    {
        // インスペクターの未設定対策
        if (bulletIntervalTime == null)
        {
            Debug.LogWarning("bulletIntervalTime (PlayerのIntervalTime) が設定されていません。");
            return;
        }

        // 直接 enabled の切り替えやメソッドの実行を行う
        if (bulletIntervalTime.enabled)
        {
            bulletIntervalTime.ReduceInterval(0.5f);
        }
        else
        {
            bulletIntervalTime.enabled = true;
            Debug.Log("Bulletのスクリプトを有効化しました！");
        }
    }

    /// <summary>
    /// リロール価格表示を更新する
    /// </summary>
    void UpdateRerollPriceDisplay()
    {
        if (rerollPriceText != null)
        {
            rerollPriceText.text = rerollPrice.ToString();
        }
    }

    /// <summary>
    /// アイテムをリロールする
    /// </summary>
    public void RerollItems()
    {
        if (!CoinManager.instance.SpendCoins(rerollPrice))
        {
            Debug.LogWarning("コインが足りません。");
            return;
        }

        GenerateRandomItems();

        rerollPrice =
            Mathf.Min(
                rerollPrice + 5,
                maxRerollPrice);

        UpdateRerollPriceDisplay();
    }

    /// <summary>
    /// 購入した効果を適用する
    /// </summary>
    private void ApplyPurchasedEffects()
    {
        if (healItemCount > 0)
        {
            int totalHeal = 20 * healItemCount;

            playerHpBar.Heal(totalHeal);

            healItemCount = 0;
        }

        if (attackBoostCount > 0)
        {
            player.IncreaseAttack(
                2 * attackBoostCount);

            attackBoostCount = 0;
        }

        if (speedBoostCount > 0)
        {
            player.IncreaseSpeed(
                0.01f * speedBoostCount);

            speedBoostCount = 0;
        }
    }

    /// <summary>
    /// アイテムUIを全削除する
    /// </summary>
    void ClearItems()
    {
        foreach (Transform child in itemParent)
        {
            Destroy(child.gameObject);
        }
    }
}
