using SaintsField;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class EnemyKillerGOD : MonoBehaviour
{
    public EnemySpawner spawner;

    void Start()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>().actions["KILL_ALL (K)"].performed += OnKillAll;
    }

    public void OnKillAll(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        
        spawner.KillAll();
        Debug.Log("KILL");
    }

    private void OnDestroy()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;
        player.GetComponent<PlayerInput>().actions["KILL_ALL (K)"].performed -= OnKillAll;
    }
}
