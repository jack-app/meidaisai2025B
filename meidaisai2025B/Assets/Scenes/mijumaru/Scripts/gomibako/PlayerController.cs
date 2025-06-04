using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlayerController : CharacterBase
{
    protected override void Die()
    {
        Debug.Log($"{stats.characterName} が死亡しました（プレイヤー）");
        // ゲームオーバー処理など
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemy"))
        {
            EnemyController enemy = collision.collider.GetComponent<EnemyController>();
            if (enemy != null)
            {
                TakeDamage(enemy.stats.attackPower);
            }
        }
    }
}
