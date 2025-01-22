using UnityEngine;

public class Needle : MonoBehaviour
{
    public Transform floatTarget;

    public float floatRadius = 0.25f;
    public float speed = 0.5f;

    private float seedX, seedY, seedZ;

    private bool isFloating = true;

    private bool isShooting = false;

    private Transform hitTarget;

    void Start()
    {
        seedX = Random.Range(0.0f, 100.0f);
        seedY = Random.Range(0.0f, 100.0f);
        seedZ = Random.Range(0.0f, 100.0f);
    }

    void Update()
    {
        if (isFloating)
        {
            float x = Mathf.PerlinNoise(seedX, Time.time * speed) * 2.0f - 1.0f;
            float y = Mathf.PerlinNoise(seedY, Time.time * speed) * 2.0f - 1.0f;
            float z = Mathf.PerlinNoise(seedZ, Time.time * speed) * 2.0f - 1.0f;

            Vector3 offset = new Vector3(x, y, z) * floatRadius;

            transform.position = Vector3.Lerp(transform.position, floatTarget.position + offset, speed);
        }

        if (isShooting)
        {
            transform.position = Vector3.Lerp(transform.position, hitTarget.position, speed * Time.deltaTime);
        }
    }

    // Called to shoot the spike
    public void ShootSpike(Transform target)
    {
        isFloating = false;

        isShooting = true;

        hitTarget = target;
    }

    // Called when we hit the target
    public void TargetHit()
    {
        transform.parent = hitTarget;
        isShooting = false;

        // Despawn after 5 seconds
        Invoke(nameof(Despawn), 5);
    }

    // Despawn/disable spike
    private void Despawn()
    {
        enabled = false;
    }

    // Target has been asked to be reset above character
    public void ResetSpike()
    {
        transform.position = floatTarget.position; // Move back to our anchor
        isFloating = true;
        enabled = true;
    }
}
