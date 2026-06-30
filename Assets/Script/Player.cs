using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// プレイヤーキャラクターの動き、攻撃、ステータスを管理するクラス
/// </summary>
public class Player : MonoBehaviour
{
    /// <summary>
    /// シングルトンインスタンス
    /// </summary>
    public static Player instance { get; private set; }

    [Header("Settings")]

    /// <summary>
    /// プレイヤーの体力
    /// </summary>
    [SerializeField]
    private int hp;

    /// <summary>
    /// プレイヤーの移動速度
    /// </summary>
    [SerializeField]
    private float moveSpeed = 0.02f;

    [Header("Attack")]

    /// <summary>
    /// プレイヤーの攻撃力
    /// </summary>
    [SerializeField]
    private int attack = 1;

    /// <summary>
    /// 攻撃間隔
    /// </summary>
    [SerializeField]
    private float attackInterval = 2.0f;

    /// <summary>
    /// 攻撃判定の中心となるTransform
    /// </summary>
    [SerializeField]
    private Transform attackPoint;

    /// <summary>
    /// 攻撃が届く範囲の半径
    /// </summary>
    [SerializeField]
    private float attackRadius;

    /// <summary>
    /// 攻撃対象のレイヤー
    /// </summary>
    [SerializeField]
    private LayerMask enemyLayer;

    [Header("Prefabs")]

    /// <summary>
    /// 斬撃エフェクトのプレハブ
    /// </summary>
    [SerializeField]
    private GameObject slashEffectPrefab;

    /// <summary>
    /// 攻撃力アップ時のエフェクト
    /// </summary>
    [SerializeField]
    private GameObject attackEffectPrefab;

    /// <summary>
    /// 移動速度アップ時のエフェクト
    /// </summary>
    [SerializeField]
    private GameObject speedEffectPrefab;

    [Header("References")]

    /// <summary>
    /// アニメーション制御用
    /// </summary>
    private Animator animator;

    /// <summary>
    /// 効果音再生用
    /// </summary>
    private AudioSource[] audioSources;

    /// <summary>
    /// 現在の攻撃力
    /// </summary>
    public int AttackPower => attack;

    /// <summary>
    /// 現在の移動速度
    /// </summary>
    public float CurrentMoveSpeed => moveSpeed;

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
        animator = GetComponentInChildren<Animator>();
        audioSources = GetComponents<AudioSource>();

        StartCoroutine(AutoAttackCoroutine());
    }

    private void Update()
    {
        HandleMovement();
    }

    /// <summary>
    /// プレイヤーの移動処理
    /// </summary>
    private void HandleMovement()
    {
        if (Time.timeScale == 0)
        {
            return;
        }

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector2 inputDir = new Vector2(h, v).normalized;

        if (inputDir.magnitude > 0)
        {
            transform.position +=
                (Vector3)inputDir * moveSpeed * Time.deltaTime;

            if (h != 0)
            {
                transform.localScale =
                    new Vector3(Mathf.Sign(h), 1, 1);
            }

            animator.SetFloat("speed", 1);
        }
        else
        {
            animator.SetFloat("speed", 0);
        }
    }

    /// <summary>
    /// 一定間隔で自動攻撃を行う
    /// </summary>
    private IEnumerator AutoAttackCoroutine()
    {
        WaitForSeconds wait = new WaitForSeconds(attackInterval);

        while (true)
        {
            bool isShopOpen =
                ShopManager.instance != null &&
                ShopManager.instance.IsShopOpen;

            if (!isShopOpen && Time.timeScale != 0)
            {
                Attack();
            }

            yield return wait;
        }
    }

    /// <summary>
    /// 攻撃処理
    /// </summary>
    private void Attack()
    {
        if (slashEffectPrefab == null)
        {
            return;
        }

        PlaySound(0);

        int finalDamage = attack + Random.Range(1, 6);

        GameObject slash =
            Instantiate(
                slashEffectPrefab,
                attackPoint.position,
                Quaternion.identity,
                transform);

        if (slash.TryGetComponent(out SlashEffect slashEffect))
        {
            slashEffect.Initialize(
                finalDamage,
                enemyLayer,
                attackRadius);
        }

        Destroy(slash, 1.0f);
    }

    /// <summary>
    /// 攻撃力を増加させる
    /// </summary>
    public void IncreaseAttack(int amount)
    {
        attack += amount;

        SpawnEffect(attackEffectPrefab);
        PlaySound(1);
    }

    /// <summary>
    /// 移動速度を増加させる
    /// </summary>
    public void IncreaseSpeed(float amount)
    {
        moveSpeed += amount;

        SpawnEffect(speedEffectPrefab);
        PlaySound(1);
    }

    /// <summary>
    /// ステータス上昇エフェクトを生成
    /// </summary>
    private void SpawnEffect(GameObject effectPrefab)
    {
        if (effectPrefab == null)
        {
            return;
        }

        GameObject effect =
            Instantiate(
                effectPrefab,
                transform.position,
                Quaternion.identity);

        Destroy(effect, 2.0f);
    }

    /// <summary>
    /// 攻撃範囲を可視化
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
        {
            return;
        }

        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            attackPoint.position,
            attackRadius);
    }

    /// <summary>
    /// 効果音を再生する
    /// </summary>
    /// <param name="index">再生するAudioSourceのインデックス</param>
    private void PlaySound(int index)
    {
        if (audioSources != null &&
            index >= 0 &&
            index < audioSources.Length)
        {
            audioSources[index].Play();
        }
    }
}