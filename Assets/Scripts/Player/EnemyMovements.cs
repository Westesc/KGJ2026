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
    public Vector2 MovementDirection;
    public float TimeOfStupid;
    public float MaxTimeStupid;

    private void Start()
    {
        PlayerPosition = new Vector3(0, 0, 0);

        GetComponent<HealthBar>().OnDeath.AddListener(() => { Destroy(gameObject); });
    }

    // Update is called once per frame
    void Update()
    {
        if (IsMove)
            EnemyMove();
        EnemyAttack();
        TimeOfStupid += Time.deltaTime;
        if (TimeOfStupid > MaxTimeStupid && enemyType == EnemyType.RangeDealer)
        {
            TimeOfStupid = 0.0f;
            IsMove = !IsMove;
        }

    }

    void EnemyMove()
    {
        MovementDirection = Vector2.zero;
        PlayerPosition = GameObject.FindWithTag("Player").transform.position;
        Vector3 direction = PlayerPosition - this.gameObject.transform.transform.localPosition;
        direction.y = 0;
        if (Vector3.Distance(PlayerPosition, this.gameObject.transform.position) > MeleeDistance && enemyType == EnemyType.MeleeDealer)
        {
            MovementDirection.x = direction.normalized.x * Speed * Time.deltaTime;
            MovementDirection.y = direction.normalized.z * Speed * Time.deltaTime;
            this.gameObject.transform.transform.localPosition += direction.normalized * Speed * Time.deltaTime;
        }
        else if (enemyType == EnemyType.RangeDealer && Vector3.Distance(PlayerPosition, this.gameObject.transform.position) < RangerDistance)
        {
            MovementDirection.x = direction.normalized.x * Speed * Time.deltaTime * (-1);
            MovementDirection.y = direction.normalized.z * Speed * Time.deltaTime * (-1);
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
                this.transform.GetComponentInChildren<EnemyBody>().isAttacking = true;
                var go = Instantiate(AttackPrefab, PlayerPosition, this.gameObject.transform.rotation,this.gameObject.transform);
                go.GetComponent<MeleeAttack>().AttackRadius = 1;
                
            }
            else if (enemyType == EnemyType.RangeDealer)
            {
                var go = Instantiate(AttackPrefab, this.gameObject.transform.position, AttackPrefab.rotation);
            }
        }
    }
}
