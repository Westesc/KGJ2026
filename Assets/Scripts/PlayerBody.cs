using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public enum PlayerMoveState
{
    Idle, MoveUp, MoveDown, MoveLeft, MoveRight
}

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

    private Sequence currentAnimSequence = null;

    private PlayerMoveState currentState = PlayerMoveState.Idle;
    private PlayerMoveState CurrentState
    {
        get => currentState;
        set
        {
            if (value == currentState)
            {
                return;
            }

            if (value == PlayerMoveState.Idle)
            {
                StopAnimation();
                spriteRenderer.sprite = idlePose;
                currentState = value;
                return;
            }

            if ((value == PlayerMoveState.MoveLeft && currentState == PlayerMoveState.MoveRight) || (value == PlayerMoveState.MoveRight && currentState == PlayerMoveState.MoveLeft))
            {
                spriteRenderer.flipX = !spriteRenderer.flipX;
                currentState = value;
                return;
            }

            spriteRenderer.flipX = false;
            switch (value)
            {
                case PlayerMoveState.MoveLeft:
                    PlayAnimation(moveLeftAnim);
                    break;
                case PlayerMoveState.MoveRight:
                    spriteRenderer.flipX = true;
                    PlayAnimation(moveLeftAnim);
                    break;
                case PlayerMoveState.MoveUp:
                    PlayAnimation(moveUpAnim);
                    break;
                case PlayerMoveState.MoveDown:
                    PlayAnimation(moveDownAnim);
                    break;
            }
            currentState = value;
        }
    }

    void Start()
    {
        playerMovement = playerInfo.playerMovement;    
    }

    void PlayAnimation(Animation anim)
    {
        currentAnimSequence?.Kill();

        currentAnimSequence = DOTween.Sequence();
        foreach (var sprite in anim.sprites)
        {
            currentAnimSequence.AppendCallback(() => { spriteRenderer.sprite = sprite; }).AppendInterval(anim.timeForSprite);
        }
        currentAnimSequence.SetLoops(-1);
        currentAnimSequence.Play();
    }

    void StopAnimation()
    {
        currentAnimSequence.Kill();
        currentAnimSequence = null;
    }

    public void PlayIdleAnim()
    {
        CurrentState = PlayerMoveState.Idle;
    }

    public void PlayMoveUpAnim()
    {
        CurrentState = PlayerMoveState.MoveUp;
    }

    public void PlayMoveDownAnim()
    {
        CurrentState = PlayerMoveState.MoveDown;
    }

    public void PlayMoveRightAnim()
    {
        CurrentState = PlayerMoveState.MoveRight;
    }

    public void PlayMoveLeftAnim()
    {
        CurrentState = PlayerMoveState.MoveLeft;
    }

    void UpdateBody()
    {
        if (playerMovement.RawMoveInput == Vector2.zero)
        {
            PlayIdleAnim();
            return;
        }

        if (playerMovement.RawMoveInput.y > 0.0f)
        {
            PlayMoveUpAnim();
            return;
        }

        if (playerMovement.RawMoveInput.y < 0.0f)
        {
            PlayMoveDownAnim();
            return;
        }

        if (playerMovement.RawMoveInput.x > 0.0f)
        {
            PlayMoveRightAnim();
            return;
        }

        if (playerMovement.RawMoveInput.x < 0.0f)
        {
            PlayMoveLeftAnim();
        }
    }

    void Update()
    {
        UpdateBody();
    }
}
