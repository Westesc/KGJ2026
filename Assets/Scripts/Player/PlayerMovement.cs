using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInfo))]
[DisallowMultipleComponent]
public class PlayerMovement : MonoBehaviour
{
    public PlayerInfo playerInfo;
    public float moveSpeed = 5.0f;

    public Vector2 RawMoveInput { get; private set; } = Vector2.zero;

    private Vector3 moveInput = Vector3.zero;

    private void OnValidate()
    {
        if (playerInfo == null)
        {
            playerInfo = GetComponent<PlayerInfo>();
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (this.transform.GetComponentInChildren<PlayerBody>().isMoving)
        {
            playerInfo.transform.localPosition += moveSpeed * Time.deltaTime * moveInput;
        }
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        RawMoveInput = ctx.ReadValue<Vector2>();
        moveInput = new Vector3(RawMoveInput.x, 0.0f, RawMoveInput.y);
    }
}
