using Il2CppSystem.Collections.Generic;
using Photon.Pun;
using VRC;
using VRC.UI;

namespace MerinoClient.Core.VRChat;

public static class PlayerEx
{
    private static Dictionary<int, Photon.Realtime.Player> _playersInRoom;
    public static Dictionary<int, Photon.Realtime.Player> PlayersInRoom
    {
        get
        {
            _playersInRoom = PhotonNetwork.prop_Room_0.field_Private_Dictionary_2_Int32_Player_0;
            return _playersInRoom;
        }
    }

    private static VRCPlayer _vrcPlayer;
    public static VRCPlayer VRCPlayer
    {
        get
        {
            _vrcPlayer = VRCPlayer.field_Internal_Static_VRCPlayer_0;
            return _vrcPlayer;
        }
    }

    private static PlayerManager _playerManager;
    public static PlayerManager PlayerManager
    {
        get
        {
            _playerManager = PlayerManager.field_Private_Static_PlayerManager_0;
            return _playerManager;
        }
    }

    private static PageUserInfo _pageUserInfo;

    public static PageUserInfo PageUserInfo
    {
        get
        {
            var userInfoTransform = VRCUiManagerEx.Instance.MenuContent().transform.Find("Screens/UserInfo");
            _pageUserInfo = userInfoTransform.GetComponent<PageUserInfo>();
            return _pageUserInfo;
        }
    }
}