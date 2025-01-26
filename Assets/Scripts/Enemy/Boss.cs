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

    private float phaseTwoTimeToSpawn = 0.5f;

    private float phaseThreeTimeToSpawn = 1.5f;

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
                        // Set parent
                        Vector3 newPos = transform.position + new Vector3(-5.0f + (i * 1.5f),
                                                                    0.0f,
                                                                    -5.0f + (i * -2.5f)); // Front of boss

                        newPos.y = 1.5f;

                        // Spawn bubble
                        GameObject newBubble = ObjectPooler.Instance.SpawnFromPool("bubbles", newPos, Quaternion.identity);

                        // Set mode
                        Bubble bubble = newBubble.GetComponent<Bubble>();

                        bubble.mode = Bubble.Mode.Tracking;
                        bubble.target = GameManager.instance.player.transform;
                        bubble.trackingSpeed = 5.0f;
                        bubble.combatHandler = combatHandler;

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

                float spinRate = 35.0f;
                
                parentObject.transform.Rotate(0, spinRate * Time.deltaTime, 0);

                if (currentTime >= phaseTwoTimeToSpawn)
                {
                    currentTime = 0.0f;

                    float p2speed = 1.0f;

                    float offsetAmount = 5.0f;

                    Vector3 positionRight = transform.position + new Vector3(offsetAmount, 0.0f, 0.0f);
                    positionRight.y = 1.5f;
                    Quaternion rotationRight = Quaternion.Euler(Vector3.up * 90.0f);
                    GameObject rightBubbleGobj = ObjectPooler.Instance.SpawnFromPool("bubbles", positionRight, rotationRight);
                    rightBubbleGobj.transform.parent = parentObject.transform;

                    Bubble rightBubble = rightBubbleGobj.GetComponent<Bubble>();
                    rightBubble.speed = p2speed;
                    rightBubble.combatHandler = combatHandler;

                    Vector3 positionLeft = transform.position + new Vector3(-offsetAmount, 0.0f, 0.0f);
                    positionLeft.y = 1.5f;
                    Quaternion rotationLeft = Quaternion.Euler(Vector3.up * 270.0f);
                    GameObject leftBubbleGobj = ObjectPooler.Instance.SpawnFromPool("bubbles", positionLeft, rotationLeft);
                    leftBubbleGobj.transform.parent = parentObject.transform;

                    Bubble leftBubble = leftBubbleGobj.GetComponent<Bubble>();
                    leftBubble.speed = p2speed;
                    leftBubble.combatHandler = combatHandler;

                    Vector3 positionForward = transform.position + new Vector3(0f, 0.0f, offsetAmount);
                    positionForward.y = 1.5f;
                    Quaternion rotationForward = Quaternion.Euler(Vector3.up * 0);
                    GameObject forwardBubbleGobj = ObjectPooler.Instance.SpawnFromPool("bubbles", positionForward, rotationForward);
                    forwardBubbleGobj.transform.parent = parentObject.transform;

                    Bubble forwardBubble = forwardBubbleGobj.GetComponent<Bubble>();
                    forwardBubble.speed = p2speed;
                    forwardBubble.combatHandler = combatHandler;

                    Vector3 positionBackward = transform.position + new Vector3(0f, 0.0f, -offsetAmount);
                    positionBackward.y = 1.5f;
                    Quaternion rotationBackward = Quaternion.Euler(Vector3.up * 180.0f);
                    GameObject backwardBubbleGobj = ObjectPooler.Instance.SpawnFromPool("bubbles", positionBackward, rotationBackward);
                    backwardBubbleGobj.transform.parent = parentObject.transform;

                    Bubble backwardBubble = backwardBubbleGobj.GetComponent<Bubble>();
                    backwardBubble.speed = p2speed;
                    backwardBubble.combatHandler = combatHandler;

                    int deleteTime = 5;

                    DespawnBubble(rightBubbleGobj, deleteTime);
                    DespawnBubble(leftBubbleGobj, deleteTime);
                    DespawnBubble(forwardBubbleGobj, deleteTime);
                    DespawnBubble(backwardBubbleGobj, deleteTime);
                }

                break;
            case Phase.Three:

                if (currentTime >= phaseThreeTimeToSpawn)
                {
                    currentTime = 0.0f;

                    int bubbleCount = 50;

                    float angleStep = 360.0f / bubbleCount;

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

                        Vector3 newPos = transform.position + (direction * spawnOffset);

                        newPos.y = 1.5f;

                        Quaternion rotation = Quaternion.LookRotation(direction);
                        GameObject bubbleGobj = ObjectPooler.Instance.SpawnFromPool("bubbles", newPos, rotation);

                        Bubble bubble = bubbleGobj.GetComponent<Bubble>();
                        bubble.speed = 2.0f;
                        bubble.combatHandler = combatHandler;

                        int deleteTime = 5;

                        DespawnBubble(bubbleGobj, deleteTime);
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

                        Vector3 newPos = transform.position + (direction * spawnOffset);

                        newPos.y = 1.5f;

                        Quaternion rotation = Quaternion.LookRotation(direction);
                        GameObject bubbleGobj = ObjectPooler.Instance.SpawnFromPool("bubbles", newPos, rotation);

                        Bubble bubble = bubbleGobj.GetComponent<Bubble>();
                        bubble.speed = 2.0f;
                        bubble.combatHandler = combatHandler;

                        int deleteTime = 5;

                        DespawnBubble(bubbleGobj, deleteTime);
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
