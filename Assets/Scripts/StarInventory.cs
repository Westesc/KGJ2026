using UnityEngine;
using UnityEngine.Events;

public class StarInventory : MonoBehaviour
{
    public bool topCollected;
    public bool leftCollected;
    public bool rightCollected;

    public UnityEvent OnStarCollected;
    public UnityEvent OnAllStarsCollected;

    public void CollectTopStar()
    {
        topCollected = true;
        OnStarCollected?.Invoke();
    }

    public void CollectRightStar()
    {
        rightCollected = true;
        OnStarCollected?.Invoke();
    }

    public void CollectLeftStar()
    {
        leftCollected = true;
        OnStarCollected?.Invoke();
    }

    public void CheckAllStars()
    {
        if (topCollected && leftCollected && rightCollected)
        {
            OnAllStarsCollected?.Invoke();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TopStar"))
        {
            Destroy(other.gameObject);
            CollectTopStar();
        } 
        else if (other.CompareTag("LeftStar"))
        {
            Destroy(other.gameObject);
            CollectLeftStar();
        }
        else if (other.CompareTag("RightStar"))
        {
            Destroy(other.gameObject);
            CollectRightStar();
        }

        CheckAllStars();
    }
}
