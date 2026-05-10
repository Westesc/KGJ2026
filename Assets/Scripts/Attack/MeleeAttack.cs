using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    public float timeToLandAttack = 5;
    public bool isAttacked = false;
    public float AttackRadius = 0;

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
            this.gameObject.AddComponent<SphereCollider>().isTrigger = true;
            if (AttackRadius != 0)
                this.gameObject.GetComponent<SphereCollider>().radius = AttackRadius;
            if (this.gameObject.transform.parent.tag =="Enemy")
                this.gameObject.transform.parent.GetComponent<EnemyMovements>().IsMove = true;
            if (isAttacked && this.gameObject.transform.tag != "Player")
                Destroy( this.gameObject );
            isAttacked = true;
            timeToLandAttack = 0.5f;
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
            if(collision.gameObject.GetComponent<HealthBar>() != null && collision.gameObject.tag != this.gameObject.transform.parent.tag)
                collision.gameObject.GetComponent<HealthBar>().TakeDamage();
        Destroy(this.gameObject);
    }
}
