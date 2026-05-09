using UnityEngine;

public class CollisionTester : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Entered collision with {collision}");
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Entered trigger with {other}");
    }
}
