using UnityEngine;

public abstract class CharacterBase : MonoBehaviour
{
    public CharacterStatsData stats;
    protected int currentHealth;

    protected virtual void Start()
    {
        currentHealth = stats.maxHealth;
    }

    public virtual void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"{stats.characterName} の残りHP: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected abstract void Die();
}
