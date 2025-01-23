using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatHandler : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;
    public LayerMask hitMask;
    public float invincibleTimer = .1f;
    public int teamIndex;
    public float flashColorDuration;

    public SpriteRenderer actorRenderer;

    private List<Hitbox> hitboxes = new List<Hitbox>();
    private bool colorFlashing;
    private Color originalEmissionColor;

    private void Awake()
    {
        InitializeHitboxes();

        originalEmissionColor = actorRenderer.material.GetColor("_Color");
    }

    public void TakeDamage(float _damage)
    {
        currentHealth -= _damage;
        FlashMaterial(actorRenderer.material, Color.red, flashColorDuration);
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

    void FlashMaterial(Material material, Color _color, float _duration)
    {
        // Check if the material has an emission property
        if (material != null)
        {
            ResetMaterial(material, originalEmissionColor);
            //Debug.Log("flashing!");
            // Save the original emission color

            // Set the emission color to red
            //material.SetColor("_Color", Color.red * 10f);
            // Use DOTween to animate the color back to the original over a short duration
            material.DOColor(_color, _duration).OnComplete(() => ResetMaterial(material, originalEmissionColor));
        }
    }
    // Reset the material to its original state
    void ResetMaterial(Material material, Color color)
    {
        // Check if the material has an emission property
        if (material != null && material.HasProperty("_Color"))
        {
            // Reset the emission color to its original value
            material.DOColor(color, flashColorDuration);
        }

        colorFlashing = false;
    }
}
