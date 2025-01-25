using UnityEngine;

public class Needle : GameplayBehaviour 
{
    public Transform floatTarget;

    public float floatRadius = 0.25f;
    public float floatSpeed = 0.5f;

    private float seedX, seedY, seedZ;

    private bool isFloating = true;

    private bool isShooting = false;

    private Vector3 targetDirection;

    public float projectileSpeed = 2.0f;

    [HideInInspector]
    public bool isAvailable = true;

    void Start()
    {
        seedX = Random.Range(0.0f, 100.0f);
        seedY = Random.Range(0.0f, 100.0f);
        seedZ = Random.Range(0.0f, 100.0f);
    }

    protected override void OnUpdate()
    {
        if (isFloating)
        {
            float x = Mathf.PerlinNoise(seedX, Time.time * floatSpeed) * 2.0f - 1.0f;
            float y = Mathf.PerlinNoise(seedY, Time.time * floatSpeed) * 2.0f - 1.0f;
            float z = Mathf.PerlinNoise(seedZ, Time.time * floatSpeed) * 2.0f - 1.0f;

            Vector3 offset = new Vector3(x, y, z) * floatRadius;

            transform.position = Vector3.Lerp(transform.position, floatTarget.position + offset, floatSpeed);
        }

        if (isShooting)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation,
                Quaternion.FromToRotation(Vector3.up, targetDirection),
                5.0f * Time.deltaTime);

            transform.position += transform.up * projectileSpeed * Time.deltaTime;
        }
    }

    // Called to shoot the spike
    public void ShootSpike(Vector3 target)
    {
        isFloating = false;

        isShooting = true;

        targetDirection = target;

        isAvailable = false;
    }

    // Called when we hit the target
    public void TargetHit(Transform hitTarget)
    {
        transform.parent = hitTarget;
        isShooting = false;

        // Despawn after 5 seconds
        Invoke(nameof(Despawn), 5);
    }

    // Despawn/disable spike
    private void Despawn()
    {
        gameObject.SetActive(false);
    }

    // Target has been asked to be reset above character
    public void ResetSpike()
    {
        transform.position = floatTarget.position; // Move back to our anchor
        isFloating = true;
        gameObject.SetActive(true);

        isAvailable = true;
    }
}
