using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ボスのHP、移動、攻撃、死亡時の挙動を管理するクラス
/// </summary>
public class Boss : MonoBehaviour
{
    private Player player;

    [SerializeField]
    private float speed = 1f;
    [SerializeField] 
    private float attackRange = 2f;
    [SerializeField]
    private float rangedAttackCooldown = 5f;
    [SerializeField]
    private float attackInterval = 1f;

    private float rangedAttackTimer;
    private float attackTimer;

    [SerializeField] private int hp;

    [SerializeField] 
    private GameObject explosionEffectPrefab;
    [SerializeField] 
    private GameObject rangedAttackEffectPrefab;
    [SerializeField] 
    private Collider2D attackHitbox;
    [SerializeField] 
    private GameObject childObject;
    [SerializeField] 
    private AudioClip[] attackSounds;
    [SerializeField] 
    private GameObject damagePopupPrefab;

    private Animator animator;
    private AudioSource audioSource;

    private bool isDead;

    private void Start()
    {
        player = Player.instance;

        if (player == null)
        {
            Debug.LogError("Player が見つかりません。");
            enabled = false;
            return;
        }

        animator = childObject.GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (attackHitbox != null)
        {
            attackHitbox.enabled = false;
        }
    }

    private void Update()
    {
        if (isDead)
        {
            return;
        }

        Vector3 playerPos = player.transform.position;

        float distanceToPlayer = Vector2.Distance(transform.position, playerPos);

        attackTimer += Time.deltaTime;

        if (distanceToPlayer <= attackRange)
        {
            if (attackTimer >= attackInterval)
            {
                attackTimer = 0f;
                StartCoroutine(PerformMeleeAttack());
            }
        }
        else
        {
            // 移動処理
            transform.position = Vector2.MoveTowards(
                transform.position,
                playerPos,
                speed * Time.deltaTime);

            // 向きの変更
            Vector3 diff = playerPos - transform.position;
            if (diff.x > 0)
            {
                transform.eulerAngles = new Vector3(0, 180, 0);
            }
            else if (diff.x < 0)
            {
                transform.eulerAngles = Vector3.zero;
            }
        }

        // 遠距離攻撃のタイマー処理
        rangedAttackTimer += Time.deltaTime;
        if (rangedAttackTimer >= rangedAttackCooldown)
        {
            rangedAttackTimer = 0f;
            StartCoroutine(PerformRangedAttack(playerPos));
        }
    }

    private IEnumerator PerformRangedAttack(Vector3 playerPos)
    {
        animator.SetTrigger("cast");

        yield return new WaitForSeconds(0.5f);

        PlayAttackSound(1);

        if (rangedAttackEffectPrefab == null)
        {
            yield break;
        }

        int numberOfAttacks = Random.Range(3, 5);

        for (int i = 0; i < numberOfAttacks; i++)
        {
            Vector3 randomOffset =
                new Vector3(
                    Random.Range(-2f, 2f),
                    Random.Range(-2f, 2f),
                    0f);

            Vector3 attackPosition = playerPos + randomOffset;

            GameObject effect =
                Instantiate(
                    rangedAttackEffectPrefab,
                    attackPosition,
                    Quaternion.identity);

            Destroy(effect, 2f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead)
        {
            return;
        }

        if (other.CompareTag("Attack") ||
            other.CompareTag("FellowAttack") ||
            other.CompareTag("Runetracer"))
        {
            int randomDamage = Random.Range(1, 6);

            TakeDamage(randomDamage);
        }
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;

        ShowDamagePopup(damage);

        hp = Mathf.Max(hp, 0);

        animator.SetTrigger("TakeDamage");

        if (hp == 0 && !isDead)
        {
            isDead = true;

            animator.SetTrigger("death");

            ScoreManager.instance.AddScore(1000);
            ScoreManager.instance.IncrementBossCount();

            StartCoroutine(HandleDeath());
        }
    }

    private void ShowDamagePopup(int damage)
    {
        if (damagePopupPrefab == null)
        {
            return;
        }

        Vector3 popupPosition =
            transform.position + new Vector3(0f, 0.5f, 0f);

        GameObject popup =
            Instantiate(
                damagePopupPrefab,
                popupPosition,
                Quaternion.identity);

        if (popup.TryGetComponent(out DamagePopup damagePopup))
        {
            damagePopup.Setup(damage);
        }
    }

    private IEnumerator HandleDeath()
    {
        yield return new WaitForSeconds(2f);

        Destroy(gameObject);

        if (WaveManager.Instance != null &&
            WaveManager.Instance.CurrentWaveCount < WaveManager.Instance.MaxWaveCount &&
            ShopManager.instance != null)
        {
            ShopManager.instance.OpenShop();
        }
    }

    private IEnumerator PerformMeleeAttack()
    {
        animator.SetTrigger("Attack");

        PlayAttackSound(0);

        if (attackHitbox != null)
        {
            attackHitbox.enabled = true;
        }

        yield return new WaitForSeconds(0.5f);

        if (attackHitbox != null)
        {
            attackHitbox.enabled = false;
        }
    }

    private void PlayAttackSound(int index)
    {
        if (attackSounds == null ||
            audioSource == null ||
            index < 0 ||
            index >= attackSounds.Length)
        {
            return;
        }

        audioSource.PlayOneShot(attackSounds[index]);
    }
}