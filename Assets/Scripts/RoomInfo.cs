using SaintsField;
using UnityEngine;

public enum DoorType { Top, Bottom, Right, Left }

public class RoomInfo : MonoBehaviour
{
    public RoomDoors topDoors;
    public RoomDoors bottomDoors;
    public RoomDoors leftDoors;
    public RoomDoors rightDoors;

    public void Exit(RoomDoors door)
    {
        if (door == null)
        {
            Debug.LogError($"Door provided was null: {door}");
            return;
        }

        if (door == bottomDoors)
        {
            RoomsManager.Instance.ExitRoom(this, DoorType.Bottom);
            return;
        }
        
        if (door == topDoors)
        {
            RoomsManager.Instance.ExitRoom(this, DoorType.Top);
            return;
        }
        
        if (door == rightDoors)
        {
            RoomsManager.Instance.ExitRoom(this, DoorType.Right);
            return;
        }
        
        if (door == leftDoors)
        {
            RoomsManager.Instance.ExitRoom(this, DoorType.Left);
            return;
        }

        Debug.LogError($"Unrecogniezed room doors: {door}");
    }
}
