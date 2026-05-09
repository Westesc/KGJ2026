using UnityEngine;
using UnityEngine.UI;

public class HealthManagement : MonoBehaviour
{
    public Sprite FullHeart;
    public Sprite EmptyHeart;
    public int MaxHealth;
    public int Health;

    void Start()
    {
        MaxHealth = GameObject.FindWithTag("Player").GetComponent<HealthBar>().health;
        Health = MaxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (Health != GameObject.FindWithTag("Player").GetComponent<HealthBar>().health || MaxHealth != this.gameObject.transform.childCount)
        {
            Health = GameObject.FindWithTag("Player").GetComponent<HealthBar>().health;
            while (this.gameObject.transform.GetChild(0) != null)
                Destroy(this.gameObject.transform.GetChild(0));
            for (int i = 0; i < MaxHealth; i++)
            {
                GameObject go = new GameObject("Heart" + i);
                go.transform.parent = this.gameObject.transform;
                go.AddComponent<Image>();
                if (i<Health)
                    go.GetComponent<Image>().sprite = FullHeart;
                else
                    go.GetComponent<Image>().sprite = EmptyHeart;
                if (i != 0)
                {
                    go.transform.localPosition = this.gameObject.transform.GetChild(i - 1).transform.localPosition;
                    go.transform.localPosition += new Vector3(this.gameObject.transform.GetChild(i - 1).GetComponent<RectTransform>().rect.width,0, 0);
                }
            }
        }
    }
}
