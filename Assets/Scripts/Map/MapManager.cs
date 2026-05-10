using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;
using static RoomsManager;

[RequireComponent(typeof(MapGenerator))]
public class MapManager : MonoBehaviour
{
    // STATIC
    private static readonly Vector2Int TEXTURE_PIXELS = new(9, 9);
    private static readonly int ALL_TEXTURE_PIXELS = TEXTURE_PIXELS.x * TEXTURE_PIXELS.y;
    private static readonly Color32 DEFAULT_ROOM_COLOR_VALUE = new(41, 41, 41, 0);
    private static readonly Color32 DEFAULT_BG_COLOR_VALUE = Color.gray2;
    private static readonly Color32[] DEFAULT_ROOM_TEXTURE_COLOR =
    {
        DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE,
        DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE,
        DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE,
        DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE,
        DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE,
        DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE,
        DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE,
        DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE,
        DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE, DEFAULT_ROOM_COLOR_VALUE
    };

    private static readonly Color32[] DEFAULT_CONNECTION_TEXTURE_COLOR =
    {
        DEFAULT_BG_COLOR_VALUE
    };

    // DRAW
    private Texture2D[] m_RoomTextures;
    private RawImage[] m_RoomImages;

    private List<Texture2D> m_ConnectionTextures;
    private List<RawImage> m_ConnectionImages;

    [SerializeField] private Vector2 RoomSize = new(50, 50);
    [SerializeField] private Vector2 RoomOffset = new(10, 10);
    [SerializeField] private GameObject parentObject;

    // ROOMS
    private List<GameObject> m_Rooms;
    private Dictionary<int, int> m_RoomIndexToObjectIndex;
    private List<bool> m_RoomsVisible;
    private List<bool> m_RoomsVisited;

    [SerializeField] private GameObject[] OneWayRoom;

    [SerializeField] private GameObject[] TwoWayRoomMirror;
    [SerializeField] private GameObject[] TwoWayRoomL;

    [SerializeField] private GameObject[] ThreeWayRoom;

    [SerializeField] private GameObject[] FourWayRoom;

    // GENERATOR
    private MapGenerator m_Generator;

    // ACTIVE
    private int activeIndex = 0;
    private int endIndex = 0;

    private void DrawRoom(RoomData data, int index)
    {
        if (data.Empty)
        {
            return;
        }

        for (int z = 0; z < ALL_TEXTURE_PIXELS; ++z)
        {
            int x = z % TEXTURE_PIXELS.x;
            int y = z / TEXTURE_PIXELS.x;
            if ((data.Up && (z == 3 || z == 4 || z == 5)) ||
                (data.Right && (z == 35 || z == 44 || z == 53)) ||
                (data.Down && (z == 75 || z == 76 || z == 77)) ||
                (data.Left && (z == 27 || z == 36 || z == 45)))
            {
                m_RoomTextures[index].SetPixel(x, y, DEFAULT_BG_COLOR_VALUE);
            }
            else if (!data.Empty && (x - 1 < 0 || x + 1 == TEXTURE_PIXELS.x || y - 1 < 0 || y + 1 == TEXTURE_PIXELS.y))
            {
                m_RoomTextures[index].SetPixel(x, y, Color.blue);
            }
            else
            {
                m_RoomTextures[index].SetPixel(x, y, DEFAULT_BG_COLOR_VALUE);
            }
        }

        m_RoomTextures[index].Apply();
    }

    private void DrawConnection(ConnectionsData data)
    {
        Vector3 pos = MapHelpers.GetWorldPositionFromMapIndex(data.indexA, RoomSize, RoomOffset);
        Vector2 size = Vector2.zero;

        if (data.isHorizontal)
        {
            pos.x -= (RoomOffset.x + RoomSize.x) * 0.5f;
            size.x = RoomOffset.x * parentObject.transform.lossyScale.x;
            size.y = (3f / (float)TEXTURE_PIXELS.y) * RoomSize.y * parentObject.transform.lossyScale.y;
        }
        else
        {
            pos.y -= (RoomOffset.x + RoomSize.x) * 0.5f;
            size.x = (3f / (float)TEXTURE_PIXELS.x) * RoomSize.x * parentObject.transform.lossyScale.x;
            size.y = RoomOffset.y * parentObject.transform.lossyScale.y;
        }

        GameObject obj = new($"Con_{data.indexA}_{data.indexB}");
        obj.transform.parent = parentObject.transform;
        int connIndex = m_ConnectionImages.Count;
        m_ConnectionImages.Add(obj.AddComponent<RawImage>());
        m_ConnectionImages[connIndex].rectTransform.sizeDelta = size;
        m_ConnectionImages[connIndex].rectTransform.SetLocalPositionAndRotation(pos, Quaternion.identity);
        m_ConnectionTextures.Add(new Texture2D(1, 1));
        m_ConnectionTextures[connIndex].filterMode = FilterMode.Point;
        m_ConnectionTextures[connIndex].alphaIsTransparency = true;
        m_ConnectionTextures[connIndex].SetPixels32(DEFAULT_CONNECTION_TEXTURE_COLOR);
        m_ConnectionTextures[connIndex].Apply();
        m_ConnectionImages[connIndex].texture = m_ConnectionTextures[connIndex];
    }

    private void ClearTextures()
    {
        for (int i = 0; i < MapHelpers.ALL_ROOM_NUM; ++i)
        {
            if (m_RoomTextures[i] == null)
            {
                m_RoomTextures[i] = new(TEXTURE_PIXELS.x, TEXTURE_PIXELS.y)
                {
                    filterMode = FilterMode.Point,
                    alphaIsTransparency = true
                };
            }

            m_RoomTextures[i].SetPixels32(DEFAULT_ROOM_TEXTURE_COLOR);
            m_RoomTextures[i].Apply();
        }
    }

    private void ClearConnections()
    {
        while (m_ConnectionImages.Count > 0)
        {
            Destroy(m_ConnectionImages[0].gameObject);
            m_ConnectionImages.RemoveAt(0);
        }

        m_ConnectionImages.Clear();
        m_ConnectionTextures.Clear();

        RoomsManager.Instance.roomConections.Clear();
    }

    private void Init()
    {
        m_RoomTextures = new Texture2D[MapHelpers.ALL_ROOM_NUM];
        ClearTextures();

        m_RoomImages = new RawImage[MapHelpers.ALL_ROOM_NUM];
        for (int i = 0; i < MapHelpers.ALL_ROOM_NUM; ++i)
        {
            GameObject obj = new($"Room_{i % MapHelpers.MAP_SIZE.x}_{i / MapHelpers.MAP_SIZE.x}");
            obj.transform.parent = parentObject.transform;
            m_RoomImages[i] = obj.AddComponent<RawImage>();
            m_RoomImages[i].rectTransform.sizeDelta = new Vector2(RoomSize.x * parentObject.transform.lossyScale.x, RoomSize.y * parentObject.transform.lossyScale.y);
            m_RoomImages[i].rectTransform.SetLocalPositionAndRotation(MapHelpers.GetWorldPositionFromMapIndex(i, RoomSize, RoomOffset), Quaternion.identity);
            m_RoomImages[i].texture = m_RoomTextures[i];
        }

        m_Rooms = new();
        m_RoomIndexToObjectIndex = new();

        m_ConnectionImages = new();
        m_ConnectionTextures = new();
    }

    private void SpawnRoom(RoomData data, int index)
    {
        if (data.Empty)
        {
            return;
        }

        int connections = MapHelpers.CalculateNumOfConnections(data);
        GameObject toSpawn = null;
        System.Random rand = new();
        uint rot = 0;
        switch (connections)
        {
            case 0:
                {
                    return;
                }
            case 1:
                {
                    toSpawn = OneWayRoom[rand.Next(0, OneWayRoom.Length - 1)];
                    if (data.Right)
                    {
                        rot = 1;
                    }
                    else if (data.Down)
                    {
                        rot = 2;
                    }
                    else if(data.Left)
                    {
                        rot = 3;
                    }
                    break;
                }
            case 2:
                {
                    bool UpDown = data.Up && data.Down;
                    bool LeftRight = data.Right && data.Left;
                    if (UpDown || LeftRight)
                    {
                        toSpawn = TwoWayRoomMirror[rand.Next(0, TwoWayRoomMirror.Length - 1)];
                        if (UpDown)
                        {
                            rot = 1;
                        }
                    }
                    else
                    {
                        toSpawn = TwoWayRoomL[rand.Next(0, TwoWayRoomL.Length - 1)];
                        if (data.Up)
                        {
                            if (data.Left)
                            {
                                rot = 3;
                            }
                        }
                        else if (data.Down)
                        {
                            if (data.Right)
                            {
                                rot = 1;
                            }
                            else
                            {
                                rot = 2;
                            }
                        }
                    }
                    break;
                }
            case 3:
                {
                    toSpawn = ThreeWayRoom[rand.Next(0, ThreeWayRoom.Length - 1)];
                    if (data.Up && data.Right && data.Down)
                    {
                        rot = 1;
                    }
                    else if (data.Down && data.Left && data.Right)
                    {
                        rot = 2;
                    }
                    else if(data.Down && data.Left && data.Up)
                    {
                        rot = 3;
                    }
                    break;
                }
            case 4:
                {
                    toSpawn = FourWayRoom[rand.Next(0, FourWayRoom.Length - 1)];
                    break;
                }
        }

        m_RoomIndexToObjectIndex.Add(index, m_Rooms.Count);
        m_Rooms.Add(Instantiate(toSpawn, Vector3.zero, Quaternion.Euler(0.0f, rot * 90.0f, 0.0f)));
        m_Rooms[^1].SetActive(false);
        m_Rooms[^1].GetComponent<RoomInfo>().Rotate(rot);
        m_Rooms[^1].GetComponent<RoomInfo>().index = index;
    }

    private void ConnectRooms()
    {
        var connections = m_Generator.GetConnections();
        foreach (var connection in connections)
        {
            DrawConnection(connection);

            RoomsManager.RoomConnection con = new();
            con.enterRoom = m_Rooms[m_RoomIndexToObjectIndex[connection.indexA]].GetComponent<RoomInfo>();
            con.exitRoom = m_Rooms[m_RoomIndexToObjectIndex[connection.indexB]].GetComponent<RoomInfo>();
            if (connection.isHorizontal)
            {
                con.type = RoomConnectionType.LeftToRight;
            }
            else
            {
                con.type = RoomConnectionType.TopToBottom;
            }
            RoomsManager.Instance.roomConections.Add(con);
        }
    }

    private void SetVisibile(int index)
    {

    }

    private void SetVisited(int index)
    {

    }

    private void UpdateActive(int index)
    {
        m_RoomImages[activeIndex].color = Color.white;
        UpdateEnd(endIndex);
        m_RoomImages[index].color = Color.red;
        activeIndex = index;
    }

    private void UpdateEnd(int index)
    {
        m_RoomImages[endIndex].color = Color.white;
        m_RoomImages[index].color = Color.yellow;
        endIndex = index;
    }

    private void OnGenerated()
    {
        ClearTextures();
        ClearConnections();
        var map = m_Generator.GetMap();
        m_RoomIndexToObjectIndex.Clear();
        while (m_Rooms.Count != 0)
        {
            Destroy(m_Rooms[0]);
            m_Rooms.RemoveAt(0);
        }
        m_Rooms.Clear();
        m_RoomsVisible.Clear();
        m_RoomsVisited.Clear();

        for (int i = 0; i < map.Length; ++i)
        {
            DrawRoom(map[i], i);
            SpawnRoom(map[i], i);
        }

        ConnectRooms();

        Vector2Int startEnd = m_Generator.GetStartAndEndRoomIndex();
        m_Rooms[m_RoomIndexToObjectIndex[startEnd.x]].SetActive(true);
        UpdateActive(startEnd.x);
        UpdateEnd(startEnd.y);
    }

    private void Awake()
    {
        Init();

        m_Generator = GetComponent<MapGenerator>();
        m_Generator.OnGenerated.AddListener(OnGenerated);
    }

    private void Start()
    {
        RoomsManager.Instance.OnRoomChanged.AddListener(UpdateActive);
    }
}
