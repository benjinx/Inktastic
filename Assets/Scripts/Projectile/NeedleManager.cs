
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
        PlayerStateMachine psm = GetComponent<PlayerStateMachine>();

        Vector3 direction = psm.controllerState.pointerSprite.transform.forward;

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

                    currentHits = 0;
                }
            }
        }
    }
}
