using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public enum EnemuMoveState
{
    Idle, Move,Attack
}

[DisallowMultipleComponent]
public class EnemyBody : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [System.Serializable]
    public struct Animation
    {
        public List<Sprite> sprites;
        public float timeForSprite;
    }

    [SerializeField]
    private EnemyMovements enemyMovement;

    public SpriteRenderer spriteRenderer;

    public Sprite idlePose;
    public Animation moveAnim;
    public Animation attackAnim;
    public bool isAttacking = false;
    public bool isMoving = true;

    private Sequence currentAnimSequence = null;

    private EnemuMoveState currentState = EnemuMoveState.Idle;
    private EnemuMoveState CurrentState
    {
        get => currentState;
        set
        {
            if (value == currentState)
            {
                return;
            }

            if (value == EnemuMoveState.Idle)
            {
                StopAnimation();
                spriteRenderer.sprite = idlePose;
                currentState = value;
                return;
            }

            switch (value)
            {
                case EnemuMoveState.Move:
                    PlayAnimation(moveAnim);
                    break;
                case EnemuMoveState.Attack:
                    PlayAnimation(attackAnim, false, () => { isAttacking = false; isMoving = true; });
                    break;
            }
            currentState = value;
        }
    }

    void Start()
    {
        enemyMovement = this.transform.parent.GetComponent<EnemyMovements>();
    }

    void PlayAnimation(Animation anim, bool hasLoops = true, TweenCallback onCycleEnd = null)
    {
        currentAnimSequence?.Kill();

        currentAnimSequence = DOTween.Sequence();
        foreach (var sprite in anim.sprites)
        {
            currentAnimSequence.AppendCallback(() => { spriteRenderer.sprite = sprite; }).AppendInterval(anim.timeForSprite);
        }
        if (onCycleEnd != null)
        {
            currentAnimSequence.AppendCallback(onCycleEnd);
        }
        if (hasLoops)
        {
            currentAnimSequence.SetLoops(-1);
        }
        currentAnimSequence.Play();
    }

    void StopAnimation()
    {
        currentAnimSequence.Kill();
        currentAnimSequence = null;
    }

    public void PlayIdleAnim()
    {
        CurrentState = EnemuMoveState.Idle;
    }

    public void PlayMoveAnim()
    {
        CurrentState = EnemuMoveState.Move;
    }
    public void PlayAttackAnim()
    {
        CurrentState = EnemuMoveState.Attack;
    }

    void UpdateBody()
    {
        if (isAttacking)
        {
            PlayAttackAnim();
            isMoving = false;
            return;
        }
        if (enemyMovement.MovementDirection == Vector2.zero)
        {
            PlayIdleAnim();
            return;
        }

        if (enemyMovement.MovementDirection.y != 0.0f || enemyMovement.MovementDirection.x != 0.0f)
        {
            PlayMoveAnim();
            return;
        }
    }

    void Update()
    {
        isMoving = enemyMovement.IsMove;
        if (isMoving)
            UpdateBody();
    }

    private void OnDestroy()
    {
        currentAnimSequence?.Kill(true);
    }
}
