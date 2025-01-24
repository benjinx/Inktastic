using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyEyes : MonoBehaviour
{
    public EnemyStateMachine esm;
    public LayerMask targetMask;
    public LayerMask obstructionMask;
    public float radius;
    public float angle;
    public float checkInterval = .1f;
    public GameObject eyePointer;
    public PlayerStateMachine activeTarget;
    public Vector3 lastSeenPos;
    public bool canSeeCurrentTarget;



    private float currentCheckTime;
    private Vector3 ogEyePos;

    public float distanceToTarget;


    public void Awake()
    {
        //esm = GetComponent<EnemyStateMachine>();
        //ogEyePos = eyePointer.transform.position;
    }

    public void Update()
    {
       
        currentCheckTime += Time.deltaTime;

        if (currentCheckTime >= checkInterval)
        {
            FieldOfViewCheck();
            currentCheckTime = 0;
        }

        if (activeTarget != null)
        {
            distanceToTarget = Vector3.Distance(esm.transform.position, activeTarget.transform.position);

            //if (activeTarget.GetComponent<TargetObject>() != null && eyePointer != null)
            {
                //eyePointer.transform.position = activeTarget.GetComponent<TargetObject>().aimPosition.position;
            }
            if (activeTarget.GetCurrentState() == activeTarget.deathState)
            {
                activeTarget = null;
            }
        }
    }

    public void Spook(PlayerStateMachine _actor)
    {
        if(esm == null)
        {
            return;
        }

        activeTarget = _actor;
        esm.OnSpook();
        //bark to other enemies
    }

    public void ResetEyes()
    {
        activeTarget = null;
        //eyePointer.transform.position = ogEyePos;
    }

    private void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

        if (rangeChecks.Length != 0)
        {

            for (int i = 0; i < rangeChecks.Length; i++)
            {
                if (rangeChecks[i].GetComponent<PlayerStateMachine>() != null)
                {
                    PlayerStateMachine target = rangeChecks[i].GetComponent<PlayerStateMachine>();

                   /* if (target.teamIndex == esm.teamIndex)
                    {
                        //aight we cool
                        continue;
                    }*/

                    Vector3 directionToTarget = (target.transform.position - transform.position).normalized;

                    if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
                    {
                        float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);


                        if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                        {
                            if (activeTarget == null && target != null)
                            {
                                Spook(target);
                                //esm.agroState.Bark();
                            }
                            if (target == activeTarget)
                            {
                                canSeeCurrentTarget = true;
                            }

                          

                        }
                        else
                        {
                            //canSeePlayer = false;
                            //_target.visible = false;
                            //GetComponent<BaseEnemyStateMachine>().ClearTarget();

                            //if we previously saw a player we need to investigate
                            if (activeTarget != null && canSeeCurrentTarget == true)
                            {
                                lastSeenPos = activeTarget.transform.position;
                            }
                            canSeeCurrentTarget = false;
                        }
                    }
                    else
                    {
                        //_target.visible = false;
                        //GetComponent<BaseEnemyStateMachine>().ClearTarget();
                        //canSeePlayer = false;
                        //he long gone or behind us uwu
                        if (activeTarget != null && canSeeCurrentTarget == true)
                        {
                            lastSeenPos = activeTarget.transform.position;
                        }
                        canSeeCurrentTarget = false;
                    }
                }


            }

        }

    }
}
