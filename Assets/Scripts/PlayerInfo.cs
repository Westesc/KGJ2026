using UnityEngine;

[RequireComponent(typeof(Transform))]
[RequireComponent(typeof(PlayerMovement))]
public class PlayerInfo : MonoBehaviour
{
    public Transform playerTransform;
    public PlayerMovement playerMovement;
    public PlayerBody playerBody;

    private void OnValidate()
    {
        if (playerTransform == null)
        {
            playerTransform = transform;
        }
    }
}
