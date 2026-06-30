using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// ショップアイテムのUI表示と購入ボタンの設定を行うクラス
/// </summary>
public class ShopItemUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField]
    private TextMeshProUGUI itemNameText;

    [SerializeField]
    private TextMeshProUGUI itemPriceText;

    [SerializeField]
    private Image itemIconImage;

    [SerializeField]
    private Button buyButton;

    /// <summary>
    /// 現在表示しているアイテム
    /// </summary>
    private ShopItem currentShopItem;

    /// <summary>
    /// アイテム情報をUIへ反映し、購入ボタンを設定する
    /// </summary>
    /// <param name="item">表示するアイテムデータ</param>
    /// <param name="onBuyAction">購入時に実行する処理</param>
    public void Setup(ShopItem item, UnityAction onBuyAction)
    {
        if (item == null)
        {
            Debug.LogWarning("ShopItem が設定されていません。");
            return;
        }

        currentShopItem = item;

        if (itemNameText != null)
        {
            itemNameText.text = item.itemName;
        }

        if (itemPriceText != null)
        {
            itemPriceText.text = $"{item.price} Coins";
        }

        if (itemIconImage != null)
        {
            itemIconImage.sprite = item.itemIcon;
        }

        if (buyButton != null)
        {
            buyButton.onClick.RemoveAllListeners();

            if (onBuyAction != null)
            {
                buyButton.onClick.AddListener(onBuyAction);
            }
        }
    }
}