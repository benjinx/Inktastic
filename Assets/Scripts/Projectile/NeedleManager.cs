
using System.Collections.Generic;
using UnityEngine;

public class NeedleManager : MonoBehaviour
{
    public List<Needle> needles = new List<Needle>();

    private int currentHits = 0;

    void Start()
    {
        GetComponent<CombatHandler>().hitSuccess += HandleHitCounter;


        //GameplayStates.ChangePlayerAmmo(ammoBinding.RawValue, ammoBinding.Denominator);
    }

    void Update()
    {
        
    }

    public void ShootProjectile()
    {
        float angle = gameObject.GetComponent<PlayerStateMachine>().controllerState.currentLookAngle;

        // Somethings goin on with the angle
        Debug.Log(angle);

        Vector3 direction = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0.0f, Mathf.Sin(angle * Mathf.Deg2Rad));

        foreach (Needle needle in needles)
        {
            if (needle.isAvailable)
            {
                needle.ShootSpike(direction);

                // Return so we only shoot the first
                return;
            }
        }
    }

    public void HandleHitCounter()
    {
        // between 3-5 hits = turn on isAvailable for a needle that isn't
        currentHits++;

        if (currentHits >= Random.Range(3, 6)) // 3-5 hits
        {
            foreach (Needle needle in needles)
            {
                if (!needle.isAvailable)
                {
                    // We could try to just reset 1 instead of all, but for now let's just do all
                    needle.ResetSpike();
                }
            }
        }
    }
}
