using VRC;

namespace MerinoClient.Core.VRChat;

public static class PlayerWrappers
{
    public static VRCPlayer GetVRCplayer()
    {
        return VRCPlayer.field_Internal_Static_VRCPlayer_0;
    }

    public static PlayerManager GetPlayerManager()
    {
        return PlayerManager.field_Private_Static_PlayerManager_0;
    }
}