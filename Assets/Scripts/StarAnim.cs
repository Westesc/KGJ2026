using DG.Tweening;
using SaintsField.Playa;
using UnityEngine;

public class StarAnim : MonoBehaviour
{
    public float endY;
    public float moveTime;

    private Vector3 startPos;

    private Sequence currentSequence;

    void Start()
    {
        startPos = transform.position;
        InitAnim();
    }

    [Button]
    public void InitAnim()
    {
        transform.position = startPos;

        currentSequence?.Kill();
        currentSequence = DOTween.Sequence();
        currentSequence.Append(transform.DOLocalMoveY(endY, moveTime));
        currentSequence.SetLoops(-1, LoopType.Yoyo);
        currentSequence.Play();
    }

    private void OnDestroy()
    {
        currentSequence?.Kill(true);
    }
}
