using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class PlayerAttack : MonoBehaviour
{
    public Vector3 AttackDirection = Vector2.one;
    private Vector2 RawVectorInput;
    public Transform AttackObj;
    public float restAttcTime;
    public float LastAttcTime;
    public float attackRange;
    public float attackDistance;

    public Texture2D cursorIdle;
    public Texture2D cursorAttack;
    public AudioClip attackSound;

    void Update()
    {
        LastAttcTime += Time.deltaTime;
    }
    public void OnAttack(InputAction.CallbackContext ctx)
    {
        if (LastAttcTime > restAttcTime)
        {
            if (ctx.started)
            {
                Cursor.SetCursor(cursorAttack, Vector2.zero, CursorMode.Auto);
            }
            else if (ctx.canceled)
            {
                Vector3 AttackPosition = this.transform.localPosition + AttackDirection;
                Transform go = Instantiate<Transform>(AttackObj, AttackPosition, this.gameObject.transform.localRotation, this.gameObject.transform);
                go.GetComponent<MeleeAttack>().timeToLandAttack = 0.5f;
                go.GetComponent<MeleeAttack>().AttackRadius = attackRange;
                this.transform.GetComponentInChildren<PlayerBody>().OnAttackEnd.AddListener(() =>
                {
                    Cursor.SetCursor(cursorIdle, Vector2.zero, CursorMode.Auto);
                    this.transform.GetComponentInChildren<PlayerBody>().OnAttackEnd.RemoveAllListeners();
                });
                this.transform.GetComponentInChildren<PlayerBody>().isAttacking = true;
                LastAttcTime = 0;
                AudioSource source = GetComponent<AudioSource>();
                source.clip = attackSound;
                source.Play();
            }
        }
    }

    public void OnLook(InputAction.CallbackContext ctx) 
    {

        RawVectorInput = ctx.ReadValue<Vector2>();
        AttackDirection = new Vector3(RawVectorInput.x- Screen.width/2, 0.0f, RawVectorInput.y- Screen.height/2).normalized * attackDistance;

    }
}
