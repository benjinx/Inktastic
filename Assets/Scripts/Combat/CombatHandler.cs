using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatHandler : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;
    public LayerMask hitMask;
    public float invincibleTimer = .1f;

    public SpriteRenderer actorRenderer;

    private List<Hitbox> hitboxes = new List<Hitbox>();

    private void Awake()
    {
        InitializeHitboxes();
    }

    public void TakeDamage(float _damage)
    {
        currentHealth -= _damage;

        //call either enemy or player graphics handler and do temp hit
    }

    public void Die()
    {

    }

    public void InitializeHitboxes()
    {
        foreach (Hitbox hitbox in GetComponentsInChildren<Hitbox>())
        {
            hitbox.InitializeHitbox(this);
        }
    }
}
