using SaintsField;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(MapGenerator))]
public class MapManager : MonoBehaviour
{
    struct RoomToConnection
    {
        public int roomIdx;
        public int connIdx;

        public RoomToConnection(int roomIndex, int connIndex)
        {
            this.roomIdx = roomIndex;
            this.connIdx = connIndex;
        }
    };

    enum VisibilityState
    {
        Visible,
        Hidden,
        Visited
    }

    // STATIC
    private static readonly Vector2Int TEXTURE_PIXELS = new(9, 9);
    private static readonly int ALL_TEXTURE_PIXELS = TEXTURE_PIXELS.x * TEXTURE_PIXELS.y;

    // Colors
    [SerializeField] private Color DefaultRoomColor = new(0, 0, 0, 0);
    [SerializeField] private Color DefaultBGColor = Color.gray2;
    [SerializeField] private Color RoomOutlineColor = Color.plum;
    [SerializeField] private Color StarColorBase = Color.blue;
    [SerializeField] private Color StarColorAddition = Color.rebeccaPurple;
    [SerializeField] private Color SpawnColorBase = Color.gray7;
    [SerializeField] private Color InvisibleColor = new(255, 255, 255, 0);
    [SerializeField] private Color BarelyVisibleColor = new(255, 255, 255, 126);
    [SerializeField] private Color VisibleColor = Color.white;
    [SerializeField] private Color ActiveColor = Color.red;

    // DRAW
    private Texture2D[] m_RoomTextures;
    private RawImage[] m_RoomImages;
    private List<VisibilityState> m_RoomsVisible;

    private List<Texture2D> m_ConnectionTextures;
    private List<RawImage> m_ConnectionImages;
    private List<RoomToConnection> m_RoomToConnection;

    [SerializeField] private Vector2 RoomSize = new(50, 50);
    [SerializeField] private Vector2 RoomOffset = new(10, 10);
    [SerializeField] private GameObject ParentObject;

    // ROOMS
    private List<GameObject> m_Rooms;
    private Dictionary<int, int> m_RoomIndexToObjectIndex;

    [SerializeField] private GameObject StartRoom;
    [SerializeField] private GameObject EndRoom;

    [SerializeField] private GameObject[] OneWayRoom;

    [SerializeField] private GameObject[] TwoWayRoomMirror;
    [SerializeField] private GameObject[] TwoWayRoomL;

    [SerializeField] private GameObject[] ThreeWayRoom;

    [SerializeField] private GameObject[] FourWayRoom;

    // GENERATOR
    private MapGenerator m_Generator;

    // ACTIVE
    private int m_ActiveIndex = 0;
    private int m_EndIndex = 0;

    // STARS
    private int m_StarsCollected = 0;
    [SerializeField] private GameObject StarCollectedText;

    // SCENE
    [Scene]
    [SerializeField] private string LastScene;

    private void DrawEnd(int index)
    {
        for (int z = 0; z < ALL_TEXTURE_PIXELS; ++z)
        {
            int x = z % TEXTURE_PIXELS.x;
            int y = z / TEXTURE_PIXELS.x;

            if (((y == 3 || y == 5) && (x == 3 || x == 5)) || (x == 4 && y == 4))
            {
                m_RoomTextures[index].SetPixel(x, y, StarColorAddition);
            }
            else if ((x == 4 && (y >= 2 && y <= 6)) || (y == 4 && (x >= 2 && x <= 6)))
            {
                m_RoomTextures[index].SetPixel(x, y, StarColorBase);
            }
            else if ((x - 1 < 0 || x + 1 == TEXTURE_PIXELS.x || y - 1 < 0 || y + 1 == TEXTURE_PIXELS.y))
            {
                m_RoomTextures[index].SetPixel(x, y, StarColorBase);
            }
        }

        m_RoomTextures[index].Apply();
    }

    private void DrawStart(int index)
    {
        for (int z = 0; z < ALL_TEXTURE_PIXELS; ++z)
        {
            int x = z % TEXTURE_PIXELS.x;
            int y = z / TEXTURE_PIXELS.x;

            if ((y == 6 && x >= 2 && x <= 6) || (y == 5 && x >= 3 && x <= 5) || (x == 4 && y >= 3 && y <= 4))
            {
                m_RoomTextures[index].SetPixel(x, y, SpawnColorBase);
            }
            else if ((x - 1 < 0 || x + 1 == TEXTURE_PIXELS.x || y - 1 < 0 || y + 1 == TEXTURE_PIXELS.y))
            {
                m_RoomTextures[index].SetPixel(x, y, SpawnColorBase);
            }
        }

        m_RoomTextures[index].Apply();
    }

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
                m_RoomTextures[index].SetPixel(x, y, DefaultBGColor);
            }
            else if (!data.Empty && (x - 1 < 0 || x + 1 == TEXTURE_PIXELS.x || y - 1 < 0 || y + 1 == TEXTURE_PIXELS.y))
            {
                m_RoomTextures[index].SetPixel(x, y, RoomOutlineColor);
            }
            else
            {
                m_RoomTextures[index].SetPixel(x, y, DefaultBGColor);
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
            size.x = RoomOffset.x * ParentObject.transform.lossyScale.x;
            size.y = (3f / (float)TEXTURE_PIXELS.y) * RoomSize.y * ParentObject.transform.lossyScale.y;
        }
        else
        {
            pos.y -= (RoomOffset.x + RoomSize.x) * 0.5f;
            size.x = (3f / (float)TEXTURE_PIXELS.x) * RoomSize.x * ParentObject.transform.lossyScale.x;
            size.y = RoomOffset.y * ParentObject.transform.lossyScale.y;
        }

        pos.x *= ParentObject.transform.lossyScale.x;
        pos.y *= ParentObject.transform.lossyScale.y;
        pos.z *= ParentObject.transform.lossyScale.z;

        GameObject obj = new($"Con_{data.indexA}_{data.indexB}");
        obj.transform.SetParent(ParentObject.transform, false);
        int connIndex = m_ConnectionImages.Count;
        m_ConnectionImages.Add(obj.AddComponent<RawImage>());
        m_ConnectionImages[connIndex].rectTransform.sizeDelta = size;
        m_ConnectionImages[connIndex].rectTransform.SetLocalPositionAndRotation(pos, Quaternion.identity);
        m_ConnectionTextures.Add(new Texture2D(1, 1));
        m_ConnectionTextures[connIndex].filterMode = FilterMode.Point;
        m_ConnectionTextures[connIndex].alphaIsTransparency = true;
        m_ConnectionTextures[connIndex].SetPixel(0, 0, DefaultBGColor);
        m_ConnectionTextures[connIndex].Apply();
        m_ConnectionImages[connIndex].texture = m_ConnectionTextures[connIndex];
        m_ConnectionImages[connIndex].color = InvisibleColor;
        m_RoomToConnection.Add(new RoomToConnection(data.indexA, connIndex));
        m_RoomToConnection.Add(new RoomToConnection(data.indexB, connIndex));
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

            MapHelpers.ClearColor(ref m_RoomTextures[i], DefaultRoomColor);
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

    private void StarCollected()
    {
        ++m_StarsCollected;
        StarCollectedText.SetActive(true);
        StarCollectedText.GetComponent<TextMeshProUGUI>().text = $"You've collected {m_StarsCollected} out of 3. Keep being awesome.";

        if (m_StarsCollected < 3)
        {
            StartCoroutine(nameof(StarCollectedWait));
        }
    }

    private IEnumerator StarCollectedWait()
    {
        yield return new WaitForSeconds(0.7f);

        m_Generator.Generate();
    }

    private void AllStarsCollected()
    {
        StartCoroutine(nameof(ChangeScene));
    }

    private IEnumerator ChangeScene()
    {
        yield return new WaitForSeconds(0.6f);

        SceneManager.LoadScene(LastScene);
    }

    private void Init()
    {
        m_RoomTextures = new Texture2D[MapHelpers.ALL_ROOM_NUM];
        ClearTextures();

        m_RoomImages = new RawImage[MapHelpers.ALL_ROOM_NUM];
        for (int i = 0; i < MapHelpers.ALL_ROOM_NUM; ++i)
        {
            GameObject obj = new($"Room_{i % MapHelpers.MAP_SIZE.x}_{i / MapHelpers.MAP_SIZE.x}");
            obj.transform.SetParent(ParentObject.transform, false);
            m_RoomImages[i] = obj.AddComponent<RawImage>();
            m_RoomImages[i].rectTransform.sizeDelta = new Vector2(RoomSize.x * ParentObject.transform.lossyScale.x, RoomSize.y * ParentObject.transform.lossyScale.y);
            Vector3 pos = MapHelpers.GetWorldPositionFromMapIndex(i, RoomSize, RoomOffset);
            pos.x *= ParentObject.transform.lossyScale.x;
            pos.y *= ParentObject.transform.lossyScale.y;
            pos.z *= ParentObject.transform.lossyScale.z;
            m_RoomImages[i].rectTransform.SetLocalPositionAndRotation(pos, Quaternion.identity);
            m_RoomImages[i].texture = m_RoomTextures[i];
        }

        m_Rooms = new();
        m_RoomIndexToObjectIndex = new();
        m_RoomsVisible = new();

        m_ConnectionImages = new();
        m_ConnectionTextures = new();
        m_RoomToConnection = new();

        StarInventory inv = GameObject.FindGameObjectWithTag("Player").GetComponent<StarInventory>();
        inv.OnStarCollected.AddListener(StarCollected);
        inv.OnAllStarsCollected.AddListener(AllStarsCollected);
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

            RoomsManager.RoomConnection con = new()
            {
                enterRoom = m_Rooms[m_RoomIndexToObjectIndex[connection.indexA]].GetComponent<RoomInfo>(),
                exitRoom = m_Rooms[m_RoomIndexToObjectIndex[connection.indexB]].GetComponent<RoomInfo>()
            };

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

    private void ReplaceStartAndEnd(int start, int end)
    {
        int startObj = m_RoomIndexToObjectIndex[start];
        uint rot = m_Rooms[startObj].GetComponent<RoomInfo>().rotation;
        int index = m_Rooms[startObj].GetComponent<RoomInfo>().index;
        Destroy(m_Rooms[startObj]);
        m_Rooms[startObj] = Instantiate(StartRoom, Vector3.zero, Quaternion.Euler(0.0f, rot * 90.0f, 0.0f));
        m_Rooms[startObj].SetActive(false);
        m_Rooms[startObj].GetComponent<RoomInfo>().Rotate(rot);
        m_Rooms[startObj].GetComponent<RoomInfo>().index = index;

        int endObj = m_RoomIndexToObjectIndex[end];
        rot = m_Rooms[endObj].GetComponent<RoomInfo>().rotation;
        index = m_Rooms[endObj].GetComponent<RoomInfo>().index;
        Destroy(m_Rooms[endObj]);
        m_Rooms[endObj] = Instantiate(EndRoom, Vector3.zero, Quaternion.Euler(0.0f, rot * 90.0f, 0.0f));
        m_Rooms[endObj].SetActive(false);
        m_Rooms[endObj].GetComponent<RoomInfo>().Rotate(rot);
        m_Rooms[endObj].GetComponent<RoomInfo>().index = index;

        StarInventory starInfo = GameObject.FindGameObjectWithTag("Player").GetComponent<StarInventory>();
        string tag = "";
        if (!starInfo.rightCollected)
        {
            tag = "RightStar";
            m_StarsCollected = 0;
        }
        else if (!starInfo.topCollected)
        {
            tag = "TopStar";
            m_StarsCollected = 1;
        }
        else if (!starInfo.leftCollected)
        {
            tag = "LeftStar";
            m_StarsCollected = 2;
        }

        var stars = m_Rooms[endObj].GetComponentsInChildren<StarAnim>();
        for (int i = 0; i < stars.Length; ++i)
        {
            if (!stars[i].gameObject.transform.parent.gameObject.CompareTag(tag))
            {
                DestroyImmediate(stars[i].gameObject.transform.parent.gameObject);
            }
        }
    }

    private List<int> GetAllConnectionsForRoom(int index)
    {
        List<int> ret = new();

        for (int i = 0; i < m_RoomToConnection.Count; ++i)
        {
            if (m_RoomToConnection[i].roomIdx == index)
            {
                ret.Add(m_RoomToConnection[i].connIdx);
            }
        }

        return ret;
    }

    private void SetVisibility(int index, VisibilityState state)
    {
        m_RoomsVisible[index] = state;

        List<int> allConnections = GetAllConnectionsForRoom(index);

        switch (state)
        {
            case VisibilityState.Visible:
                {
                    m_RoomImages[index].color = BarelyVisibleColor;
                    for (int i = 0; i < allConnections.Count; ++i)
                    {
                        m_ConnectionImages[allConnections[i]].color = BarelyVisibleColor;
                    }
                    break;
                }
            case VisibilityState.Hidden:
                {
                    m_RoomImages[index].color = InvisibleColor;
                    for (int i = 0; i < allConnections.Count; ++i)
                    {
                        m_ConnectionImages[allConnections[i]].color = InvisibleColor;
                    }
                    break;
                }
            case VisibilityState.Visited:
                {
                    m_RoomImages[index].color = index == m_ActiveIndex ? ActiveColor : VisibleColor;
                    for (int i = 0; i < allConnections.Count; ++i)
                    {
                        m_ConnectionImages[allConnections[i]].color = VisibleColor;
                    }
                    break;
                }
        }
    }

    private Color GetVisibilityColorForRoom(int index)
    {
        switch(m_RoomsVisible[index])
        {
            case VisibilityState.Visible:
                {
                    return BarelyVisibleColor;
                }
            case VisibilityState.Hidden:
                {
                    return InvisibleColor;
                }
            case VisibilityState.Visited:
                { 
                    return index == m_ActiveIndex ? ActiveColor : VisibleColor;
                }
        }

        return InvisibleColor;
    }

    private void UpdateVisibilityOfNeighbourRooms(int index)
    {
        var neighbours = MapHelpers.GetNonEmptyNeighbours(index, m_Generator.GetMap());

        for(int i = 0; i < neighbours.Count; ++i)
        {
            if (m_RoomsVisible[neighbours[i]] == VisibilityState.Hidden)
            {
                SetVisibility(neighbours[i], VisibilityState.Visible);
            }
        }
    }

    private void UpdateActive(int index)
    {
        int oldIndex = m_ActiveIndex;
        UpdateEnd(m_EndIndex);
        m_RoomImages[index].color = ActiveColor;
        m_ActiveIndex = index;
        m_RoomImages[oldIndex].color = GetVisibilityColorForRoom(oldIndex);

        SetVisibility(m_ActiveIndex, VisibilityState.Visited);
        UpdateVisibilityOfNeighbourRooms(m_ActiveIndex);
    }

    private void UpdateEnd(int index)
    {
        m_RoomImages[m_EndIndex].color = GetVisibilityColorForRoom(m_EndIndex);
        DrawEnd(index);
        m_EndIndex = index;
    }

    private void OnGenerated()
    {
        StarCollectedText.SetActive(false);
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
        m_RoomToConnection.Clear();

        for (int i = 0; i < map.Length; ++i)
        {
            DrawRoom(map[i], i);
            SpawnRoom(map[i], i);
            m_RoomsVisible.Add(VisibilityState.Hidden);
            m_RoomImages[i].color = GetVisibilityColorForRoom(i);
        }
        Vector2Int startEnd = m_Generator.GetStartAndEndRoomIndex();
        ReplaceStartAndEnd(startEnd.x, startEnd.y);

        ConnectRooms();

        m_Rooms[m_RoomIndexToObjectIndex[startEnd.x]].SetActive(true);
        DrawStart(startEnd.x);
        UpdateActive(startEnd.x);
        UpdateEnd(startEnd.y);
    }

    private void Awake()
    {
        Init();

        m_Generator = GetComponent<MapGenerator>();
        m_Generator.OnGenerated.AddListener(OnGenerated);

        StarCollectedText.SetActive(false);
    }

    private void Start()
    {
        RoomsManager.Instance.OnRoomExited.AddListener(UpdateActive);
    }
}
