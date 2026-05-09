using SaintsField;
using UnityEngine;

public enum DoorType { Left = 0, Top = 1, Right = 2, Bottom = 3 }

public class RoomInfo : MonoBehaviour
{
    public int index;
    public RoomDoor topDoors;
    public RoomDoor bottomDoors;
    public RoomDoor leftDoors;
    public RoomDoor rightDoors;

    public void Rotate(uint cycles)
    {
        cycles %= 4;
        for (uint i = 0; i < cycles; ++i)
        {
            RotateOne();
        }
    }

    public void RotateOne()
    {
        (topDoors, rightDoors, bottomDoors, leftDoors) = (leftDoors, topDoors, rightDoors, bottomDoors);

        topDoors.SetDoorType(DoorType.Top);
        rightDoors.SetDoorType(DoorType.Right);
        bottomDoors.SetDoorType(DoorType.Bottom);
        leftDoors.SetDoorType(DoorType.Left);
    }

    public void Exit(RoomDoor door)
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
