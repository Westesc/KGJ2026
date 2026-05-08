using UnityEngine;

public class SimpleMovements : MonoBehaviour
{
    private Vector3 direction;
    public int distanceToDelete;
    public int speed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        direction = GameObject.Find("Player").transform.position - this.gameObject.transform.localPosition;
        direction = direction.normalized;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.localPosition += direction * speed * Time.deltaTime;
        if(Vector3.Distance(this.gameObject.transform.localPosition, GameObject.Find("Player").transform.position)> distanceToDelete)
            Destroy(this.gameObject);
    }
}
