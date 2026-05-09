using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public int health;

    // Update is called once per frame
    void Update()
    {
        if (health == 0)
            Destroy(gameObject);

    }
}
