using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bubble : MonoBehaviour
{
    public float speed = 0.1f;

    public float bubbleWabbleVarience = 0.5f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    void Update()
    {
        // This would add up for bubble effect floating up
        //rb.AddForce(Vector3.up * 2f);

        // Add force for "bullet"
        rb.AddForce(transform.forward * speed);
        
        // Add drift
        rb.AddForce(new Vector3(Random.Range(-bubbleWabbleVarience, bubbleWabbleVarience), 0, Random.Range(-bubbleWabbleVarience, bubbleWabbleVarience))); // random drift
    }
}
