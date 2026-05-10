using DG.Tweening;
using SaintsField;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class StarTakerGOD : MonoBehaviour
{
    public StarInventory inventory;
    [Scene]
    public string gameScene;

    public void OnStarTaker(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        if (SceneManager.GetActiveScene() != SceneManager.GetSceneByName(gameScene)) { return; }

        Sequence seq = DOTween.Sequence().AppendCallback(() =>
        {
            inventory.CollectTopStar();
        }).AppendInterval(1).AppendCallback(() =>
        {
            inventory.CollectRightStar();
        }).AppendInterval(1).AppendCallback(() =>
        {
            inventory.CollectLeftStar();
        }).AppendInterval(1).AppendCallback(() =>
        {
            inventory.CheckAllStars();
        }).Play();
    }
}
