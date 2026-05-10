using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public enum PlayerState
{
    Idle, Move, Attack, Dash
}

public enum PlayerDirection
{
    Up, Down, Left, Right
}

[DisallowMultipleComponent]
public class PlayerBody : MonoBehaviour
{
    [System.Serializable]
    public struct Animation
    {
        public List<Sprite> sprites;
        public float timeForSprite;
    }

    [SerializeField]
    private PlayerInfo playerInfo;
    private PlayerMovement playerMovement;

    public SpriteRenderer spriteRenderer;

    public Sprite idlePose;
    public Animation moveUpAnim;
    public Animation moveDownAnim;
    public Animation moveLeftAnim;
    public Animation attackLeftAnim;
    public Animation dashUpAnim;
    public Animation dashDownAnim;
    public Animation dashLeftAnim;
    public bool isAttacking = false;
    public bool isMoving = true;
    public bool isDashing = false;

    private Sequence currentAnimSequence = null;

    private PlayerState currentState = PlayerState.Idle;
    private PlayerState CurrentState
    {
        get => currentState;
        set
        {
            if (value == currentState)
            {
                return;
            }

            var lastState = currentState;
            currentState = value;
            UpdateState(lastState, currentDirection);
        }
    }

    private PlayerDirection currentDirection = PlayerDirection.Down;
    private PlayerDirection CurrentDirection
    {
        get => currentDirection;
        set
        {
            if (value == currentDirection)
            {
                return;
            }

            var lastDirection = currentDirection;
            currentDirection = value;
            UpdateState(currentState, lastDirection);
        }
    }

    void ChangeStateAndDirection(PlayerState state, PlayerDirection direction)
    {
        if (currentState == state && currentDirection == direction)
        {
            return;
        }

        var lastState = currentState;
        var lastDirection = currentDirection;

        currentState = state;
        currentDirection = direction;

        UpdateState(lastState, lastDirection);
    }

    void UpdateAttackState()
    {
        spriteRenderer.flipX = false;
        switch (currentDirection)
        {
            case PlayerDirection.Left:
                PlayAnimation(attackLeftAnim, false, () => { isAttacking = false; isMoving = true; });
                break;
            case PlayerDirection.Right:
                spriteRenderer.flipX = true;
                PlayAnimation(attackLeftAnim, false, () => { isAttacking = false; isMoving = true; });
                break;
        }
    }

    void UpdateMoveState(PlayerDirection lastDirection)
    {
        if ((currentDirection == PlayerDirection.Left && lastDirection == PlayerDirection.Right) || (currentDirection == PlayerDirection.Right && lastDirection == PlayerDirection.Left))
        {
            spriteRenderer.flipX = !spriteRenderer.flipX;
            return;
        }

        spriteRenderer.flipX = false;
        switch (currentDirection)
        {
            case PlayerDirection.Left:
                PlayAnimation(moveLeftAnim);
                break;
            case PlayerDirection.Right:
                spriteRenderer.flipX = true;
                PlayAnimation(moveLeftAnim);
                break;
            case PlayerDirection.Up:
                PlayAnimation(moveUpAnim);
                break;
            case PlayerDirection.Down:
                PlayAnimation(moveDownAnim);
                break;
        }
    }

    void UpdateDashState(PlayerDirection lastDirection)
    {
        if (isDashing)
        {
            return;
        }

        isDashing = true;

        if ((currentDirection == PlayerDirection.Left && lastDirection == PlayerDirection.Right) || (currentDirection == PlayerDirection.Right && lastDirection == PlayerDirection.Left))
        {
            spriteRenderer.flipX = !spriteRenderer.flipX;
            return;
        }

        spriteRenderer.flipX = false;
        switch (currentDirection)
        {
            case PlayerDirection.Left:
                PlayAnimation(dashLeftAnim, false, () => { isDashing = false; });
                break;
            case PlayerDirection.Right:
                spriteRenderer.flipX = true;
                PlayAnimation(dashLeftAnim, false, () => { isDashing = false; });
                break;
            case PlayerDirection.Up:
                PlayAnimation(dashUpAnim, false, () => { isDashing = false; });
                break;
            case PlayerDirection.Down:
                PlayAnimation(dashDownAnim, false, () => { isDashing = false; });
                break;
        }
    }

    void UpdateState(PlayerState lastState, PlayerDirection lastDirection)
    {
        if (currentState == PlayerState.Idle)
        {
            StopAnimation();
            spriteRenderer.sprite = idlePose;
            return;
        }

        if (currentState == PlayerState.Dash)
        {
            UpdateDashState(lastDirection);
            return;
        }

        if (currentState == PlayerState.Move)
        {
            UpdateMoveState(lastDirection);
            return;
        }

        if (currentState == PlayerState.Attack)
        {
            UpdateAttackState();
            return;
        }
    }

    void Start()
    {
        playerMovement = playerInfo.playerMovement;    
    }

    void PlayAnimation(Animation anim, bool hasLoops = true, TweenCallback onComplete = null)
    {
        currentAnimSequence?.Kill(true);

        currentAnimSequence = DOTween.Sequence();
        foreach (var sprite in anim.sprites)
        {
            currentAnimSequence.AppendCallback(() => { spriteRenderer.sprite = sprite; }).AppendInterval(anim.timeForSprite);
        }
        if (onComplete != null)
        {
            currentAnimSequence.OnComplete(onComplete);
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
        ChangeStateAndDirection(PlayerState.Idle, PlayerDirection.Down);
    }

    public void PlayAttackLeftAnim()
    {
        ChangeStateAndDirection(PlayerState.Attack, PlayerDirection.Left);
    }

    public void PlayAttackRightAnim()
    {
        ChangeStateAndDirection(PlayerState.Attack, PlayerDirection.Right);
    }

    public void PlayDashUpAnim()
    {
        ChangeStateAndDirection(PlayerState.Dash, PlayerDirection.Up);
    }

    public void PlayDashDownAnim()
    {
        ChangeStateAndDirection(PlayerState.Dash, PlayerDirection.Down);
    }

    public void PlayDashRightAnim()
    {
        ChangeStateAndDirection(PlayerState.Dash, PlayerDirection.Right);
    }

    public void PlayDashLeftAnim()
    {
        ChangeStateAndDirection(PlayerState.Dash, PlayerDirection.Left);
    }

    public void PlayMoveUpAnim()
    {
        ChangeStateAndDirection(PlayerState.Move, PlayerDirection.Up);
    }

    public void PlayMoveDownAnim()
    {
        ChangeStateAndDirection(PlayerState.Move, PlayerDirection.Down);
    }

    public void PlayMoveRightAnim()
    {
        ChangeStateAndDirection(PlayerState.Move, PlayerDirection.Right);
    }

    public void PlayMoveLeftAnim()
    {
        ChangeStateAndDirection(PlayerState.Move, PlayerDirection.Left);
    }

    void UpdateBody()
    {   
        if (isAttacking)
        {
            if(this.transform.parent.GetComponentInChildren<PlayerAttack>().AttackDirection.x > 0 )
            {
                PlayAttackRightAnim();
            }
            else
            {
                PlayAttackLeftAnim();
            }
            isMoving = false;
            return;
        }

        if (playerMovement.RawMoveInput == Vector2.zero)
        {
            if (playerMovement.Dash)
            {
                PlayDashDownAnim();
                return;
            }
            PlayIdleAnim();
            return;
        }

        if (playerMovement.RawMoveInput.y > 0.0f)
        {
            if (playerMovement.Dash)
            {
                PlayDashUpAnim();
                return;
            }
            PlayMoveUpAnim();
            return;
        }

        if (playerMovement.RawMoveInput.y < 0.0f)
        {
            if (playerMovement.Dash)
            {
                PlayDashDownAnim();
                return;
            }
            PlayMoveDownAnim();
            return;
        }

        if (playerMovement.RawMoveInput.x > 0.0f)
        {
            if (playerMovement.Dash)
            {
                PlayDashRightAnim();
                return;
            }
            PlayMoveRightAnim();
            return;
        }

        if (playerMovement.RawMoveInput.x < 0.0f)
        {
            if (playerMovement.Dash)
            {
                PlayDashLeftAnim();
                return;
            }
            PlayMoveLeftAnim();
        }
    }

    void Update()
    {
        if (isDashing)
        {
            return;
        }

        if (isMoving) UpdateBody();
    }
}
