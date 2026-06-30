using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ルーントレーサー（武器）の動きと衝突時の挙動を制御するクラス
/// オブジェクトの反射とダメージ処理を行う
/// </summary>
public class RunetracerController : MonoBehaviour
{
    /// <summary>
    /// ルーントレーサーの移動速度
    /// </summary>
    [SerializeField]
    private float speed = 15f;

    /// <summary>
    /// 与えるダメージ量
    /// </summary>
    [SerializeField]
    private int damage = 5;

    private Rigidbody2D rigidBody2D;
    private Vector2 lastVelocity;

    private void Start()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();

        if (rigidBody2D == null)
        {
            Debug.LogError(
                $"{name}: Rigidbody2D が見つかりません。");
            enabled = false;
            return;
        }

        // 初期方向をランダムに決定
        float randomAngle = Random.Range(0f, 360f);
        Vector2 direction = new Vector2(
            Mathf.Cos(randomAngle * Mathf.Deg2Rad),
            Mathf.Sin(randomAngle * Mathf.Deg2Rad));

        rigidBody2D.velocity = direction.normalized * speed;
    }

    void Update()
    {
        // 衝突する直前の正確な速度を毎フレーム保存しておく
        if(rigidBody2D.velocity.sqrMagnitude>0.1f)
        {
            lastVelocity = rigidBody2D.velocity;
        }
    }

    /// <summary>
    /// 他のオブジェクトと衝突したときの処理
    /// </summary>
    /// <param name="other">衝突情報</param>
    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("Wall"))
        {
            // 衝突面の方戦を取得
            Vector2 normal = other.contacts[0].normal;

            // 衝突直前の速度を使って反射ベクトルを計算
            Vector2 reflectDir = Vector2.Reflect(lastVelocity.normalized, normal);

            // 角スタック防止
            float angleOffset = Random.Range(-10f, 10f) * Mathf.Deg2Rad;
            float cos = Mathf.Cos(angleOffset);
            float sin = Mathf.Sin(angleOffset);
            reflectDir = new Vector2(
                reflectDir.x * cos - reflectDir.y * sin,
                reflectDir.x * sin + reflectDir.y * cos
                );

            // 速度を再設定
            rigidBody2D.velocity = reflectDir.normalized * speed;
        }

        if(other.gameObject.TryGetComponent(out Enemy enemy))
        {
            enemy.OnDamage(damage);
        }

        if(other.gameObject.TryGetComponent(out Boss boss))
        {
            boss.TakeDamage(damage);
        }
    }

    /// <summary>
    /// 設定されているダメージ量を取得する
    /// </summary>
    public int GetDamage() => damage;
}