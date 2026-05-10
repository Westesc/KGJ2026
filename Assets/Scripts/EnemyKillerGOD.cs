using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyKillerGOD : MonoBehaviour
{
    public EnemySpawner spawner;

    public void OnKillAll(InputAction.CallbackContext ctx)
    {
        spawner.KillAll();

        Debug.Log("KILL");
    }
}
