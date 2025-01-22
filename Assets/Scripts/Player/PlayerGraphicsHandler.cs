using UnityEngine;

public class PlayerGraphicsHandler : MonoBehaviour
{
    public Transform graphicParent;
    public SpriteRenderer spriteRenderer;

    public Sprite idleSprite;
    public Sprite moveSprite;
    public Sprite dashSprite;

    public float floatHeight;
    public float floatSpeed;
    public float floatOffset;

    private PlayerStateMachine psm;
    private Vector3 ogGraphicPos;

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
        if(psm.GetCurrentState() == psm.controllerState)
        {
            spriteRenderer.sprite = psm.controllerState.currentPlayerVelocity.magnitude > 0.1 ? moveSprite : idleSprite;
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
