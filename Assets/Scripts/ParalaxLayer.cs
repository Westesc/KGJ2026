using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ParalaxLayer : MonoBehaviour
{
    private float length, startPos;

    public GameObject playerCamera;
    [Range(0.0f, 1.0f)]
    public float parallaxEffect;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void Update()
    {
        float temp = playerCamera.transform.position.x * (1.0f - parallaxEffect);
        float dist = playerCamera.transform.position.x * parallaxEffect;

        transform.position = new Vector3(startPos + dist, transform.position.y, transform.position.z);

        if (temp > startPos + length) startPos += length;
        else if (temp < startPos - length) startPos -= length;
    }
}
