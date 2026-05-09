using DG.Tweening;
using SaintsField.Playa;
using UnityEngine;

public class RoomDoor : MonoBehaviour
{
    [System.Serializable]
    public struct DoorData
    {
        public GameObject handler;
        public float branchesZOffset;
        public GameObject branches;
        public GameObject lockWall;
    }

    public DoorType currentDoorType = DoorType.Left;

    public RoomInfo room;
    public bool locked = false;
    private bool interactive = true;

    public DoorData[] doorTypesData;

    public float lockTime = 2f;
    public float lockedYLocalPosition;
    public float unlockedYLocalPosition;

    private Sequence lockingSequence = null;

    [Button]
    public void Lock()
    {
        lockingSequence?.Kill();

        float completePercent = (lockedYLocalPosition - doorTypesData[(int)currentDoorType].branches.transform.localPosition.y) / (lockedYLocalPosition - unlockedYLocalPosition);

        Vector3 desiredPos = new(doorTypesData[(int)currentDoorType].branches.transform.localPosition.x, lockedYLocalPosition, doorTypesData[(int)currentDoorType].branches.transform.localPosition.z + completePercent * (lockedYLocalPosition - unlockedYLocalPosition));

        lockingSequence = DOTween.Sequence().AppendCallback(() => { interactive = false; doorTypesData[(int)currentDoorType].lockWall.SetActive(true); })
            .Append(doorTypesData[(int)currentDoorType].branches.transform.DOLocalMove(desiredPos, completePercent * lockTime))
            .AppendCallback(() => { interactive = true; lockingSequence = null; locked = true; }).Play();
    }

    [Button]
    public void Unlock()
    {
        lockingSequence?.Kill();

        float completePercent = (unlockedYLocalPosition - doorTypesData[(int)currentDoorType].branches.transform.localPosition.y) / (unlockedYLocalPosition - lockedYLocalPosition);

        Vector3 desiredPos = new(doorTypesData[(int)currentDoorType].branches.transform.localPosition.x, unlockedYLocalPosition, doorTypesData[(int)currentDoorType].branches.transform.localPosition.z + completePercent * (unlockedYLocalPosition - lockedYLocalPosition));

        lockingSequence = DOTween.Sequence().AppendCallback(() => { interactive = false; })
            .Append(doorTypesData[(int)currentDoorType].branches.transform.DOLocalMove(desiredPos, completePercent * lockTime))
            .AppendCallback(() => { interactive = true; lockingSequence = null; locked = false; doorTypesData[(int)currentDoorType].lockWall.SetActive(false); }).Play();
    }

    void Init()
    {
        for (int i = 0; i < doorTypesData.Length; ++i)
        {
            if (i == (int)currentDoorType)
            {
                doorTypesData[i].handler.SetActive(true);

                float initBranchLocalY = locked ? lockedYLocalPosition : unlockedYLocalPosition;
                doorTypesData[i].branches.transform.localPosition = new Vector3(doorTypesData[(int)currentDoorType].branches.transform.localPosition.x, initBranchLocalY, initBranchLocalY - lockedYLocalPosition + doorTypesData[i].branchesZOffset);
                doorTypesData[i].lockWall.SetActive(locked);
            }
            else
            {
                doorTypesData[i].handler.SetActive(false);
            }
        }
    }

    public BoxCollider GetCurrentTrigger()
    {
        return doorTypesData[(int)currentDoorType].handler.transform.GetChild(0).GetComponent<BoxCollider>();
    }

    public void SetDoorType(DoorType doorType)
    {
        lockingSequence?.Kill();

        currentDoorType = doorType;

        Init();
    }

    void Start()
    {
        Init();
    }

    public void Exit()
    {
        if (!locked && interactive)
        {
            room.Exit(this);
        }
    }
}
