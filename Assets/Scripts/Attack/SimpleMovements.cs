using UnityEngine;

public class SimpleMovements : MonoBehaviour
{
    private Vector3 direction;
    public int distanceToDelete;
    public int speed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        direction = GameObject.FindWithTag("Player").transform.position - this.gameObject.transform.localPosition;
        direction = direction.normalized;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.localPosition += direction * speed * Time.deltaTime;
        if(Vector3.Distance(this.gameObject.transform.localPosition, GameObject.FindWithTag("Player").transform.position)> distanceToDelete)
            Destroy(this.gameObject);
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.transform.tag == "Player")
        {
            collision.gameObject.GetComponent<HealthBar>().TakeDamage();
        }
        if(collision.transform.tag != "Enemy")
        Destroy(this.gameObject);
    }
}
