using UnityEngine;

//[RequireComponent(typeof(Rigidbody))]
public class Needle : MonoBehaviour
{
    public Transform target;

    public float floatRadius = 0.25f;
    public float speed = 0.5f;

    private float seedX, seedY, seedZ;

    void Start()
    {
        seedX = Random.Range(0.0f, 100.0f);
        seedY = Random.Range(0.0f, 100.0f);
        seedZ = Random.Range(0.0f, 100.0f);
    }

    void Update()
    {
        float x = Mathf.PerlinNoise(seedX, Time.time * speed) * 2.0f - 1.0f;
        float y = Mathf.PerlinNoise(seedY, Time.time * speed) * 2.0f - 1.0f;
        float z = Mathf.PerlinNoise(seedZ, Time.time * speed) * 2.0f - 1.0f;

        Vector3 offset = new Vector3(x, y, z) * floatRadius;

        transform.position = Vector3.Lerp(transform.position, target.position + offset, speed);
    }
}
