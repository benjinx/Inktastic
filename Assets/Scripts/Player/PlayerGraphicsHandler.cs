using UnityEngine;

public class PlayerGraphicsHandler : GameplayBehaviour
{
    public Transform graphicParent;
    public SpriteRenderer spriteRenderer;

    public SpriteSet idleSet, moveSet, moveBackSet, dashSet, attackSet, stunSet, deathSet;
    private SpriteSet currentSpriteSet;

    //WIP REFACTORING
   /* public Sprite idleSprite;
    public Sprite moveSprite;
    public Sprite moveBackSprite;
    public Sprite dashSprite;
*/
    public float floatHeight;
    public float floatSpeed;
    public float floatOffset;
    public float oppositeMovementThreshold = 0.75f;
    public float speedThreshold = .5f;

    private PlayerStateMachine psm;
    private Vector3 ogGraphicPos;
    private bool moving;
    private bool movingOpposite;

    protected override void Awake()
    {
        base.Awake();
        psm = GetComponent<PlayerStateMachine>();
        ogGraphicPos = graphicParent.transform.localPosition;
    }

    protected override void OnUpdate()
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
                //spriteRenderer.sprite = movingOpposite ? moveSprite : moveBackSprite;
                currentSpriteSet = movingOpposite ? moveSet : moveBackSet;
            }
            else
            {
                //spriteRenderer.sprite = idleSprite;
                currentSpriteSet = idleSet;
            }
        }
        else
        {
            if(psm.GetCurrentState() == psm.dodgeState)
            {
                //spriteRenderer.sprite = movingOpposite ? dashSprite : moveBackSprite;
                currentSpriteSet = movingOpposite ? dashSet : moveBackSet;
            }

            if(psm.GetCurrentState() == psm.attackState)
            {
                currentSpriteSet = attackSet;
            }

            if(psm.GetCurrentState() == psm.stunState)
            {
                currentSpriteSet = stunSet;
            }

            if(psm.GetCurrentState() == psm.deathState)
            {
                currentSpriteSet = deathSet;
            }
        }

        if(currentSpriteSet != null)
        {
            Sprite loadedSprite = currentSpriteSet.GetDirectionalSprite(psm.controllerState.currentLookAngle);

            if (loadedSprite != null)
            {
                spriteRenderer.sprite = loadedSprite;
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
public class SpriteSet
{
    public Sprite forward, back, side;

    public Sprite GetDirectionalSprite(float angle)
    {
        // Normalize the angle to the range [0, 360)
        angle = Mathf.Repeat(angle, 360f);

        // Determine the direction
        Sprite selectedSprite;
        if (angle >= 45f && angle < 135f)
        {
            selectedSprite = forward; // Forward (e.g., facing up)
        }
        else if (angle >= 225f && angle < 315f)
        {
            selectedSprite = back; // Back (e.g., facing down)
        }
        else
        {
            selectedSprite = side; // Side (default case for left/right)
        }

        // Fallback logic: return the first non-null sprite, or null if none exists
        if (selectedSprite != null) return selectedSprite;
        if (forward != null) return forward;
        if (back != null) return back;
        if (side != null) return side;

        // All sprites are null
        return null;
    }
}
