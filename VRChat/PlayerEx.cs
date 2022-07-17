using VRC;
using VRC.UI;

namespace MerinoClient.Core.VRChat;

public static class PlayerEx
{
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