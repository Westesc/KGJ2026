using UnityEngine;
using System.Collections.Generic;

public class MapHelpers : MonoBehaviour
{
    // STATIC
    public static readonly Vector2Int MAP_SIZE = new(6, 4);
    public static readonly int ALL_ROOM_NUM = MAP_SIZE.x * MAP_SIZE.y;

    public static int GetMapIndexFromRoomPosition(Vector2Int pos)
    {
        if (pos.x < 0 || pos.y < 0 || pos.x >= MAP_SIZE.x || pos.y >= MAP_SIZE.y)
        {
            return -1;
        }
        return pos.y * MAP_SIZE.x + pos.x;
    }

    public static Vector2Int GetRoomPositionFromMapIndex(int index)
    {
        if (index < 0 || index >= ALL_ROOM_NUM)
        {
            return new Vector2Int(-1, -1);
        }
        return new Vector2Int(index % MAP_SIZE.x, index / MAP_SIZE.x);
    }

    public static Vector3 GetWorldPositionFromMapIndex(int index, Vector2 roomSize, Vector2 roomOffset)
    {
        Vector2Int pos = GetRoomPositionFromMapIndex(index);
        float imageSizeWPaddingX = roomSize.x + roomOffset.x;
        float halfSizeX = imageSizeWPaddingX * MAP_SIZE.x * 0.5f;
        float startX = -halfSizeX + imageSizeWPaddingX * 0.5f;
        float addX = imageSizeWPaddingX;

        float imageSizeWPaddingY = roomSize.y + roomOffset.y;
        float halfSizeY = imageSizeWPaddingY * MAP_SIZE.y * 0.5f;
        float startY = halfSizeY - imageSizeWPaddingY * 0.5f;
        float addY = -imageSizeWPaddingY;
        return new Vector3(startX + pos.x * addX, startY + pos.y * addY, 0f);
    }

    public static int CalculateNumOfConnections(RoomData data)
    {
        if (data.Empty)
        {
            return 0;
        }

        int count = 0;
        if (data.Up)
        {
            ++count;
        }
        if (data.Down)
        {
            ++count;
        }
        if (data.Left)
        {
            ++count;
        }
        if (data.Right)
        {
            ++count;
        }

        return count;
    }

    public static float Distance2(int a, int b)
    {
        return a > b ? a * a - b * b : b * b - a * a;
    }

    public static void ClearColor(ref Texture2D tex, Color color)
    {
        for (int i = 0; i < tex.width; ++i)
        {
            for (int j = 0; j < tex.height; ++j)
            {
                tex.SetPixel(i, j, color);
            }
        }
        tex.Apply();
    }

    public static List<int> GetNonEmptyNeighbours(int index, RoomData[] map)
    {
        List<int> ret = new();
        Vector2Int pos = GetRoomPositionFromMapIndex(index);

        if (pos.x > 0 && !map[index - 1].Empty)
        {
            ret.Add(index - 1);
        }

        if (pos.x < MAP_SIZE.x - 1 && !map[index + 1].Empty)
        {
            ret.Add(index + 1);
        }

        if (pos.y > 0 && !map[index - MAP_SIZE.x].Empty)
        {
            ret.Add(index - MAP_SIZE.x);
        }

        if (pos.y < MAP_SIZE.y - 1 && !map[index + MAP_SIZE.x].Empty)
        {
            ret.Add(index + MAP_SIZE.x);
        }

        return ret;
    }
}
