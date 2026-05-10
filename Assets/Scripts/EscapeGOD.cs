using SaintsField;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class EscapeGOD : MonoBehaviour
{
    [Scene]
    public string menuScene;

    public void OnEscapeLikeAFool(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        SceneManager.LoadScene(menuScene);
    }
}
