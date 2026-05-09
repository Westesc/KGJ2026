using System;
using UnityEngine;



public class EnemyMovements : MonoBehaviour
{
    public EnemyType enemyType;
    public enum EnemyType
    {
        MeleeDealer,
        RangeDealer
    };
    public Vector3 PlayerPosition;
    public int RangerDistance;
    public int MeleeDistance;
    public float Speed = 10.0f;
    public int MeleeAttackDistance;
    public float TimeToAttack;
    private float TimeFromLastAttack;
    public Transform AttackPrefab;
    public bool IsMove = true;

    private void Start()
    {
        PlayerPosition = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (IsMove)
            EnemyMove();
        EnemyAttack();

    }

    void EnemyMove()
    {
        PlayerPosition = GameObject.FindWithTag("Player").transform.position;
        Vector3 direction = PlayerPosition - this.gameObject.transform.transform.localPosition;
        if (Vector3.Distance(PlayerPosition, this.gameObject.transform.position) > MeleeDistance && enemyType == EnemyType.MeleeDealer)
        {
            this.gameObject.transform.transform.localPosition += direction.normalized * Speed * Time.deltaTime;
        }
        else if (enemyType == EnemyType.RangeDealer && Vector3.Distance(PlayerPosition, this.gameObject.transform.position) < RangerDistance)
        {
            this.gameObject.transform.transform.localPosition -= direction.normalized * Speed * Time.deltaTime;
        }
    }
    
    void EnemyAttack()
    {
        TimeFromLastAttack += Time.deltaTime;
        if (TimeFromLastAttack > TimeToAttack)
        {
            TimeFromLastAttack = 0.0f;
            if (enemyType == EnemyType.MeleeDealer && Vector3.Distance(PlayerPosition, this.gameObject.transform.position) < MeleeAttackDistance)
            {
                var go = Instantiate(AttackPrefab, PlayerPosition, this.gameObject.transform.rotation,this.gameObject.transform);
                
            }
            else if (enemyType == EnemyType.RangeDealer)
            {
                var go = Instantiate(AttackPrefab, this.gameObject.transform.position,this.gameObject.transform.rotation);
            }
        }
    }
}
