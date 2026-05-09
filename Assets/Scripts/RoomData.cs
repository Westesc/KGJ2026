using UnityEngine;

/// <summary>
/// - 3 bits for x
/// - 3 bits for y
/// - 4 bits for doors
/// - 1 bit is empty
/// </summary>
public class RoomData
{
    // |fre|||dr||y||x|
    // 0000000000000000
    private short data;

    public RoomData()
    {
        this.data = 1 << 10;
    }

    public RoomData(short data)
    {
        this.data = data;
    }

    public int X
    {
        get
        {
            return data & 7;
        }
    }

    public int Y
    {
        get
        {
            return (data & (7 << 3) >> 3);
        }
    }

    public bool Up
    {
        get
        {
            return (data & (1 << 6)) >> 6 == 1;
        }
    }

    public bool Right
    {
        get
        {
            return (data & (1 << 7)) >> 7 == 1;
        }
    }

    public bool Down
    {
        get
        {
            return (data & (1 << 8)) >> 8 == 1;
        }
    }

    public bool Left
    {
        get
        {
            return (data & (1 << 9)) >> 9 == 1;
        }
    }

    public bool Empty
    {
        get
        {
            return (data & (1 << 10)) >> 10 == 1;
        }
    }

    public void Reset()
    {
        SetEmpty();
        ClearDoors();
        ClearPosition();
    }

    public void SetNotEmpty()
    {
        this.data &= ~(1 << 10);
    }

    public void SetEmpty()
    {
        this.data = 1 << 10;
    }

    public void SetDoors(short value)
    {
        SetNotEmpty();
        short v = (short)Mathf.Clamp(value, 0, 8);
        ClearDoors();
        this.data |= (short)(v << 6);
    }

    public void ClearDoors()
    {
        this.data &= ~(15 << 6);
    }

    public void ClearPosition()
    {
        this.data &= ~63;
    }

    public void SetDoors(bool up, bool right, bool down, bool left)
    {
        SetNotEmpty();
        this.data &= ~(15 << 6);
        if (up)
        {
            this.data |= (1 << 6);
        }
        if (right)
        {
            this.data |= (1 << 7);
        }
        if (down)
        {
            this.data |= (1 << 8);
        }
        if (left)
        {
            this.data |= (1 << 9);
        }
    }

    public void SetDoor(Vector2Int direction)
    {
        SetNotEmpty();
        if (direction == Vector2Int.up)
        {
            this.data |= (1 << 6);
        }
        if (direction == Vector2Int.right)
        {
            this.data |= (1 << 7);
        }
        if (direction == Vector2Int.down)
        {
            this.data |= (1 << 8);
        }
        if (direction == Vector2Int.left)
        {
            this.data |= (1 << 9);
        }
    }

    public void SetPos(Vector2Int pos)
    {
        ClearPosition();
        Vector2Int value = new Vector2Int(Mathf.Clamp(pos.x, 0, 6), Mathf.Clamp(pos.x, 0, 6));
        this.data |= (short)value.x;
        this.data |= (short)(value.y << 3);
    }

    public static RoomData CreateEmpty() => new();
}
