using UnityEngine;

[RequireComponent(typeof(Transform))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerInfo : MonoBehaviour
{
    public Transform playerTransform;
    public Rigidbody2D playerRigidbody;

    private void OnValidate()
    {
        if (playerTransform == null)
        {
            playerTransform = transform;
        }

        if (playerRigidbody == null)
        {
            playerRigidbody = GetComponent<Rigidbody2D>();
        }
    }
}
