using UnityEngine;
using UnityEngine.UI;

public class FloatingHealthBar : MonoBehaviour
{
    [SerializeField] private Slider m_Slider;

    public HealthBar healthBar;

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(healthBar.health.ToString() + "  " + healthBar.maxHealth.ToString());
        m_Slider.value = (float)healthBar.health / healthBar.maxHealth;
    }
}
