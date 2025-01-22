using UnityEngine;

[System.Serializable]
public class PlayerControllerState : PlayerState
{
    //pc use mouse to change orientation
    //controller twin stick

    public Vector2 moveInput;
    public Vector2 lookInput;
    public CharacterController cCon;
    public float currentLookAngle;
    public float playerSpeed;
    public float controllerLookSpeed;
    public GameObject pointerSprite;

    public Vector3 currentPlayerVelocity;
    public Vector3 mouseWorldPosition;
    public Vector3 mouseScreenPosition;
    private float currentSpeed;

    public void UpdatePlayerMovement(Vector2 _move)
    {
        moveInput = _move;
    }

    public void UpdatePlayerLook(Vector2 _look)
    {
        lookInput = _look;
    }

    public override void OnStateUpdate()
    {
        Movement();
        MovePlayer();
        HandleLookAngle();
    }


    public void Movement()
    {

        //currentSpeed = Mathf.Lerp(currentSpeed, maxSpeedThisFrame, currentAccelerationMod * Time.deltaTime);
        currentSpeed = playerSpeed;

        // Get the adjusted camera forward direction
        Vector3 cameraForward = Camera.main.transform.forward;
        cameraForward.y = 0f; // Remove the vertical component
        cameraForward.Normalize();

        // Get the camera's right direction
        Vector3 cameraRight = Camera.main.transform.right;
        cameraRight.y = 0f; // Remove the vertical component
        cameraRight.Normalize();


        currentPlayerVelocity = (cameraRight * moveInput.x  + cameraForward * moveInput.y) * currentSpeed;
    }

    public void MovePlayer()
    {
        cCon.Move(currentPlayerVelocity * Time.deltaTime);
    }

    public void HandleLookAngle()
    {
        //read look input, determine look angle float, rotate sprite
        if (psm.GetComponent<PlayerInputHandler>().gamePad)
        {
            float newAngle = Mathf.Atan2(lookInput.x, lookInput.y) * Mathf.Rad2Deg;

            if(lookInput.magnitude >= .1f)
            {
                currentLookAngle = newAngle;
            }
        }
        else
        {
            // Get the normalized mouse position (values between 0 and 1)
            Vector2 mousePosition = lookInput;

            // Create a ray from the camera to the mouse position in world space
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);

            // Perform a raycast to find the intersection with the ground plane (XZ plane at the specified height)
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                // Get the hit point (world position) on the ground plane
                mouseWorldPosition = hit.point;

                // Calculate direction to the mouse position
                Vector3 direction = mouseWorldPosition - pointerSprite.transform.position;

                // Project the direction onto the XZ plane (ignore Y axis)
                direction.y = 0;

                // Ensure the direction is normalized
                direction.Normalize();

                // Calculate the rotation around the Y-axis only (using the y component of the direction)
                currentLookAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

            }


        }


        // Apply the rotation to the pointer
        pointerSprite.transform.rotation = Quaternion.Euler(0, currentLookAngle, 0);

    }

}
