using UnityEngine;

public class RoomDoors : MonoBehaviour
{
    public RoomInfo room;
    public bool locked = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!locked && other.tag == RoomsManager.Instance.playerTag)
        {
            room.Exit(this);
        }
    }
}
