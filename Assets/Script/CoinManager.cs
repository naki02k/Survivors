using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ゲーム内のコインを管理するクラス
/// </summary>
public class CoinManager : MonoBehaviour
{
    public static CoinManager instance;

    [SerializeField]
    private TextMeshProUGUI coinText;

    private int coinCount;

    public int CoinCount => coinCount;

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

    private void Start()
    {
        UpdateCoinUI();
    }

    public void AddCoin()
    {
        coinCount++;
        UpdateCoinUI();
    }

    public bool SpendCoins(int amount)
    {
        if (amount <= 0)
        {
            return false;
        }

        if (coinCount < amount)
        {
            return false;
        }

        coinCount -= amount;

        UpdateCoinUI();

        return true;
    }

    public int GetCoinCount()
    {
        return coinCount;
    }

    private void UpdateCoinUI()
    {
        if (coinText != null)
        {
            coinText.text = $"Coins: {coinCount}";
        }
    }
}