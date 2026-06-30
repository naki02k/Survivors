using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 斧の軌道、速度、ダメージ判定を制御するクラス
/// 放物線を描いて移動し、敵に当たるとダメージを与えて消滅する
/// </summary>
public class AxeController : MonoBehaviour
{
    [SerializeField]
    private int damage = 10;
    [SerializeField] 
    private float speed = 5f;
    [SerializeField]
    private float arcHeight = 2f;
    [SerializeField] 
    private float rotationSpeed = 360f;

    private float lifeTime;
    private float elapsedTime;

    private Vector3 startPoint;
    private Vector3 endPoint;

    /// <summary>
    /// 投擲先を設定する
    /// </summary>
    /// <param name="targetPosition">斧が向かう位置</param>
    public void Initialize(Vector3 targetPosition)
    {
        startPoint = transform.position;
        endPoint = targetPosition;
    }

    /// <summary>
    /// 斧の速度を設定し、到達時間を計算する
    /// </summary>
    public void SetSpeed(float newSpeed)
    {
        if (newSpeed <= 0f)
        {
            Debug.LogError("Speed must be greater than 0.");
            return;
        }

        speed = newSpeed;
        lifeTime = Vector3.Distance(startPoint, endPoint) / speed;
    }

    private void Update()
    {
        if (lifeTime <= 0f)
        {
            return;
        }

        elapsedTime += Time.deltaTime;

        // フレーム落ちによる値のオーバーを防ぐ
        float t = Mathf.Clamp01(elapsedTime / lifeTime);

        Vector3 linearPosition = Vector3.Lerp(startPoint, endPoint, t);

        // Sin波を利用して放物線軌道を表現
        float height = Mathf.Sin(t * Mathf.PI) * arcHeight;

        transform.position = new Vector3(
            linearPosition.x,
            linearPosition.y + height,
            linearPosition.z);

        // 投擲中であることを分かりやすくするため回転
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);

        if (t >= 1f)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 敵と接触した際にダメージを与える
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy"))
        {
            return;
        }

        if (other.TryGetComponent(out Enemy enemy))
        {
            enemy.OnDamage(damage);
        }

        Destroy(gameObject);
    }
}
