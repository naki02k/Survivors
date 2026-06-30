using System.Collections;
using UnityEngine;

/// <summary>
/// ’eŠÛ‚ÌˆÚ“®‚ÆÕ“Ë‚ğ§Œä‚·‚éƒNƒ‰ƒX
/// </summary>
public class Bullet : MonoBehaviour
{
    [SerializeField] 
    private float speed = 20f;
    [SerializeField] 
    private float lifeTime = 5f;

    private bool canMove;
    private Vector3 direction;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        if (!canMove)
        {
            return;
        }

        transform.Translate(
            direction.normalized *
            speed *
            Time.deltaTime,
            Space.World);
    }

    /// <summary>
    /// ’eŠÛ‚ğw’è•ûŒü‚Ö”­Ë‚·‚é
    /// </summary>
    public void Shoot(Vector3 target)
    {
        direction = target - transform.position;
        canMove = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") ||other.CompareTag("Boss"))
        {
            Destroy(gameObject);
        }
    }
}