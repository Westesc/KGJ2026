using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MapGenerator))]
public class MapManager : MonoBehaviour
{
    // STATIC
    private static readonly Vector2Int TEXTURE_PIXELS = new(9, 9);
    private static readonly int ALL_TEXTURE_PIXELS = TEXTURE_PIXELS.x * TEXTURE_PIXELS.y;
    private static readonly Color32 DEFAULT_COLOR_VALUE = new(41, 41, 41, 0);
    private static readonly Color32[] DEFAULT_TEXTURE_COLOR =
    {
        DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE,
        DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE,
        DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE,
        DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE,
        DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE,
        DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE,
        DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE,
        DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE,
        DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE, DEFAULT_COLOR_VALUE
    };

    // DRAW
    private Texture2D[] m_Textures;
    private RawImage[] m_Images;

    [SerializeField] private Vector2 RoomSize = new(50, 50);
    [SerializeField] private Vector2 RoomOffset = new(10, 10);
    [SerializeField] private GameObject parentObject;

    // GENERATOR
    private MapGenerator m_Generator;

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
                m_Textures[index].SetPixel(x, y, Color.gray2);
            }
            else if (!data.Empty && (x - 1 < 0 || x + 1 == TEXTURE_PIXELS.x || y - 1 < 0 || y + 1 == TEXTURE_PIXELS.y))
            {
                m_Textures[index].SetPixel(x, y, Color.cadetBlue);
            }
            else
            {
                m_Textures[index].SetPixel(x, y, Color.gray2);
            }
        }

        m_Textures[index].Apply();
    }

    private void ClearTextures()
    {
        for (int i = 0; i < MapHelpers.ALL_ROOM_NUM; ++i)
        {
            if (m_Textures[i] == null)
            {
                m_Textures[i] = new(TEXTURE_PIXELS.x, TEXTURE_PIXELS.y)
                {
                    filterMode = FilterMode.Point,
                    alphaIsTransparency = true
                };
            }

            m_Textures[i].SetPixels32(DEFAULT_TEXTURE_COLOR);
            m_Textures[i].Apply();
        }
    }

    private void Init()
    {
        m_Textures = new Texture2D[MapHelpers.ALL_ROOM_NUM];
        ClearTextures();

        m_Images = new RawImage[MapHelpers.ALL_ROOM_NUM];
        for (int i = 0; i < MapHelpers.ALL_ROOM_NUM; ++i)
        {
            GameObject obj = new($"Image_{i % MapHelpers.MAP_SIZE.x}_{i / MapHelpers.MAP_SIZE.x}");
            obj.transform.parent = parentObject.transform;
            m_Images[i] = obj.AddComponent<RawImage>();
            m_Images[i].rectTransform.sizeDelta = new Vector2(RoomSize.x * parentObject.transform.lossyScale.x, RoomSize.y * parentObject.transform.lossyScale.y);
            m_Images[i].rectTransform.SetLocalPositionAndRotation(MapHelpers.GetWorldPositionFromMapIndex(i, RoomSize, RoomOffset), Quaternion.identity);
            m_Images[i].texture = m_Textures[i];
        }
    }

    private void OnGenerated()
    {
        ClearTextures();
        var map = m_Generator.GetMap();

        for (int i = 0; i < map.Length; ++i)
        {
            DrawRoom(map[i], i);
        }
    }

    private void Awake()
    {
        Init();
        m_Generator = GetComponent<MapGenerator>();
        m_Generator.OnGenerated.AddListener(OnGenerated);
    }
}
