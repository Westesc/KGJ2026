using SaintsField;
using UnityEngine;

public class CameraFolow : MonoBehaviour
{
    [Tag]
    public string playerTag;
    private Transform playerTransform;

    [SerializeField] 
    private Vector2 offset;

    [SerializeField]
    private float snapBoxSize = 2.0f;

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag(playerTag).GetComponent<PlayerInfo>().playerTransform;
    }

    void Update()
    {
        Vector3 desiredPosition = new(playerTransform.position.x + offset.x, transform.position.y, playerTransform.position.z + offset.y);

        Vector3 newPosition = transform.position;

        // RIGHT SNAP BOX WALL
        if (desiredPosition.x - newPosition.x > snapBoxSize)
        {
            newPosition.x += desiredPosition.x - newPosition.x - snapBoxSize;
        }

        // LEFT SNAP BOX WALL
        if (desiredPosition.x - newPosition.x < -snapBoxSize)
        {
            newPosition.x += desiredPosition.x - newPosition.x + snapBoxSize;
        }

        // TOP SNAP BOX WALL
        if (desiredPosition.z - newPosition.z > snapBoxSize)
        {
            newPosition.z += desiredPosition.z - newPosition.z - snapBoxSize;
        }

        // BOTTOM SNAP BOX WALL
        if (desiredPosition.z - newPosition.z < -snapBoxSize)
        {
            newPosition.z += desiredPosition.z - newPosition.z + snapBoxSize;
        }

        transform.position = newPosition;
    }
}
