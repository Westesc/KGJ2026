using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public PlayerInfo player;
    public MapGenerator mapGenerator;

    private void Awake()
    {
        if (Instance != null) Destroy(this);
        Instance = this;
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInfo>();
        player.GetComponent<HealthBar>().OnDeath.AddListener(() =>
        {
            Restart();
        });
    }

    public void Restart()
    {
        player.GetComponent<HealthBar>().Resurect();
        player.transform.position = Vector3.zero;
        mapGenerator.Generate();
    }
}
