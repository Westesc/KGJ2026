using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInfo))]
[DisallowMultipleComponent]
public class PlayerMovement : MonoBehaviour
{
    public PlayerInfo playerInfo;
    public float moveSpeed = 5.0f;
    public float dashSpeed = 15.0f;

    public Vector2 RawMoveInput { get; private set; } = Vector2.zero;

    private Vector3 moveInput = Vector3.zero;

    private Vector3 dashInput = Vector3.zero;

    public bool Dash { get; private set; } = false;

    private void OnValidate()
    {
        if (playerInfo == null)
        {
            playerInfo = GetComponent<PlayerInfo>();
        }
    }

    void Update()
    {
        if (!playerInfo.playerBody.isMoving)
        {
            return;
        }

        float speed = moveSpeed;
        Vector3 move = moveInput;
        if (Dash)
        {
            speed = dashSpeed;
            
            if (dashInput != Vector3.zero && !playerInfo.playerBody.isDashing)
            {
                Dash = false;
            }

            if (dashInput == Vector3.zero)
            {
                if (moveInput == Vector3.zero)
                {
                    dashInput = -Vector3.forward;
                }
                else
                {
                    dashInput = moveInput;
                }
            }

            move = dashInput;
        }

        playerInfo.transform.localPosition += speed * Time.deltaTime * move;
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        RawMoveInput = ctx.ReadValue<Vector2>();
        moveInput = new Vector3(RawMoveInput.x, 0.0f, RawMoveInput.y);
    }

    public void OnDash(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed || Dash) return;
        Dash = true;
        dashInput = Vector3.zero;
    }
}
