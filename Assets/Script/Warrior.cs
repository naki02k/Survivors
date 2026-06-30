using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// プレイヤーに追従し、敵を攻撃するウォーリアの行動を制御するクラス
/// </summary>
public class Warrior : MonoBehaviour
{
    /// <summary>
    /// 追跡するプレイヤー
    /// </summary>
    private Transform player;

    /// <summary>
    /// 投げ斧プレハブ
    /// </summary>
    [SerializeField]
    private GameObject axePrefab;

    /// <summary>
    /// プレイヤー追従距離
    /// </summary>
    [SerializeField]
    private float followDistance = 3.0f;

    /// <summary>
    /// 敵検知範囲
    /// </summary>
    [SerializeField]
    private float detectRange = 10.0f;

    /// <summary>
    /// 攻撃範囲
    /// </summary>
    [SerializeField]
    private float attackRange = 2.0f;

    /// <summary>
    /// 攻撃クールダウン
    /// </summary>
    [SerializeField]
    private float attackCooldown = 1.0f;

    /// <summary>
    /// ナビゲーション制御
    /// </summary>
    private NavMeshAgent2D agent;

    /// <summary>
    /// 現在のターゲット
    /// </summary>
    private Transform currentTarget;

    /// <summary>
    /// 最後に攻撃した時間
    /// </summary>
    private float lastAttackTime;

    /// <summary>
    /// アニメーション制御
    /// </summary>
    private Animator animator;

    /// <summary>
    /// 移動中か
    /// </summary>
    private bool isMoving;

    /// <summary>
    /// プレイヤーを設定
    /// </summary>
    public void SetPlayerTransform(Transform playerTransform)
    {
        player = playerTransform;
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (player == null)
        {
            return;
        }

        // 敵がいない時はプレイヤーを追従し、プレイヤーの方を向く
        if (currentTarget == null)
        {
            FollowPlayer();
            DetectEnemies();
            FacePlayer();
        }
        // 敵がいる時は敵を追跡・攻撃し、敵の方を向く
        else
        {
            ChaseAttackEnemy();
            FaceEnemy();
        }
    }

    /// <summary>
    /// プレイヤー追従
    /// </summary>
    void FollowPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position,player.position);

        if (distanceToPlayer > followDistance + 0.5f)
        {
            agent.SetDestination(player.position);

            if (!isMoving)
            {
                isMoving = true;
                animator.SetBool("isFollowing",true);
            }
        }
        else if (distanceToPlayer <= followDistance)
        {
            agent.SetDestination(transform.position);

            if (isMoving)
            {
                isMoving = false;
                animator.SetBool("isFollowing",false);
            }
        }
    }

    /// <summary>
    /// 敵検知
    /// </summary>
    void DetectEnemies()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position,detectRange);

        foreach (Collider2D hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy") || hitCollider.CompareTag("Boss"))
            {
                currentTarget = hitCollider.transform;
                break;
            }
        }
    }

    /// <summary>
    /// 敵追跡・攻撃
    /// </summary>
    void ChaseAttackEnemy()
    {
        if (currentTarget == null)
        {
            return;
        }

        float distanceToEnemy =Vector3.Distance(transform.position,currentTarget.position);

        if (distanceToEnemy <= attackRange)
        {
            AttackEnemy();
        }
        else
        {
            agent.SetDestination(currentTarget.position);

            animator.SetBool("isFollowing",true);
        }
    }

    /// <summary>
    /// 攻撃処理
    /// </summary>
    void AttackEnemy()
    {
        if (Time.time - lastAttackTime < attackCooldown)
        {
            return;
        }

        lastAttackTime = Time.time;

        agent.SetDestination(transform.position);

        animator.SetBool("isFollowing",false);

        ThrowAxe();
    }

    /// <summary>
    /// 斧を投げる
    /// </summary>
    void ThrowAxe()
    {
        if (axePrefab == null ||
            currentTarget == null)
        {
            return;
        }

        GameObject axe =Instantiate(axePrefab,transform.position,Quaternion.identity);

        AxeController axeController =axe.GetComponent<AxeController>();

        if (axeController != null)
        {
            axeController.Initialize(currentTarget.position);
        }
    }

    /// <summary>
    /// プレイヤーの方向を向く
    /// </summary>
    void FacePlayer()
    {
        if (player == null)
        {
            return;
        }

        if (player.position.x > transform.position.x)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (player.position.x < transform.position.x)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }

    /// <summary>
    /// 敵の方向を向く
    /// </summary>
    void FaceEnemy()
    {
        if (currentTarget == null)
        {
            return;
        }

        if (currentTarget.position.x > transform.position.x)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (currentTarget.position.x <transform.position.x)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }

    /// <summary>
    /// 検知範囲表示
    /// </summary>
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position,detectRange);
    }
}