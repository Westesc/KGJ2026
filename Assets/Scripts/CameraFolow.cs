using SaintsField;
using UnityEngine;

public class CameraFolow : MonoBehaviour
{
    [Tag]
    public string playerTag;
    private Transform playerTransform;

    [SerializeField] 
    private Vector2 offset;

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag(playerTag).GetComponent<PlayerInfo>().playerTransform;
    }

    void Update()
    {
        transform.position = new Vector3(playerTransform.position.x + offset.x, transform.position.y, playerTransform.position.z + offset.y);
    }
}
