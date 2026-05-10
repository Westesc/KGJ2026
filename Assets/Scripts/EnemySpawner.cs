using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class EnemySpawner : MonoBehaviour
{
    public bool spawnEnemies = true;

    public List<GameObject> enemyVariants;
    public BoxCollider spawnArea;

    private bool spawned = false;

    public int enemysNum = 4;

    private int currentEnemiesCount = 0;

    public UnityEvent OnEnemiesDefeated;

    int GetEnemyVariantIdx()
    {
        return Random.Range(0, enemyVariants.Count);
    }

    Vector3 GetRandomSpawnPosition()
    {
        return new Vector3(Random.Range(spawnArea.bounds.min.x, spawnArea.bounds.max.x), 0.0f, Random.Range(spawnArea.bounds.min.z, spawnArea.bounds.max.z));
    }

    public bool Spawn()
    {
        if (!spawnEnemies || spawned) return false;

        for (int i = 0; i < enemysNum; ++i)
        {
            GameObject enemy = Instantiate(enemyVariants[GetEnemyVariantIdx()], transform, true);
            enemy.transform.position = GetRandomSpawnPosition() + spawnArea.transform.position;
            enemy.GetComponent<HealthBar>().OnDeath.AddListener(() => { --currentEnemiesCount; });
        }
        currentEnemiesCount = enemysNum;
        spawned = true;
        return true;
    }

    void Update()
    {
        if (currentEnemiesCount == 0 && spawnEnemies && spawned)
        {
            spawnEnemies = false;
            OnEnemiesDefeated.Invoke();
        }
    }
}
