using UnityEngine;
using UnityEngine.Events;

public class StarInventory : MonoBehaviour
{
    public bool topCollected;
    public bool leftCollected;
    public bool rightCollected;

    public UnityEvent OnStarCollected;
    public UnityEvent OnAllStarsCollected;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TopStar"))
        {
            Destroy(other.gameObject);
            topCollected = true;
            OnStarCollected?.Invoke();
        } 
        else if (other.CompareTag("LeftStar"))
        {
            Destroy(other.gameObject);
            leftCollected = true;
            OnStarCollected?.Invoke();
        }
        else if (other.CompareTag("RightStar"))
        {
            Destroy(other.gameObject);
            rightCollected = true;
            OnStarCollected?.Invoke();
        }

        if (topCollected && leftCollected && rightCollected)
        {
            OnAllStarsCollected?.Invoke();
        }
    }
}
