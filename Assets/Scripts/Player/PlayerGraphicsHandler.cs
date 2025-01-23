using UnityEngine;

public class PlayerGraphicsHandler : MonoBehaviour
{
    public Transform graphicParent;
    public SpriteRenderer spriteRenderer;

    public SpriteSet idleSet, moveSet, dashSet;
    private SpriteSet currentSpriteSet;

    //WIP REFACTORING
    public Sprite idleSprite;
    public Sprite moveSprite;
    public Sprite moveBackSprite;
    public Sprite dashSprite;

    public float floatHeight;
    public float floatSpeed;
    public float floatOffset;
    public float oppositeMovementThreshold = 0.75f;
    public float speedThreshold = .5f;

    private PlayerStateMachine psm;
    private Vector3 ogGraphicPos;
    private bool moving;
    private bool movingOpposite;

    public void Awake()
    {
        psm = GetComponent<PlayerStateMachine>();
        ogGraphicPos = graphicParent.transform.localPosition;
    }

    private void Update()
    {
        HandleSpriteDirection();
        HandleGraphicFloat();
        MovementCheck();
    }

    public void MovementCheck()
    {
        //if player is moving the opposite way that their aiming, do the back sprite
        // Convert aim angle to a direction vector in the XZ plane
        Vector3 aimDirection = new Vector3(Mathf.Cos(Mathf.Deg2Rad * psm.controllerState.currentLookAngle), 0, Mathf.Sin(Mathf.Deg2Rad * psm.controllerState.currentLookAngle)).normalized;

        // Normalize the player's velocity to get direction
        Vector3 velocityDirection = psm.controllerState.currentPlayerVelocity.normalized;

        // Calculate the dot product
        float dot = Vector3.Dot(aimDirection, velocityDirection);

        // Check if the player is moving generally opposite to their aim
        movingOpposite = dot < -oppositeMovementThreshold;

        if (psm.GetCurrentState() == psm.controllerState)
        {
            moving = psm.controllerState.currentPlayerVelocity.magnitude > speedThreshold;

            if (moving)
            {
                spriteRenderer.sprite = movingOpposite ? moveSprite : moveBackSprite;
            }
            else
            {
                spriteRenderer.sprite = idleSprite;
            }
        }
        else
        {
            if(psm.GetCurrentState() == psm.dodgeState)
            {
                spriteRenderer.sprite = movingOpposite ? dashSprite : moveBackSprite;
            }
        }
    }

    public void HandleGraphicFloat()
    {
        graphicParent.transform.localPosition = new Vector3(ogGraphicPos.x, floatOffset + ogGraphicPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight, ogGraphicPos.z);
    }

    public void HandleSpriteDirection()
    {
        spriteRenderer.flipX = psm.controllerState.currentLookAngle <= 0;
    }

}

[System.Serializable]
public struct SpriteSet
{
    public Sprite forward, back, side;
}
