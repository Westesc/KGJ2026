using SaintsField;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuRoom : MonoBehaviour
{
    public MenuTrigger playTrigger;
    public MenuTrigger exitTrigger;

    [Scene]
    public string gameLoopScene;

    public void Enter(MenuTrigger trigger)
    {
        if (trigger == playTrigger)
        {
            Play();
            return;
        }

        if (trigger == exitTrigger)
        {
            Exit();
        }
    }

    void Exit()
    {
        Application.Quit();
    }

    void Play()
    {
        SceneManager.LoadScene(gameLoopScene);
    }
}
