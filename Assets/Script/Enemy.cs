using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敵キャラクターの振る舞い（移動、HP、ダメージ、死亡）を制御するクラス
/// </summary>
public class Enemy : MonoBehaviour
{
    Player player;

    Vector3 playerPos;

    [SerializeField]
    float speed = 1f;

    [SerializeField]
    int hp = 10;

    [Header("Wave Scaling Settings")]
    [Tooltip("1ウェーブごとに上昇するHPの割合 (0.2 = 20%ずつ上昇)")]
    [SerializeField]
    float hpGrowthPerWave = 0.2f;

    [SerializeField]
    CoinDropper coinDropper;

    [SerializeField]
    GameObject explosionEffectPrefab;

    [SerializeField]
    GameObject damagePopupPrefab;

    Animator animator;

    bool isDead = false;

    Collider2D enemyCollider;

    void Start()
    {
        if (isDead) return;

        AdjustHpByWave();

        if (Player.instance != null)
        {
            player = Player.instance;
        }
        else
        {
            Debug.LogError("Player instance が見つかりません");
            return;
        }

        animator = GetComponent<Animator>();
        enemyCollider = GetComponent<Collider2D>();

        IntervalTime timer = GetComponent<IntervalTime>();

        if (timer != null)
        {
            timer.enabled = true;
        }
    }

    /// <summary>
    /// ウェーブ数に応じてHPを調整する
    /// </summary>
    void AdjustHpByWave()
    {
        if (WaveManager.Instance == null) return;

        int currentWave = WaveManager.Instance.CurrentWaveCount;

        float multiplier = 1f + (currentWave - 1) * hpGrowthPerWave;
        hp = Mathf.RoundToInt(hp * multiplier);

        Debug.Log($"<color=green>[スポーン] {gameObject.name} (Wave {currentWave}): 現在のHPは 【{hp}】 です (ベースの {multiplier}倍)</color>");
    }

    void Update()
    {
        if (player == null || isDead) return;

        playerPos = player.transform.position;

        transform.position = Vector2.MoveTowards(
            transform.position,
            playerPos,
            speed * Time.deltaTime);

        float diffX = playerPos.x - transform.position.x;

        if (diffX > 0)
        {
            transform.eulerAngles = Vector3.zero;
        }
        else if (diffX < 0)
        {
            transform.eulerAngles = new Vector3(0, -180, 0);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead) return;

        if (other.CompareTag("Attack") ||
            other.CompareTag("FellowAttack") ||
            other.CompareTag("Runetracer"))
        {
            int randomDamage = Random.Range(1, 6);

            TakeDamage(randomDamage);

            StartCoroutine(DisableColliderTemporarily(0.5f));
        }
    }

    /// <summary>
    /// 外部からダメージを与える
    /// </summary>
    public void OnDamage(int damage)
    {
        TakeDamage(damage);

        StartCoroutine(DisableColliderTemporarily(0.5f));
    }

    /// <summary>
    /// ダメージ処理
    /// </summary>
    void TakeDamage(int damage)
    {
        animator.SetTrigger("hit");

        hp -= damage;

        ShowDamagePopup(damage);

        if (hp <= 0 && !isDead)
        {
            isDead = true;

            ScoreManager.instance.AddScore(100);
            ScoreManager.instance.IncrementEnemyCount();

            animator.SetTrigger("death");

            EnemyManager.Instance.EnemyDefeated();

            StartCoroutine(HandleDeath());
        }
    }

    void ShowDamagePopup(int damage)
    {
        if (damagePopupPrefab == null) return;

        Vector3 popupPosition =
            transform.position + new Vector3(0, 0.5f, 0);

        GameObject popup =
            Instantiate(damagePopupPrefab,
            popupPosition,
            Quaternion.identity);

        DamagePopup damagePopup =
            popup.GetComponent<DamagePopup>();

        if (damagePopup != null)
        {
            damagePopup.Setup(damage);
        }
    }

    IEnumerator DisableColliderTemporarily(float duration)
    {
        if (enemyCollider == null)
        {
            yield break;
        }

        enemyCollider.enabled = false;

        yield return new WaitForSeconds(duration);

        if (!isDead)
        {
            enemyCollider.enabled = true;
        }
    }

    IEnumerator HandleDeath()
    {
        yield return new WaitForSeconds(0.5f);

        if (coinDropper != null)
        {
            coinDropper.DropCoins(transform.position);
        }

        Destroy(gameObject);
    }
}