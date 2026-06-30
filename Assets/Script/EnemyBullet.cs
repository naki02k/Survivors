using UnityEngine;

/// <summary>
/// “G‚ª”­Ë‚·‚é’eŠÛ‚ÌˆÚ“®‚ÆÕ“Ë”»’è‚ğŠÇ—‚·‚éƒNƒ‰ƒX
/// </summary>
public class EnemyBullet : MonoBehaviour
{
    /// <summary>
    /// ’eŠÛ‚ÌˆÚ“®‘¬“x
    /// </summary>
    [SerializeField]
    private float speed = 20f;

    /// <summary>
    /// ’eŠÛ‚Ìõ–½
    /// </summary>
    [SerializeField]
    private float lifeTime = 5f;

    /// <summary>
    /// ”­ËÏ‚İ‚©‚Ç‚¤‚©
    /// </summary>
    private bool isLaunched;

    /// <summary>
    /// ˆÚ“®•ûŒü
    /// </summary>
    private Vector3 direction;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void FixedUpdate()
    {
        if (!isLaunched)
        {
            return;
        }

        transform.Translate(
            direction.normalized *
            speed *
            Time.fixedDeltaTime);
    }

    /// <summary>
    /// w’è‚µ‚½ˆÊ’u‚ÖŒü‚¯‚Ä’e‚ğ”­Ë‚·‚é
    /// </summary>
    public void Shot(Vector3 targetPosition)
    {
        direction = targetPosition - transform.position;
        isLaunched = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}