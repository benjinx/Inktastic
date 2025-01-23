using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Splines;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

public class Hitbox : MonoBehaviour
{
    public bool attacking;
    public SplineContainer movementSpline;


    public List<CombatHandler> alreadyHit = new List<CombatHandler>();

    public BoxCollider boxCollider;

    private bool isActive;
    private float damage;
    private float duration;
    private float currentDuration;
    private Vector3 moveOverride;
    [HideInInspector]
    public CombatHandler combat;
    private Vector3 ogPosition;

    public UnityEvent onAttackStart;
    public UnityEvent onAttackEnd;


    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        ogPosition = transform.localPosition;
    }

    public void InitializeHitbox(CombatHandler _combat)
    {
        combat = _combat;
    }

    public void ActivateHitbox(float _duration, float _damage)
    {
        currentDuration = 0;
        duration = _duration;
        damage = _damage;
        isActive = true;
        alreadyHit.Clear();
        this.transform.localPosition = ogPosition;
        onAttackStart?.Invoke();
    }


    public void Update()
    {
        if (isActive)
        {
            currentDuration += Time.deltaTime;

            if (movementSpline != null)
            {
                moveOverride = movementSpline.EvaluatePosition(currentDuration / duration);
                this.transform.position = moveOverride;
            }

            RaycastHit[] hits = Physics.BoxCastAll(this.transform.position, boxCollider.bounds.extents / 2, Vector3.forward, Quaternion.identity, boxCollider.bounds.extents.x, ~combat.hitMask);

            for (int i = 0; i < hits.Length; i++)
            {
                Vector3 hitPos = hits[i].point;

                if (hits[i].collider.GetComponent<Hitbox>() != null)
                {
                    Hitbox hitbox = hits[i].collider.GetComponent<Hitbox>();

                    if (!alreadyHit.Contains(hitbox.combat) && hitbox.combat != this.combat)
                    {
                        //take damage
                        //add to already hit
                        hitbox.combat.TakeDamage(damage);
                    }
                }
            }

            if(currentDuration >= duration)
            {
                currentDuration = 0;
                isActive = false;
                this.transform.localPosition = ogPosition;
                onAttackEnd?.Invoke();
            }
        }
    }

    void OnDrawGizmos()
    {
        if (isActive)
        {
            Gizmos.color = new Color(1, 0, 0, 1f);

            //Gizmos.DrawSphere(this.transform.position, sphereCollider.bounds.extents.x);
            Gizmos.DrawCube(this.transform.position + boxCollider.center, boxCollider.bounds.extents * 2);
            Gizmos.color = Color.red;
        }
        else
        {
            if (attacking)
            {

                Gizmos.color = new Color(1, 0, 0, .1f);
                Gizmos.DrawCube(this.transform.position + boxCollider.center, boxCollider.bounds.extents * 2);
            }
            else
            {
                Gizmos.color = new Color(0, 1, 0, .1f);

                Gizmos.DrawCube(this.transform.position + boxCollider.center, boxCollider.bounds.extents * 2);
            }
        }


    }
}
