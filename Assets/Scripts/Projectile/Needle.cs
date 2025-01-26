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

    private float projectileSpeed = 20.0f;

    [HideInInspector]
    public bool isAvailable = true;

    private Quaternion originalRotation;

    public Hitbox hitbox;

    void Start()
    {
        seedX = Random.Range(0.0f, 100.0f);
        seedY = Random.Range(0.0f, 100.0f);
        seedZ = Random.Range(0.0f, 100.0f);

        originalRotation = transform.rotation;

        hitbox.InitializeHitbox(GameManager.instance.player.GetComponent<CombatHandler>());
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        if (isFloating)
        {
            float x = Mathf.PerlinNoise(seedX, Time.time * floatSpeed) * 2.0f - 1.0f;
            float y = Mathf.PerlinNoise(seedY, Time.time * floatSpeed) * 2.0f - 1.0f;
            float z = Mathf.PerlinNoise(seedZ, Time.time * floatSpeed) * 2.0f - 1.0f;

            Vector3 offset = new Vector3(x, y, z) * floatRadius;

            transform.position = Vector3.Lerp(transform.position, floatTarget.position + offset, floatSpeed);

            Vector3 updatingTargetDir = GameManager.instance.player.
                GetComponent<PlayerStateMachine>()
                .controllerState.pointerSprite.transform.forward;

            transform.rotation = Quaternion.Slerp(transform.rotation,
                Quaternion.FromToRotation(Vector3.up, updatingTargetDir),
                5.0f * Time.deltaTime);
        }

        if (isShooting)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation,
                Quaternion.FromToRotation(Vector3.up, targetDirection),
                5.0f * Time.deltaTime);

            Vector3 finalPos = transform.position + transform.up * projectileSpeed * Time.deltaTime;

            finalPos.y = 1.5f;

            transform.position = finalPos;
        }
    }

    // Called to shoot the spike
    public void ShootSpike(Vector3 target)
    {
        isFloating = false;

        isShooting = true;

        targetDirection = target;

        isAvailable = false;

        hitbox.ActivateHitbox(1.0f, 40);
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
        // Not great
        isShooting = false;


        transform.position = floatTarget.position; // Move back to our anchor
        transform.rotation = originalRotation;
        isFloating = true;
        gameObject.SetActive(true);

        transform.parent = null;

        isAvailable = true;
    }
}
