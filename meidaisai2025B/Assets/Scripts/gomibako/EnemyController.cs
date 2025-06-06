using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EnemyController : CharacterBase
{
    protected override void Die()
    {
        Debug.Log($"{stats.characterName} が死亡しました（敵）");
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            PlayerController player = collision.collider.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(stats.attackPower);
            }
        }
    }
}
