using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    public float timeToLandAttack = 5;
    public bool isAttacked = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(this.gameObject.transform.parent.tag =="Enemy")
        {
            this.gameObject.transform.parent.GetComponent<EnemyMovements>().IsMove = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        timeToLandAttack -= Time.deltaTime;
        if (timeToLandAttack < 0)
        {
            this.gameObject.AddComponent<SphereCollider>();
            this.gameObject.transform.parent.GetComponent<EnemyMovements>().IsMove = true;
            if (isAttacked && this.gameObject.transform.tag != "Player")
                Destroy( this.gameObject );
            isAttacked = true;
            timeToLandAttack = 0.5f;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player" || collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<HealthBar>().health--;
            Destroy(this.gameObject);
        }
    }
}
