using System.Collections.Generic;
using UnityEngine;
using SaintsField.Playa;
using UnityEngine.Events;

public class EnemySpawner : MonoBehaviour
{
    public bool spawnEnemies = true;

    public List<GameObject> enemyVariants;
    public BoxCollider spawnArea;

    private bool spawned = false;

    public int enemysNum = 4;

    private List<GameObject> currentEnemies;

    public UnityEvent OnEnemiesDefeated;

    private void Start()
    {
        currentEnemies.Clear();
    }

    int GetEnemyVariantIdx()
    {
        return Random.Range(0, enemyVariants.Count);
    }

    Vector3 GetRandomSpawnPosition()
    {
        return new Vector3(Random.Range(spawnArea.bounds.min.x, spawnArea.bounds.max.x), 0.0f, Random.Range(spawnArea.bounds.min.z, spawnArea.bounds.max.z));
    }

    [Button]
    public bool Spawn()
    {
        if (!spawnEnemies || spawned) return false;

        Debug.Log(spawnArea.bounds.min + " " + spawnArea.bounds.max);

        currentEnemies = new();

        for (int i = 0; i < enemysNum; ++i)
        {
            GameObject enemy = Instantiate(enemyVariants[GetEnemyVariantIdx()], transform, true);
            enemy.transform.position = GetRandomSpawnPosition();
            enemy.GetComponent<HealthBar>().OnDeath.AddListener(() => {
                currentEnemies.Remove(enemy);
            });

            currentEnemies.Add(enemy);
        }
        spawned = true;
        return true;
    }

    public void KillAll()
    {
        for (int i = 0; i < currentEnemies.Count; ++i)
        {
            currentEnemies[i].GetComponent<HealthBar>().Die();
        }
    }

    void Update()
    {
        if (currentEnemies != null && currentEnemies.Count == 0 && spawnEnemies && spawned)
        {
            spawnEnemies = false;
            OnEnemiesDefeated.Invoke();
        }
    }
}
