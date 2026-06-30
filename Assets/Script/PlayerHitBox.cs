using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitBox : MonoBehaviour
{
    [SerializeField]
    private PlayerHPBar playerHPBar;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("ê⁄êG : " + collision.name);

        if (collision.CompareTag("Enemy")||collision.CompareTag("EnemyAttack"))
        {
            Debug.Log("EnemyÇ…ê⁄êG");
            playerHPBar.ApplyDamage(Random.Range(1,6));
        }
        else if(collision.CompareTag("Boss")||collision.CompareTag("BossAttack"))
        {
            playerHPBar.ApplyDamage(Random.Range(3, 9));
        }
        else if(collision.CompareTag("DamageArea"))
        {
            playerHPBar.ApplyDamage(5);
        }
    }
}
