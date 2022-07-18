using System;
using System.Text.RegularExpressions;
using VRC.Core;

namespace MerinoClient.Core.VRChat;

public static class VRCCoreExtensions
{
    public static string GetInstanceCreator(this string idWithTags)
    {
        var instanceAccessType = idWithTags.GetInstanceAccessType();

        if (instanceAccessType == InstanceAccessType.Public)
            throw new Exception("Can't retrieve an instance creator of a public lobby");

        var instanceCreator = Regex.Match(idWithTags, @"\((.*?)\)").Groups[1].Value;

        if (string.IsNullOrEmpty(instanceCreator))
            throw new Exception($"Instance contains no instance creator: {idWithTags}");

        return instanceCreator;
    }

    public static NetworkRegion GetNetworkRegion(this string idWithTags)
    {
        if (idWithTags.Contains("region(eu)"))
            return NetworkRegion.Europe;

        if (idWithTags.Contains("region(jp)"))
            return NetworkRegion.Japan;

        return idWithTags.Contains("region(use)") ? NetworkRegion.US_East : NetworkRegion.US_West;
    }

    public static InstanceAccessType GetInstanceAccessType(this string idWithTags)
    {
        if (idWithTags.Contains("hidden"))
            return InstanceAccessType.FriendsOfGuests;

        if (idWithTags.Contains("friends"))
            return InstanceAccessType.FriendsOnly;

        if (idWithTags.Contains("private") && idWithTags.Contains("canRequestInvite"))
            return InstanceAccessType.InvitePlus;

        return idWithTags.Contains("private") ? InstanceAccessType.InviteOnly : InstanceAccessType.Public;
    }

    public static string TranslateInstanceType(this InstanceAccessType instanceAccessType)
    {
        return instanceAccessType switch
        {
            InstanceAccessType.InviteOnly => "Invite",
            InstanceAccessType.InvitePlus => "Invite+",
            InstanceAccessType.FriendsOnly => "Friends",
            InstanceAccessType.FriendsOfGuests => "Friends+",
            InstanceAccessType.Public => "Public",
            _ => throw new ArgumentOutOfRangeException(nameof(instanceAccessType), instanceAccessType, null)
        };
    }

    public static string ParseOnlyId(this ApiWorldInstance apiWorldInstance)
    {
        var instanceId = apiWorldInstance.instanceId;

        if (string.IsNullOrEmpty(instanceId))
            throw new Exception("InstanceId is an empty string");

        var array = instanceId.Split('~');
        return array.Length == 0 ? instanceId : array[0];
    }
}