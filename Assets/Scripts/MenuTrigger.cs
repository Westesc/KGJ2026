using UnityEngine;

public class MenuTrigger : MonoBehaviour
{
    public MenuRoom menuRoom;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        menuRoom.Enter(this);
    }
}
