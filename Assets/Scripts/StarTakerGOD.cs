using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class StarTakerGOD : MonoBehaviour
{
    public StarInventory inventory;

    public void OnStarTaker(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

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
