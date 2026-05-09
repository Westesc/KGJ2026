using DG.Tweening;
using SaintsField;
using System.Collections.Generic;
using UnityEngine;

public enum RoomConnectionType
{
    TopToBottom, BottomToTop, LeftToRight, RightToLeft
}

[DisallowMultipleComponent]
public class RoomsManager : MonoBehaviour
{
    public static RoomsManager Instance { get; private set; }

    [System.Serializable]
    public struct RoomConnection
    {
        public RoomConnectionType type;
        public RoomInfo enterRoom;
        public RoomInfo exitRoom;
    }

    [Tag]
    public string playerTag = "Player";

    public float transitionTime = 4f;

    public List<RoomConnection> roomConections = new();

    private Sequence currentSequence = null;

    void Awake()
    {
        if (Instance != null) Destroy(this);
        Instance = this;
    }

    public void MakeTransitionAnimation(RoomInfo exitRoom, RoomInfo enterRoom, DoorType exitDoor)
    {
        if (currentSequence != null)
        {
            return;
        }

        PlayerInfo playerInfo = GameObject.FindGameObjectWithTag(playerTag).GetComponent<PlayerInfo>();

        Sequence seq = DOTween.Sequence();

        // EXIT
        seq.AppendCallback(() =>
        {
            playerInfo.playerMovement.enabled = false;
            playerInfo.playerBody.enabled = false;
        });
        
        switch (exitDoor)
        {
            case DoorType.Top:
                seq.AppendCallback(() => {
                    exitRoom.topDoors.enabled = false;
                    playerInfo.playerBody.PlayMoveUpAnim();
                });
                seq.Append(playerInfo.playerTransform.DOMoveZ(exitRoom.topDoors.transform.position.z, transitionTime / 2f));
                break;
            case DoorType.Bottom:
                seq.AppendCallback(() =>
                {
                    exitRoom.bottomDoors.enabled = false;
                    playerInfo.playerBody.PlayMoveDownAnim();
                });
                seq.Append(playerInfo.playerTransform.DOMoveZ(exitRoom.bottomDoors.transform.position.z, transitionTime / 2f));
                break;
            case DoorType.Left:
                seq.AppendCallback(() => {
                    exitRoom.leftDoors.enabled = false;
                    playerInfo.playerBody.PlayMoveLeftAnim();
                });
                seq.Append(playerInfo.playerTransform.DOMoveX(exitRoom.leftDoors.transform.position.x, transitionTime / 2f));
                break;
            case DoorType.Right:
                seq.AppendCallback(() => {
                    exitRoom.rightDoors.enabled = false;
                    playerInfo.playerBody.PlayMoveRightAnim();
                });
                seq.Append(playerInfo.playerTransform.DOMoveX(exitRoom.rightDoors.transform.position.x, transitionTime / 2f));
                break;
        }

        seq.AppendCallback(() => {
            exitRoom.gameObject.SetActive(false);
            enterRoom.gameObject.SetActive(true);
        });

        // ENTER
        switch (exitDoor)
        {
            case DoorType.Top:
                seq.AppendCallback(() => {
                    exitRoom.topDoors.enabled = true;
                    enterRoom.bottomDoors.enabled = false;
                    playerInfo.playerTransform.position = enterRoom.bottomDoors.transform.position;
                });
                seq.Append(playerInfo.playerTransform.DOMoveZ(
                    enterRoom.bottomDoors.transform.position.z + (enterRoom.bottomDoors.GetComponent<BoxCollider>().size.z + 1), transitionTime / 2f));
                seq.AppendCallback(() => { enterRoom.bottomDoors.enabled = true; });
                break;
            case DoorType.Bottom:
                seq.AppendCallback(() => {
                    exitRoom.bottomDoors.enabled = true;
                    enterRoom.topDoors.enabled = false;
                    playerInfo.playerTransform.position = enterRoom.topDoors.transform.position;
                });
                seq.Append(playerInfo.playerTransform.DOMoveZ(
                    enterRoom.topDoors.transform.position.z - (enterRoom.topDoors.GetComponent<BoxCollider>().size.z + 1), transitionTime / 2f));
                seq.AppendCallback(() => { enterRoom.topDoors.enabled = true; });
                break;
            case DoorType.Left:
                seq.AppendCallback(() => {
                    exitRoom.leftDoors.enabled = true;
                    enterRoom.rightDoors.enabled = false;
                    playerInfo.playerTransform.position = enterRoom.rightDoors.transform.position;
                });
                seq.Append(playerInfo.playerTransform.DOMoveX(
                    enterRoom.rightDoors.transform.position.x - (enterRoom.rightDoors.GetComponent<BoxCollider>().size.x + 1), transitionTime / 2f));
                seq.AppendCallback(() => { enterRoom.rightDoors.enabled = true; });
                break;
            case DoorType.Right:
                seq.AppendCallback(() => {
                    exitRoom.rightDoors.enabled = true;
                    enterRoom.leftDoors.enabled = false;
                    playerInfo.playerTransform.position = enterRoom.leftDoors.transform.position;
                });
                seq.Append(playerInfo.playerTransform.DOMoveX(
                    enterRoom.leftDoors.transform.position.x + (enterRoom.leftDoors.GetComponent<BoxCollider>().size.x + 1), transitionTime / 2f));
                seq.AppendCallback(() => { enterRoom.leftDoors.enabled = true; });
                break;
        }

        currentSequence = seq.AppendCallback(() =>
        {
            playerInfo.playerMovement.enabled = true;
            playerInfo.playerBody.enabled = true;
        }).OnComplete(() => { currentSequence = null; }).Play();
    }

    public void ExitRoom(RoomInfo room, DoorType type)
    {
        foreach (var connection in roomConections)
        {
            if (connection.enterRoom != room && connection.exitRoom != room)
            {
                continue;
            }

            if (connection.enterRoom == room)
            {
                switch (connection.type)
                {
                    case RoomConnectionType.TopToBottom:
                        if (type != DoorType.Top) continue;
                        break;
                    case RoomConnectionType.BottomToTop:
                        if (type != DoorType.Bottom) continue;
                        break;
                    case RoomConnectionType.LeftToRight:
                        if (type != DoorType.Left) continue;
                        break;
                    case RoomConnectionType.RightToLeft:
                        if (type != DoorType.Right) continue;
                        break;
                }
                MakeTransitionAnimation(room, connection.exitRoom, type);
                return;
            }

            // room == exitRoom
            switch (connection.type)
            {
                case RoomConnectionType.TopToBottom:
                    if (type != DoorType.Bottom) continue;
                    break;
                case RoomConnectionType.BottomToTop:
                    if (type != DoorType.Top) continue;
                    break;
                case RoomConnectionType.LeftToRight:
                    if (type != DoorType.Right) continue;
                    break;
                case RoomConnectionType.RightToLeft:
                    if (type != DoorType.Left) continue;
                    break;
            }
            MakeTransitionAnimation(room, connection.enterRoom, type);
        }
    }
}
