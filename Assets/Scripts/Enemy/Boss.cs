using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Boss : GameplayBehaviour
{
    public enum Phase { None, One, Two, Three, Four };

    // Set this to one when we're active
    Phase phase = Phase.None;

    private float currentTime = 0.0f;

    private float phaseOneTimeToSpawn = 3.0f;

    private float phaseTwoTimeToSpawn = 1.0f;

    private float phaseThreeTimeToSpawn = 1.0f;

    private float phaseFourTimeToSpawn = 2.0f;

    private GameObject parentObject;

    private bool parentFound = false;

    private int ringCount = 0;

    private CombatHandler combatHandler;

    private bool[] phaseTriggered = new bool[3]; // track to see if a phase has been triggered

    private bool hasBossFightStarted = false;

    void Start()
    {
        combatHandler = GetComponent<CombatHandler>();
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        float healthAsPercent = ((float)combatHandler.currentHealth / (float)combatHandler.maxHealth) * 100.0f;

        if (healthAsPercent <= 75.0f && !phaseTriggered[0])
        {
            ChangePhase(Phase.Two);

            phaseTriggered[0] = true;
        }
        else if (healthAsPercent <= 50.0f && !phaseTriggered[1])
        {
            ChangePhase(Phase.Three);

            phaseTriggered[1] = true;
        }
        else if (healthAsPercent <= 25.0f && !phaseTriggered[2])
        {
            ChangePhase(Phase.Four);

            phaseTriggered[2] = true;
        }

        currentTime += Time.deltaTime;

        // Boss logic here
        switch (phase)
        {
            case Phase.None: // Nada
                break;
            case Phase.One:

                if (currentTime >= phaseOneTimeToSpawn)
                {
                    currentTime = 0.0f;

                    int amount = Random.Range(3, 5);

                    for (int i = 0; i < amount; ++i)
                    {
                        // Spawn bubble
                        GameObject newBubble = ObjectPooler.Instance.SpawnFromPool("bubbles", Vector3.zero, Quaternion.identity);

                        // Set parent
                        newBubble.transform.position = transform.position + new Vector3(-5.0f + (i * 1.5f),
                                                                    0.0f,
                                                                    -5.0f + (i * -2.5f)); // Front of boss

                        // Set mode
                        Bubble bubble = newBubble.GetComponent<Bubble>();

                        bubble.mode = Bubble.Mode.Tracking;
                        bubble.target = GameManager.instance.player.transform;
                        bubble.trackingSpeed = 5.0f;

                        int randomDeleteTime = Random.Range(4, 8);

                        DespawnBubble(newBubble, randomDeleteTime);
                    }
                }

                break;
            case Phase.Two:
                if (!parentFound)
                {
                    if (parentObject == null)
                    {
                        parentObject = new GameObject("Parent Object");
                        parentObject.transform.position = transform.position;
                        parentFound = true;
                    }
                }

                float spinRate = 25.0f;
                
                parentObject.transform.Rotate(0, spinRate * Time.deltaTime, 0);

                if (currentTime >= phaseTwoTimeToSpawn)
                {
                    currentTime = 0.0f;

                    Vector3 positionRight = transform.position + new Vector3(2.0f, 0.0f, 0.0f);
                    Quaternion rotationRight = Quaternion.Euler(Vector3.up * 90.0f);
                    GameObject rightBubble = ObjectPooler.Instance.SpawnFromPool("bubbles", positionRight, rotationRight);
                    rightBubble.transform.parent = parentObject.transform;

                    Vector3 positionLeft = transform.position + new Vector3(-2.0f, 0.0f, 0.0f);
                    Quaternion rotationLeft = Quaternion.Euler(Vector3.up * 270.0f);
                    GameObject leftBubble = ObjectPooler.Instance.SpawnFromPool("bubbles", positionLeft, rotationLeft);
                    leftBubble.transform.parent = parentObject.transform;

                    Vector3 positionForward = transform.position + new Vector3(0f, 0.0f, 2.0f);
                    Quaternion rotationForward = Quaternion.Euler(Vector3.up * 0);
                    GameObject forwardBubble = ObjectPooler.Instance.SpawnFromPool("bubbles", positionForward, rotationForward);
                    forwardBubble.transform.parent = parentObject.transform;

                    Vector3 positionBackward = transform.position + new Vector3(0f, 0.0f, -2.0f);
                    Quaternion rotationBackward = Quaternion.Euler(Vector3.up * 180.0f);
                    GameObject backwardBubble = ObjectPooler.Instance.SpawnFromPool("bubbles", positionBackward, rotationBackward);
                    backwardBubble.transform.parent = parentObject.transform;

                    int deleteTime = 8;

                    DespawnBubble(rightBubble, deleteTime);
                    DespawnBubble(leftBubble, deleteTime);
                    DespawnBubble(forwardBubble, deleteTime);
                    DespawnBubble(backwardBubble, deleteTime);
                }

                break;
            case Phase.Three:

                if (currentTime >= phaseThreeTimeToSpawn)
                {
                    currentTime = 0.0f;

                    int bubbleCount = 50;

                    float angleStep = 360.0f / bubbleCount;

                    int shiftOffset = ringCount % 10;

                    int amountOfBubblesToSkip = 5;

                    bool flipPattern = (ringCount % 2 == 1);

                    for (int i = 0; i < bubbleCount; ++i)
                    {
                        bool isGap = ((i % 10) < amountOfBubblesToSkip);

                        if (flipPattern)
                        {
                            isGap = !isGap;
                        }
                        
                        if (isGap)
                        {
                            continue;
                        }

                        float angle = i * angleStep;

                        float spawnOffset = 6.0f;

                        Vector3 direction = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0.0f, Mathf.Sin(angle * Mathf.Deg2Rad));

                        Quaternion rotation = Quaternion.LookRotation(direction);
                        GameObject bubble = ObjectPooler.Instance.SpawnFromPool("bubbles", transform.position + (direction * spawnOffset), rotation);
                        bubble.GetComponent<Bubble>().speed = 1.0f;

                        int deleteTime = 8;

                        DespawnBubble(bubble, deleteTime);
                    }

                    ringCount++;

                }

                break;
            case Phase.Four:

                if (currentTime >= phaseFourTimeToSpawn)
                {
                    currentTime = 0.0f;

                    int bubbleCount = 50;

                    float angleStep = 360.0f / bubbleCount;

                    for (int i = 0; i < bubbleCount; ++i)
                    {
                        float angle = i * angleStep;

                        float spawnOffset = 6.0f;

                        Vector3 direction = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0.0f, Mathf.Sin(angle * Mathf.Deg2Rad));

                        Quaternion rotation = Quaternion.LookRotation(direction);
                        GameObject bubble = ObjectPooler.Instance.SpawnFromPool("bubbles", transform.position + (direction * spawnOffset), rotation);
                        bubble.GetComponent<Bubble>().speed = 1.0f;

                        int deleteTime = 8;

                        DespawnBubble(bubble, deleteTime);
                    }
                }

                break;
        }
    }

    void DespawnBubble(GameObject bubble, float despawnTime)
    {
        Timer time = bubble.AddComponent<Timer>();
        time.duration = despawnTime;
        time.startAction = Timer.StartAction.OnlyInvokeEvent;
        time.onFinishedInternal += bubble.GetComponent<Bubble>().Despawn;
        time.EnableTimer();
    }

    private void ChangePhase(Phase phase)
    {
        this.phase = phase;
    }

    public void StartBossCombat()
    {
        if (!hasBossFightStarted)
        {
            hasBossFightStarted = true;

            phase = Phase.One;
        }
    }

    public void EndBossCombat()
    {
        if (hasBossFightStarted)
        {
            phase = Phase.None;
        }

        // Shortly after, we need to call the end game screen
    }
}
