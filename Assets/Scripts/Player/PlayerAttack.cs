using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    public Vector3 AttackDirection = Vector2.one;
    private Vector2 RawVectorInput;
    public Transform AttackObj;
    public float restAttcTime;
    public float LastAttcTime;
    public float attackRange;
    public float attackDistance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        LastAttcTime += Time.deltaTime;
    }
    public void OnAttack(InputAction.CallbackContext ctx)
    {
        if (LastAttcTime > restAttcTime)
        {
            Vector3 AttackPosition = this.transform.localPosition + AttackDirection;
            Transform go = Instantiate<Transform>(AttackObj, AttackPosition, this.gameObject.transform.localRotation, this.gameObject.transform);
            go.GetComponent<MeleeAttack>().timeToLandAttack = 1;
            go.GetComponent<MeleeAttack>().AttackRadius = attackRange;
            LastAttcTime = 0;
        }
    }

    public void OnLook(InputAction.CallbackContext ctx) 
    {

        RawVectorInput = ctx.ReadValue<Vector2>();
        AttackDirection = new Vector3(RawVectorInput.x- Screen.width/2, 0.0f, RawVectorInput.y- Screen.height/2).normalized * attackDistance;

    }
}
