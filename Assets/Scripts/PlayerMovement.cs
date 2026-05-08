using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInfo))]
public class PlayerMovement : MonoBehaviour
{
    public PlayerInfo playerInfo;
    public float moveSpeed = 5.0f;

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
        playerInfo.transform.localPosition += moveSpeed * Time.deltaTime * moveInput;
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        Vector2 input = ctx.ReadValue<Vector2>();
        moveInput = new Vector3(input.x, 0.0f, input.y);
    }
}
