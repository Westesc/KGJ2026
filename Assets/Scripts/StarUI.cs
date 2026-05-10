using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class StarUI : MonoBehaviour
{
    private StarInventory inventory;

    public Image topStar;
    public Image leftStar;
    public Image rightStar;

    void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<StarInventory>();
        inventory.OnStarCollected.AddListener(() =>
        {
            OnStarCollected();
        });
    }

    void OnStarCollected()
    {
        if (!topStar.gameObject.activeSelf && inventory.topCollected)
        {
            topStar.gameObject.SetActive(true);
            topStar.rectTransform.DOScale(1.5f, 0.5f).SetLoops(2, LoopType.Yoyo).Play();
        }

        if (!leftStar.gameObject.activeSelf && inventory.leftCollected)
        {
            leftStar.gameObject.SetActive(true);
            leftStar.rectTransform.DOScale(1.5f, 0.5f).SetLoops(2, LoopType.Yoyo).Play();
        }

        if (!rightStar.gameObject.activeSelf && inventory.rightCollected)
        {
            rightStar.gameObject.SetActive(true);
            rightStar.rectTransform.DOScale(1.5f, 0.5f).SetLoops(2, LoopType.Yoyo).Play();
        }
    }
}
