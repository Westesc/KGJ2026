using UnityEngine;

public class RoomDoorTrigger : MonoBehaviour
{
    public RoomDoor door;
    private GameObject player = null;

    void Update()
    {
        if (player != null)
        {
            door.Exit();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(RoomsManager.Instance.playerTag))
        {
            player = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(RoomsManager.Instance.playerTag))
        {
            player = null;
        }
    }
}
