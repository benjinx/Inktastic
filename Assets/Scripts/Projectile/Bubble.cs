using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(Rigidbody))]
public class Bubble : GameplayBehaviour
{
    // Default is just a standard bullet
    // Tracking will track the target
    public enum Mode { Default, Tracking };

    [HideInInspector]
    public Mode mode = Mode.Default;

    public float speed = 0.2f;

    public LayerMask despawnMask;
    public float despawnCheckDistance;

    public float bubbleWabbleVarience = 0.5f;

    public float trackingSpeed = 2.0f;

    private Rigidbody rb;

    public Transform target;

    public CombatHandler combatHandler;
    public float bubbleDamage;
    public float hackyBubbleAttackTime;

    [HideInInspector]
    public bool dumbInitializeHack = false;

    private void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = false;
        dumbInitializeHack = false;

    }

    public void BubbleFX()
    {
        MagesAudioManager.Instance.PlayClip("BubbleLaunch");
    }



    protected override void OnFixedUpdate()
    {
        base.OnFixedUpdate();

        if (dumbInitializeHack)
        {
            if (combatHandler != null)
            {
                foreach (Hitbox hitbox in GetComponentsInChildren<Hitbox>())
                {
                    hitbox.InitializeHitbox(combatHandler);
                    hitbox.ActivateHitbox(hackyBubbleAttackTime, bubbleDamage);
                }

            }


            dumbInitializeHack =false;
        }

        RaycastHit hit;

        if(gameObject.activeSelf && Physics.Raycast(this.transform.position, this.transform.forward, out hit, despawnCheckDistance, despawnMask))
        {
            Despawn();
        }

        switch (mode)
        {
            case Mode.Default:

                // Add force for "bullet"
                rb.AddForce(transform.forward * speed);

                // Add drift
                rb.AddForce(new Vector3(
                    Random.Range(-bubbleWabbleVarience, bubbleWabbleVarience),
                    0,
                    Random.Range(-bubbleWabbleVarience, bubbleWabbleVarience))); // random drift

                break;
            case Mode.Tracking:

                Vector3 direction = (target.position - transform.position).normalized;

                rb.AddForce(direction * trackingSpeed, ForceMode.Force);
                break;
        }
    }

    public void Despawn()
    {
        gameObject.SetActive(false);
        rb.ResetInertiaTensor();
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
        mode = Mode.Default;
        MagesAudioManager.Instance.PlayClip("BubblePop");

        foreach (Hitbox hitbox in GetComponentsInChildren<Hitbox>())
        {
            hitbox.StopHitbox();
        }
    }
}
