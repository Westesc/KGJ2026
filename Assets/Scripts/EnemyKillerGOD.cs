using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyKillerGOD : MonoBehaviour
{
    public EnemySpawner spawner;

    void Start()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>().actions["KILL_ALL (K)"].performed += OnKillAll;
    }

    public void OnKillAll(InputAction.CallbackContext ctx)
    {
        spawner.KillAll();

        Debug.Log("KILL");
    }
}
