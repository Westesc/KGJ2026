using SaintsField;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class RegenerateGOD : MonoBehaviour
{
    [Scene]
    public string gameScene;

    public void OnRegenerate(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        if (SceneManager.GetActiveScene() != SceneManager.GetSceneByName(gameScene)) { return; }
        GameManager.Instance.Restart();
    } 
}
