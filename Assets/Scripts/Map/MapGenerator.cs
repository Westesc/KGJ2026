using SaintsField.Playa;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MapGenerator : MonoBehaviour
{
    // ROOMS
    // y * size.x + x
    private RoomData[] m_Map;

    // GENERATE
    [SerializeField] private int MaxRooms = 15;
    [SerializeField] private int MinRooms = 7;

    private int m_RoomsCount = 0;
    private bool m_GenerationComplete = false;
    private Queue<Vector2Int> m_RoomQueue;

    public UnityEvent OnGenerated;

    private bool m_IsInitialized = false;

    private void SetDoors(Vector2Int pos)
    {
        int index = MapHelpers.GetMapIndexFromRoomPosition(pos);

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
            nextIndex = MapHelpers.GetMapIndexFromRoomPosition(nextPos);
            if (nextIndex != -1 && !m_Map[nextIndex].Empty)
            {
                m_Map[index].SetDoor(Vector2Int.left);
                m_Map[nextIndex].SetDoor(Vector2Int.right);
            }
        }

        if (pos.x < MapHelpers.MAP_SIZE.x - 1)
        {
            nextPos = new Vector2Int(pos.x + 1, pos.y);
            nextIndex = MapHelpers.GetMapIndexFromRoomPosition(nextPos);
            if (nextIndex != -1 && !m_Map[nextIndex].Empty)
            {
                m_Map[index].SetDoor(Vector2Int.right);
                m_Map[nextIndex].SetDoor(Vector2Int.left);
            }
        }

        if (pos.y > 0)
        {
            nextPos = new Vector2Int(pos.x, pos.y - 1);
            nextIndex = MapHelpers.GetMapIndexFromRoomPosition(nextPos);
            if (nextIndex != -1 && !m_Map[nextIndex].Empty)
            {
                m_Map[index].SetDoor(Vector2Int.down);
                m_Map[nextIndex].SetDoor(Vector2Int.up);
            }
        }

        if (pos.y < MapHelpers.MAP_SIZE.y - 1)
        {
            nextPos = new Vector2Int(pos.x, pos.y + 1);
            nextIndex = MapHelpers.GetMapIndexFromRoomPosition(nextPos);
            if (nextIndex != -1 && !m_Map[nextIndex].Empty)
            {
                m_Map[index].SetDoor(Vector2Int.up);
                m_Map[nextIndex].SetDoor(Vector2Int.down);
            }
        }
    }

    private Vector2Int GetInitialPos()
    {
        Vector2Int pos;
        if (MapHelpers.MAP_SIZE.x % 2 == 0)
        {
            System.Random r = new();
            pos = new(r.Next(MapHelpers.MAP_SIZE.x / 2 - 1, MapHelpers.MAP_SIZE.x / 2), r.Next(MapHelpers.MAP_SIZE.x / 2 - 1, MapHelpers.MAP_SIZE.x / 2));
        }
        else
        {
            pos = new(MapHelpers.MAP_SIZE.x / 2, MapHelpers.MAP_SIZE.x / 2);
        }

        return pos;
    }

    private void StartRoomGenerationFromRoom(Vector2Int pos)
    {
        m_RoomQueue.Enqueue(pos);
        SetDoors(pos);
        ++m_RoomsCount;
    }

    private int CountAdjacentRooms(Vector2Int pos)
    {
        int count = 0;

        // Left
        if (pos.x > 0 && !m_Map[MapHelpers.GetMapIndexFromRoomPosition(new Vector2Int(pos.x -1, pos.y))].Empty)
        {
            ++count;
        }

        // Right
        if (pos.x < MapHelpers.MAP_SIZE.x - 1 && !m_Map[MapHelpers.GetMapIndexFromRoomPosition(new Vector2Int(pos.x + 1, pos.y))].Empty)
        {
            ++count;
        }

        // Down
        if (pos.y > 0 && !m_Map[MapHelpers.GetMapIndexFromRoomPosition(new Vector2Int(pos.x, pos.y - 1))].Empty)
        {
            ++count;
        }

        // Up
        if (pos.y < MapHelpers.MAP_SIZE.y - 1 && !m_Map[MapHelpers.GetMapIndexFromRoomPosition(new Vector2Int(pos.x, pos.y + 1))].Empty)
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

        return true;
    }

    // Clear all rooms and try again
    private void RegenerateRooms()
    {
        for (int i = 0; i < MapHelpers.ALL_ROOM_NUM; ++i)
        {
            if (m_Map[i] == null)
            {
                m_Map[i] = RoomData.CreateEmpty();
                m_Map[i].SetPos(new Vector2Int(i % MapHelpers.MAP_SIZE.x, i / MapHelpers.MAP_SIZE.x));
            }
            else
            {
                m_Map[i].SetEmpty();
                m_Map[i].ClearDoors();
            }
        }

        m_RoomQueue.Clear();
        m_RoomsCount = 0;
        m_GenerationComplete = false;

        Vector2Int pos = GetInitialPos();
        StartRoomGenerationFromRoom(pos);
    }

    private void Init()
    {
        m_RoomQueue = new();

        m_Map = new RoomData[MapHelpers.ALL_ROOM_NUM];
        for (int i = 0; i < MapHelpers.ALL_ROOM_NUM; ++i)
        {
            m_Map[i] = RoomData.CreateEmpty();
            m_Map[i].SetPos(new Vector2Int(i % MapHelpers.MAP_SIZE.x, i / MapHelpers.MAP_SIZE.x));
        }

        m_IsInitialized = true;
    }

    [Button]
    public void Generate()
    {
        if (!m_IsInitialized)
        {
            return;
        }
        RegenerateRooms();

        while (!m_GenerationComplete)
        {
            if (m_RoomQueue.Count > 0 && m_RoomsCount < MaxRooms && !m_GenerationComplete)
            {
                Vector2Int roomPos = m_RoomQueue.Dequeue();
                if (roomPos.x > 0)
                {
                    TryGenerateRoom(new Vector2Int(roomPos.x - 1, roomPos.y));
                }
                if (roomPos.x < MapHelpers.MAP_SIZE.x - 1)
                {
                    TryGenerateRoom(new Vector2Int(roomPos.x + 1, roomPos.y));
                }
                if (roomPos.y > 0)
                {
                    TryGenerateRoom(new Vector2Int(roomPos.x, roomPos.y - 1));
                }
                if (roomPos.y < MapHelpers.MAP_SIZE.y - 1)
                {
                    TryGenerateRoom(new Vector2Int(roomPos.x, roomPos.y + 1));
                }
            }
            else if (m_RoomsCount < MinRooms)
            {
                RegenerateRooms();
            }
            else if (!m_GenerationComplete)
            {
                m_GenerationComplete = true;
                OnGenerated.Invoke();
            }
        }
    }

    public RoomData[] GetMap()
    {
        return m_Map;
    }

    private void Awake()
    {
        Init();
    }

    private void Start()
    {
        Generate();
    }
}
