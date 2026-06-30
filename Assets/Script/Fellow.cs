using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーと一定の距離を保ちながら追従する仲間キャラクター
/// </summary>
public class Fellow : MonoBehaviour
{
    /// <summary>
    /// 追従対象のプレイヤー
    /// </summary>
    [SerializeField]
    private Transform player;

    /// <summary>
    /// プレイヤーとの維持距離
    /// </summary>
    [SerializeField]
    private float followDistance = 3f;

    /// <summary>
    /// ナビゲーション制御
    /// </summary>
    private NavMeshAgent2D agent;

    /// <summary>
    /// アニメーション制御
    /// </summary>
    private Animator animator;

    /// <summary>
    /// 移動中フラグ
    /// </summary>
    private bool isMoving;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent2D>();
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// プレイヤーを設定
    /// </summary>
    public void SetPlayerTransform(Transform playerTransform)
    {
        player = playerTransform;
    }

    private void Update()
    {
        if (player == null)
        {
            return;
        }

        // 常にプレイヤーを追従し、向きを合わせる
        FollowPlayer();
        FacePlayer();
    }

    /// <summary>
    /// プレイヤーを追従
    /// </summary>
    private void FollowPlayer()
    {
        float distanceToPlayer =
            Vector3.Distance(transform.position, player.position);

        // プレイヤーから一定距離以上離れたら近づく
        if (distanceToPlayer > followDistance + 0.5f)
        {
            agent.SetDestination(player.position);

            if (!isMoving)
            {
                isMoving = true;
                animator.SetBool("isFollowing", true);
            }
        }
        // プレイヤーに十分に近づいたら停止する
        else if (distanceToPlayer <= followDistance)
        {
            agent.SetDestination(transform.position);

            if (isMoving)
            {
                isMoving = false;
                animator.SetBool("isFollowing", false);
            }
        }
    }

    /// <summary>
    /// プレイヤーの方向を向く
    /// </summary>
    private void FacePlayer()
    {
        if (player.position.x > transform.position.x)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }
}