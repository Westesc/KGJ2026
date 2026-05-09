using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Map size 6x6
/// </summary>
public class MapGenerator : MonoBehaviour
{
    // STATIC
    private static readonly Vector2Int MAP_SIZE = new(6, 6);
    private static readonly int ALL_ROOM_NUM = MAP_SIZE.x * MAP_SIZE.y;
    private static readonly Vector2Int TEXTURE_PIXELS = new(3, 3);
    private static readonly int ALL_TEXTURE_PIXELS = TEXTURE_PIXELS.x * TEXTURE_PIXELS.y;
    private static readonly Color32[] DEFAULT_TEXTURE_COLOR = { Color.darkGray, Color.darkGray, Color.darkGray, Color.darkGray, Color.darkGray, Color.darkGray, Color.darkGray, Color.darkGray, Color.darkGray };

    // ROOMS
    // y * size.x + x
    private Room[] m_Map;

    // DRAW
    private Texture2D[] m_Textures;
    private RawImage[] m_Images;

    [SerializeField] private Vector2Int RoomSize = new(50, 50);
    [SerializeField] private Vector2Int RoomOffset = new(10, 10);
    [SerializeField] private GameObject parentObject;

    // GENERATE
    [SerializeField] private int MaxRooms = 15;
    [SerializeField] private int MinRooms = 7;

    private int m_RoomsCount = 0;
    private bool m_GenerationComplete = false;
    private Queue<Vector2Int> m_RoomQueue;

    private int GetMapIndexFromRoomPosition(Vector2Int pos)
    {
        if (pos.x < 0 || pos.y < 0 || pos.x >= MAP_SIZE.x || pos.y >= MAP_SIZE.y)
        {
            return -1;
        }
        return pos.y * MAP_SIZE.x + pos.x;
    }

    private Vector2Int GetRoomPositionFromMapIndex(int index)
    {
        if (index < 0 || index >= ALL_ROOM_NUM)
        {
            return new Vector2Int(-1, -1);
        }
        return new Vector2Int(index % MAP_SIZE.x, index / MAP_SIZE.x);
    }

    private Vector3 GetWorldPositionFromMapIndex(int index)
    {
        Vector2Int pos = GetRoomPositionFromMapIndex(index);
        float imageSizeWPaddingX = RoomSize.x + RoomOffset.x;
        float halfSizeX = imageSizeWPaddingX * MAP_SIZE.x * 0.5f;
        float startX = -halfSizeX + imageSizeWPaddingX * 0.5f;
        float addX = imageSizeWPaddingX;

        float imageSizeWPaddingY = RoomSize.y + RoomOffset.y;
        float halfSizeY = imageSizeWPaddingY * MAP_SIZE.y * 0.5f;
        float startY = halfSizeY - imageSizeWPaddingY * 0.5f;
        float addY = -imageSizeWPaddingY;
        return new Vector3(startX + pos.x * addX, startY + pos.y * addY, 0f);
    }

    private void DrawRoom(Vector2Int pos)
    {
        int index = GetMapIndexFromRoomPosition(pos);
        if (index < 0)
        {
            return;
        }
        Room r = m_Map[index];
        for (int z = 0; z < ALL_TEXTURE_PIXELS; ++z)
        {
            int x = z % TEXTURE_PIXELS.x;
            int y = z / TEXTURE_PIXELS.x;
            if ((r.Up && z == 1) || (r.Right && z == 5) || (r.Down && z == 7) || (r.Left && z == 3) || (z == 4 && !r.Empty))
            {
                m_Textures[index].SetPixel(x, y, Color.darkSlateGray);
            }
            else if (!r.Empty && z != 4)
            {
                m_Textures[index].SetPixel(x, y, Color.cadetBlue);
            }
        }

        m_Textures[index].Apply();
    }

    private void SetDoors(Vector2Int pos)
    {
        int index = GetMapIndexFromRoomPosition(pos);

        if (index < 0)
        {
            return;
        }

        if (m_Map[index].Empty)
        {
            m_Map[index].SetNotEmpty();
        }

        Vector2Int nextPos;
        int nextIndex;
        if (pos.x > 0)
        {
            nextPos = new Vector2Int(pos.x - 1, pos.y);
            nextIndex = GetMapIndexFromRoomPosition(nextPos);
            if (nextIndex != -1 && !m_Map[nextIndex].Empty)
            {
                m_Map[index].SetDoor(Vector2Int.left);
                m_Map[nextIndex].SetDoor(Vector2Int.right);

                DrawRoom(nextPos);
            }
        }

        if (pos.x < MAP_SIZE.x - 1)
        {
            nextPos = new Vector2Int(pos.x + 1, pos.y);
            nextIndex = GetMapIndexFromRoomPosition(nextPos);
            if (nextIndex != -1 && !m_Map[nextIndex].Empty)
            {
                m_Map[index].SetDoor(Vector2Int.right);
                m_Map[nextIndex].SetDoor(Vector2Int.left);

                DrawRoom(nextPos);
            }
        }

        if (pos.y > 0)
        {
            nextPos = new Vector2Int(pos.x, pos.y - 1);
            nextIndex = GetMapIndexFromRoomPosition(nextPos);
            if (nextIndex != -1 && !m_Map[nextIndex].Empty)
            {
                m_Map[index].SetDoor(Vector2Int.down);
                m_Map[nextIndex].SetDoor(Vector2Int.up);

                DrawRoom(nextPos);
            }
        }

        if (pos.y < MAP_SIZE.y - 1)
        {
            nextPos = new Vector2Int(pos.x, pos.y + 1);
            nextIndex = GetMapIndexFromRoomPosition(nextPos);
            if (nextIndex != -1 && !m_Map[nextIndex].Empty)
            {
                m_Map[index].SetDoor(Vector2Int.up);
                m_Map[nextIndex].SetDoor(Vector2Int.down);

                DrawRoom(nextPos);
            }
        }
    }

    private Vector2Int GetInitialPos()
    {
        Vector2Int pos;
        if (MAP_SIZE.x % 2 == 0)
        {
            System.Random r = new();
            pos = new(r.Next(MAP_SIZE.x / 2 - 1, MAP_SIZE.x / 2), r.Next(MAP_SIZE.x / 2 - 1, MAP_SIZE.x / 2));
        }
        else
        {
            pos = new(MAP_SIZE.x / 2, MAP_SIZE.x / 2);
        }

        return pos;
    }

    private void StartRoomGenerationFromRoom(Vector2Int pos)
    {
        m_RoomQueue.Enqueue(pos);
        SetDoors(pos);
        ++m_RoomsCount;
        DrawRoom(pos);
    }

    private int CountAdjacentRooms(Vector2Int pos)
    {
        int count = 0;

        // Left
        if (pos.x > 0 && !m_Map[GetMapIndexFromRoomPosition(new Vector2Int(pos.x -1, pos.y))].Empty)
        {
            ++count;
        }

        // Right
        if (pos.x < MAP_SIZE.x - 1 && !m_Map[GetMapIndexFromRoomPosition(new Vector2Int(pos.x + 1, pos.y))].Empty)
        {
            ++count;
        }

        // Down
        if (pos.y > 0 && !m_Map[GetMapIndexFromRoomPosition(new Vector2Int(pos.x, pos.y - 1))].Empty)
        {
            ++count;
        }

        // Up
        if (pos.y < MAP_SIZE.y - 1 && !m_Map[GetMapIndexFromRoomPosition(new Vector2Int(pos.x, pos.y + 1))].Empty)
        {
            ++count;
        }

        return count;
    }

    private bool TryGenerateRoom(Vector2Int pos)
    {
        if (m_RoomsCount >= MaxRooms)
        {
            return false;
        }

        if (UnityEngine.Random.value < 0.25f && pos != Vector2.zero)
        {
            return false;
        }

        if (CountAdjacentRooms(pos) > 1)
        {
            return false;
        }

        m_RoomQueue.Enqueue(pos);
        SetDoors(pos);
        ++m_RoomsCount;

        DrawRoom(pos);

        return true;
    }

    // Clear all rooms and try again
    private void RegenerateRooms()
    {
        for (int i = 0; i < ALL_ROOM_NUM; ++i)
        {
            if (m_Map[i] == null)
            {
                m_Map[i] = Room.CreateEmpty();
                m_Map[i].SetPos(new Vector2Int(i % MAP_SIZE.x, i / MAP_SIZE.x));
            }
            else
            {
                m_Map[i].SetEmpty();
                m_Map[i].ClearDoors();
            }
        }

        for (int i = 0; i < ALL_ROOM_NUM; ++i)
        {
            if (m_Textures[i] == null)
            {
                m_Textures[i] = new(TEXTURE_PIXELS.x, TEXTURE_PIXELS.y);
                m_Textures[i].filterMode = FilterMode.Point;
            }

            m_Textures[i].SetPixels32(DEFAULT_TEXTURE_COLOR);
        }

        m_RoomQueue.Clear();
        m_RoomsCount = 0;
        m_GenerationComplete = false;

        Vector2Int pos = GetInitialPos();
        StartRoomGenerationFromRoom(pos);
    }

    public void Start()
    {
        m_RoomQueue = new();

        m_Textures = new Texture2D[ALL_ROOM_NUM];
        for (int i = 0; i < ALL_ROOM_NUM; ++i)
        {
            m_Textures[i] = new(TEXTURE_PIXELS.x, TEXTURE_PIXELS.y);
            m_Textures[i].filterMode = FilterMode.Point;

            m_Textures[i].SetPixels32(DEFAULT_TEXTURE_COLOR);
        }

        m_Images = new RawImage[ALL_ROOM_NUM];
        for (int i = 0; i < ALL_ROOM_NUM; ++i)
        {
            GameObject obj = new($"Image_{i % MAP_SIZE.x}_{i / MAP_SIZE.x}");
            obj.transform.parent = parentObject.transform;
            m_Images[i] = obj.AddComponent<RawImage>();
            m_Images[i].rectTransform.sizeDelta = RoomSize;
            m_Images[i].rectTransform.SetLocalPositionAndRotation(GetWorldPositionFromMapIndex(i), Quaternion.identity);
            m_Images[i].texture = m_Textures[i];
        }

        m_Map = new Room[ALL_ROOM_NUM];
        for (int i = 0; i < ALL_ROOM_NUM; ++i)
        {
            m_Map[i] = Room.CreateEmpty();
            m_Map[i].SetPos(new Vector2Int(i % MAP_SIZE.x, i / MAP_SIZE.x));
        }

        Vector2Int pos = GetInitialPos();
        StartRoomGenerationFromRoom(pos);
    }

    public void Update()
    {
        if (m_RoomQueue.Count > 0 && m_RoomsCount < MaxRooms && !m_GenerationComplete)
        {
            Vector2Int roomPos = m_RoomQueue.Dequeue();
            if (roomPos.x > 0)
            {
                TryGenerateRoom(new Vector2Int(roomPos.x - 1, roomPos.y));
            }
            if (roomPos.x < MAP_SIZE.x - 1)
            {
                TryGenerateRoom(new Vector2Int(roomPos.x + 1, roomPos.y));
            }
            if (roomPos.y > 0)
            {
                TryGenerateRoom(new Vector2Int(roomPos.x, roomPos.y - 1));
            }
            if (roomPos.y < MAP_SIZE.y - 1)
            {
                TryGenerateRoom(new Vector2Int(roomPos.x, roomPos.y + 1));
            }            
        }
        else if(m_RoomsCount < MinRooms)
        {
            RegenerateRooms();
        }
        else if (!m_GenerationComplete)
        {
            m_GenerationComplete = true;
        }
    }
}
