using System.Collections.Generic;
using UnityEngine;

public class Boss : GameplayBehaviour
{
    enum Phase { None, One, Two, Three, Four };

    // Set this to one when we're active
    Phase phase = Phase.None;

    private ObjectPooler pooler;

    public List<GameObject> activeBubbles = new List<GameObject>();

    private bool spawned = false;

    private float currentTime = 0.0f;

    private float phaseOneTimeToSpawn = 3.0f;

    private float phaseTwoTimeToSpawn = 1.0f;

    private float spinTime = 0.0f;

    public Transform player;

    private GameObject parentObject;

    private bool parentFound = false;

    void Start()
    {
        pooler = GetComponent<ObjectPooler>();

        phase = Phase.Two;
    }

    void Update()
    {
        
        // Boss logic here
        switch (phase)
        {
            case Phase.None: // Nada
                break;
            case Phase.One:

                currentTime += Time.deltaTime;

                if (currentTime >= phaseOneTimeToSpawn)
                {
                    currentTime = 0.0f;

                    int amount = Random.Range(3, 5);

                    for (int i = 0; i < amount; ++i)
                    {
                        // Spawn bubble
                        GameObject newBubble = pooler.SpawnFromPool("bubbles", Vector3.zero, Quaternion.identity);

                        // Set parent
                        newBubble.transform.position = new Vector3(-22.0f + (i * 1.5f),
                                                                    0.0f,
                                                                    5.0f + (i * -2.5f)); // Front of boss

                        // Set mode
                        Bubble bubble = newBubble.GetComponent<Bubble>();

                        bubble.mode = Bubble.Mode.Tracking;
                        bubble.target = player;

                        // Add parent so we can track them as a group
                        activeBubbles.Add(newBubble);


                        int randomDeleteTime = Random.Range(2, 5);

                        Invoke(nameof(DespawnBubble), randomDeleteTime);
                    }
                }

                break;
            case Phase.Two:
                // Get proper parent, should only need to do it once
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

                spinTime += Time.deltaTime;

                if (spinTime >= 10.0f)
                {
                    spinTime = 0.0f;

                    spinRate *= -1;
                }
                
                parentObject.transform.Rotate(0, spinRate * Time.deltaTime, 0);

                // After x amount of time, reverse the spin

                currentTime += Time.deltaTime;

                if (currentTime >= phaseTwoTimeToSpawn)
                {
                    currentTime = 0.0f;

                    Vector3 positionRight = transform.position + new Vector3(2.0f, 0.0f, 0.0f);
                    Quaternion rotationRight = Quaternion.Euler(Vector3.up * 90.0f);
                    GameObject rightBubble = pooler.SpawnFromPool("bubbles", positionRight, rotationRight);
                    rightBubble.transform.parent = parentObject.transform;

                    Vector3 positionLeft = transform.position + new Vector3(-2.0f, 0.0f, 0.0f);
                    Quaternion rotationLeft = Quaternion.Euler(Vector3.up * 270.0f);
                    GameObject leftBubble = pooler.SpawnFromPool("bubbles", positionLeft, rotationLeft);
                    leftBubble.transform.parent = parentObject.transform;

                    Vector3 positionForward = transform.position + new Vector3(0f, 0.0f, 2.0f);
                    Quaternion rotationForward = Quaternion.Euler(Vector3.up * 0);
                    GameObject forwardBubble = pooler.SpawnFromPool("bubbles", positionForward, rotationForward);
                    forwardBubble.transform.parent = parentObject.transform;

                    Vector3 positionBackward = transform.position + new Vector3(0f, 0.0f, -2.0f);
                    Quaternion rotationBackward = Quaternion.Euler(Vector3.up * 180.0f);
                    GameObject backwardBubble = pooler.SpawnFromPool("bubbles", positionBackward, rotationBackward);
                    backwardBubble.transform.parent = parentObject.transform;

                    activeBubbles.Add(rightBubble);
                    activeBubbles.Add(leftBubble);
                    activeBubbles.Add(forwardBubble);
                    activeBubbles.Add(backwardBubble);

                    int deleteTime = 8;

                    Invoke(nameof(DespawnBubble), deleteTime);
                    Invoke(nameof(DespawnBubble), deleteTime);
                    Invoke(nameof(DespawnBubble), deleteTime);
                    Invoke(nameof(DespawnBubble), deleteTime);
                }

                break;
            case Phase.Three:
                // Shoot projectiles that create pathways in the negative space that the player must weave through
                // - Request amounts of balls and we need to do a flip flop pattern example:
                // ---   ---   ---   ---
                //    ---   ---   ---
                break;
            case Phase.Four:
                // Emit porjectiles in circular shockwaves that player must dash through with I-frames, example:
                // -------
                //        
                // -------
                //        
                // Similar to P2 just more and more delay in them
                break;
        }
    }

    void DespawnBubble()
    {
        // Valid to delete
        if (activeBubbles.Count > 0)
        {
            // Reparent if need be
            if (activeBubbles[0].transform.parent != transform)
            {
                activeBubbles[0].transform.parent = transform;
            }

            // Just remove the first
            activeBubbles[0].GetComponent<Bubble>().Despawn();
            activeBubbles.Remove(activeBubbles[0]);
        }
    }
}
