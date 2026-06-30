using UnityEngine;

/// <summary>
/// プレイヤーの特殊攻撃によって生成される
/// 斬撃エフェクトの処理を担うクラス
/// </summary>
public class SlashEffect : MonoBehaviour
{
    /// <summary>
    /// ダメージ量
    /// </summary>
    private int damageAmount;

    /// <summary>
    /// 敵レイヤー
    /// </summary>
    private LayerMask enemyLayer;

    /// <summary>
    /// 攻撃範囲
    /// </summary>
    private float attackRadius;

    /// <summary>
    /// デフォルト攻撃半径
    /// </summary>
    [SerializeField]
    private float defaultAttackRadius = 0.5f;

    /// <summary>
    /// ダメージ情報を初期化
    /// </summary>
    public void Initialize(
        int damage,
        LayerMask layer,
        float radius)
    {
        damageAmount = damage;
        enemyLayer = layer;
        attackRadius = radius;

        ApplyDamage();
    }

    void Start()
    {
        if (attackRadius <= 0)
        {
            attackRadius = defaultAttackRadius;
        }
    }

    /// <summary>
    /// 範囲内の敵へダメージを与える
    /// </summary>
    void ApplyDamage()
    {
        Collider2D[] hitEnemies =
            Physics2D.OverlapCircleAll(
                transform.position,
                attackRadius,
                enemyLayer);

        foreach (Collider2D hitEnemy in hitEnemies)
        {
            if (hitEnemy.TryGetComponent(out Boss boss))
            {
                boss.TakeDamage(damageAmount);
                continue;
            }

            hitEnemy
                .GetComponent<Enemy>()
                ?.OnDamage(damageAmount);
        }
    }

    /// <summary>
    /// 攻撃範囲の可視化
    /// </summary>
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            transform.position,
            attackRadius > 0
                ? attackRadius
                : defaultAttackRadius);
    }
}