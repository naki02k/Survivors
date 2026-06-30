using UnityEngine;

/// <summary>
/// “G‚ğŒŸ’m‚µ‚Ä’e‚ğ”­Ë‚·‚éƒNƒ‰ƒX
/// </summary>
public class BulletLauncher : MonoBehaviour
{
    [SerializeField]
    private Bullet bulletPrefab;
    [SerializeField]
    private Transform muzzlePosition;

    private EnemyScanner enemyScanner;

    private void Start()
    {
        enemyScanner = GetComponent<EnemyScanner>();

        if (enemyScanner == null)
        {
            Debug.LogError("EnemyScanner not found.");
        }
    }

    /// <summary>
    /// Å‚à‹ß‚¢“G‚Ö’e‚ğ”­Ë‚·‚é
    /// </summary>
    public void Fire()
    {
        GameObject target =
            enemyScanner.ScanWithFindTag();

        if (target == null)
        {
            return;
        }

        GameObject bullet =
            Instantiate(
                bulletPrefab.gameObject,
                muzzlePosition.position,
                Quaternion.identity);

        if (bullet.TryGetComponent(out Bullet playerBullet))
        {
            playerBullet.Shoot(target.transform.position);
        }
    }
}