using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    enum Phase { None, One, Two, Three, Four };

    // Set this to one when we're active
    Phase phase = Phase.None;

    private ObjectPooler pooler;

    public List<GameObject> activeBubbles = new List<GameObject>();

    private bool spawned = false;

    private float currentTime = 0.0f;

    private float timeToSpawn = 3.0f;

    public Transform player;

    void Start()
    {
        pooler = GetComponent<ObjectPooler>();

        phase = Phase.One;
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

                if (currentTime >= timeToSpawn)
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
                // Shoot projectiles in a cross shape formation that eventually rotates around the boss
                // - Same as our demo, shoot in all 4
                // - Turn and twist somewhat randomly the "spawn points" of the balls
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
            // Just remove the first
            activeBubbles[0].GetComponent<Bubble>().Despawn();
            activeBubbles.Remove(activeBubbles[0]);
        }
    }
}
