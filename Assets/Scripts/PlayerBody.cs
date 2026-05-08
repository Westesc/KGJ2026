using UnityEngine;

public class PlayerBody : MonoBehaviour
{
    [SerializeField]
    private PlayerInfo playerInfo;
    private PlayerMovement playerMovement;

    public SpriteRenderer spriteRenderer;

    public Sprite idlePose;
    public Sprite moveUpPose;
    public Sprite moveDownPose;
    public Sprite moveLeftPose;

    void Start()
    {
        playerMovement = playerInfo.playerMovement;    
    }

    void UpdateBody()
    {
        spriteRenderer.flipX = false;
        if (playerMovement.RawMoveInput == Vector2.zero)
        {
            spriteRenderer.sprite = idlePose;
            return;
        }

        if (playerMovement.RawMoveInput.y > 0.0f)
        {
            spriteRenderer.sprite = moveUpPose;
            return;
        }

        if (playerMovement.RawMoveInput.y < 0.0f)
        {
            spriteRenderer.sprite = moveDownPose;
            return;
        }

        if (playerMovement.RawMoveInput.x > 0.0f)
        {
            spriteRenderer.flipX = true;
            spriteRenderer.sprite = moveLeftPose;
            return;
        }

        if (playerMovement.RawMoveInput.x < 0.0f)
        {
            spriteRenderer.sprite = moveLeftPose;
        }
    }

    void Update()
    {
        UpdateBody();
    }
}
