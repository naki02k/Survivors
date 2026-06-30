using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// アニメーションイベント受信用クラス
/// </summary>
public class PlayerAnimationReceiver : MonoBehaviour
{
    /// <summary>
    /// 親のPlayer参照
    /// </summary>
    private Player parentPlayer;

    private void Start()
    {
        parentPlayer = GetComponentInParent<Player>();

        if (parentPlayer == null)
        {
            Debug.LogWarning(
                "PlayerAnimationReceiver : Playerが見つかりません。");
        }
    }

    /// <summary>
    /// 攻撃アニメーション終了イベント
    /// </summary>
    public void OnAttackAnimationEnd()
    {
        if (parentPlayer == null)
        {
            return;
        }

        // 将来必要になったら呼び出す
        // parentPlayer.OnAttackAnimationEnd();
    }
}