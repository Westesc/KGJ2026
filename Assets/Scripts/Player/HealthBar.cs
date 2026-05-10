using SaintsField.Playa;
using UnityEngine;
using UnityEngine.Events;

public class HealthBar : MonoBehaviour
{
    public bool immortal = false;

    public int health;
    private int LastHealth;
    private float timeColor;
    private bool dead = false;
    private int maxHealth;

    public UnityEvent OnDeath;

    private void Start()
    {
        LastHealth = health;
        maxHealth = health;
    }

    // Update is called once per frame
    void Update()
    {
        if(this.transform.GetComponentInChildren<SpriteRenderer>().color == Color.red)
        {
            timeColor += Time.deltaTime;
            if (timeColor > 0.75) 
                this.transform.GetComponentInChildren<SpriteRenderer>().color = Color.white;

        }
        if(health <LastHealth)
        {
            timeColor = 0;
            LastHealth = health;
            this.transform.GetComponentInChildren<SpriteRenderer>().color = Color.red;

        }
        if (health == 0 && !dead)
        {
            dead = true;
            OnDeath?.Invoke();
        }
    }

    public void Resurect()
    {
        health = maxHealth;
        LastHealth = health;
        dead = false;
    }

    [Button]
    public void Die()
    {
        health = 0;
    }

    public void TakeDamage(int damage = 1)
    {
        if (immortal) return;
        health -= damage;
    }
}
