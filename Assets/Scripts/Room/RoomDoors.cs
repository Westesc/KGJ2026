using DG.Tweening;
using SaintsField.Playa;
using UnityEngine;

public class RoomDoors : MonoBehaviour
{
    public RoomInfo room;
    public bool locked = false;
    private bool interactive = true;
    private GameObject player = null;

    public float branchesZOffset = 0;

    public float lockTime = 2f;
    public float lockedYLocalPosition;
    public float unlockedYLocalPosition;
    public GameObject branches;
    public GameObject lockWall;

    private Sequence lockingSequence = null;

    [Button]
    public void Lock()
    {
        lockingSequence?.Kill();

        float completePercent = (lockedYLocalPosition - branches.transform.localPosition.y) / (lockedYLocalPosition - unlockedYLocalPosition);

        Vector3 desiredPos = new(branches.transform.localPosition.x, lockedYLocalPosition, branches.transform.localPosition.z + completePercent * (lockedYLocalPosition - unlockedYLocalPosition));

        lockingSequence = DOTween.Sequence().AppendCallback(() => { interactive = false; lockWall.SetActive(true); })
            .Append(branches.transform.DOLocalMove(desiredPos, completePercent * lockTime))
            .AppendCallback(() => { interactive = true; lockingSequence = null; locked = true; }).Play();
    }

    [Button]
    public void Unlock()
    {
        lockingSequence?.Kill();

        float completePercent = (unlockedYLocalPosition - branches.transform.localPosition.y) / (unlockedYLocalPosition - lockedYLocalPosition);

        Vector3 desiredPos = new(branches.transform.localPosition.x, unlockedYLocalPosition, branches.transform.localPosition.z + completePercent * (unlockedYLocalPosition - lockedYLocalPosition));

        lockingSequence = DOTween.Sequence().AppendCallback(() => { interactive = false; })
            .Append(branches.transform.DOLocalMove(desiredPos, completePercent * lockTime))
            .AppendCallback(() => { interactive = true; lockingSequence = null; locked = false; lockWall.SetActive(false); }).Play();
    }

    void Start()
    {
        float initBranchLocalY = locked ? lockedYLocalPosition : unlockedYLocalPosition;
        branches.transform.localPosition = new Vector3(branches.transform.localPosition.x, initBranchLocalY, initBranchLocalY - lockedYLocalPosition + branchesZOffset);
        lockWall.SetActive(locked);
    }

    void Update()
    {
        if (!locked && interactive && player != null)
        {
            room.Exit(this);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(RoomsManager.Instance.playerTag))
        {
            player = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(RoomsManager.Instance.playerTag))
        {
            player = null;
        }
    }
}
