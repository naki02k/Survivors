using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーを追従するカメラ
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private Transform player;

    private void LateUpdate()
    {
        if (player == null)
        {
            return;
        }

        transform.position = new Vector3(
            player.position.x,
            player.position.y,
            transform.position.z);
    }
}