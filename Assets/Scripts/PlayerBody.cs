using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class PlayerBody : MonoBehaviour
{
    enum State
    {
        Idle, MoveUp, MoveDown, MoveLeft, MoveRight
    }

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

    private State currentState = State.Idle;
    private State CurrentState
    {
        get => currentState;
        set
        {
            if (value == currentState)
            {
                return;
            }

            if (value == State.Idle)
            {
                StopAnimation();
                spriteRenderer.sprite = idlePose;
                currentState = value;
                return;
            }

            if ((value == State.MoveLeft && currentState == State.MoveRight) || (value == State.MoveRight && currentState == State.MoveLeft))
            {
                spriteRenderer.flipX = !spriteRenderer.flipX;
                currentState = value;
                return;
            }

            spriteRenderer.flipX = false;
            switch (value)
            {
                case State.MoveLeft:
                    PlayAnimation(moveLeftAnim);
                    break;
                case State.MoveRight:
                    spriteRenderer.flipX = true;
                    PlayAnimation(moveLeftAnim);
                    break;
                case State.MoveUp:
                    PlayAnimation(moveUpAnim);
                    break;
                case State.MoveDown:
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

    void UpdateBody()
    {
        if (playerMovement.RawMoveInput == Vector2.zero)
        {
            CurrentState = State.Idle;
            return;
        }

        if (playerMovement.RawMoveInput.y > 0.0f)
        {
            CurrentState = State.MoveUp;
            return;
        }

        if (playerMovement.RawMoveInput.y < 0.0f)
        {
            CurrentState = State.MoveDown;
            return;
        }

        if (playerMovement.RawMoveInput.x > 0.0f)
        {
            CurrentState = State.MoveRight;
            return;
        }

        if (playerMovement.RawMoveInput.x < 0.0f)
        {
            CurrentState = State.MoveLeft;
        }
    }

    void Update()
    {
        UpdateBody();
    }
}
