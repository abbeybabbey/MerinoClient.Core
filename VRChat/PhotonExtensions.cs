using ExitGames.Client.Photon;
using Il2CppSystem;
using Il2CppSystem.Collections;
using Il2CppSystem.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;

namespace MerinoClient.Core.VRChat;

public static class PhotonExtensions
{
    public static Player GetPlayer(this int actorNumber)
    {
        if (GetPlayersInRoom() == null) return null;
        foreach (var player in GetPlayersInRoom())
            if (player.Key == actorNumber)
                return player.Value;
        return null;
    }

    public static Dictionary<int, Player> GetPlayersInRoom()
    {
        return PhotonNetwork.prop_Room_0.field_Private_Dictionary_2_Int32_Player_0;
    }

    public static bool IsSelf(this EventData eventData)
    {
        return eventData.Sender == PhotonNetwork.prop_Player_0.GetActorNumber();
    }
    //https://forum.photonengine.com/discussion/5874/how-to-check-which-player-belongs-to-the-client

    public static string GetDisplayName(this Player player)
    {
        var userDict = player.field_Private_Hashtable_0["user"].TryCast<Dictionary<string, Object>>();
        return userDict["displayName"].ToString();
    }

    public static string GetDeveloperType(this Player player)
    {
        var userDict = player.field_Private_Hashtable_0["user"].TryCast<Dictionary<string, Object>>();
        return userDict["developerType"].ToString();
    }
}