using System;
using System.Text.RegularExpressions;
using VRC.Core;

namespace MerinoClient.Core.VRChat;

public static class VRCCoreExtensions
{
    public static APIUser GetInstanceCreator(this string idWithTags)
    {
        var m = Regex.Match(idWithTags, @"\(([^)]*)\)").Groups[1].Value;

        var fetchedAPIUser = new APIUser();

        API.Fetch<APIUser>(m, new Action<ApiContainer>(container =>
        {
            fetchedAPIUser = container.Model.Cast<APIUser>();
        }));

        if (fetchedAPIUser == null)
            throw new NullReferenceException(nameof(fetchedAPIUser));

        return fetchedAPIUser;
    }

    public static NetworkRegion GetNetworkRegion(this string idWithTags)
    {
        if (idWithTags.Contains("region(eu)"))
            return NetworkRegion.Europe;

        if (idWithTags.Contains("region(jp)"))
            return NetworkRegion.Japan;

        return idWithTags.Contains("region(use)") ? NetworkRegion.US_East : NetworkRegion.US_West;
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

    public static string TranslateInstanceType(this ApiWorldInstance apiWorldInstance)
    {
        if (apiWorldInstance == null)
            throw new ArgumentNullException(nameof(apiWorldInstance));
        return apiWorldInstance.type switch
            {
                InstanceAccessType.InviteOnly => "Invite",
                InstanceAccessType.InvitePlus => "Invite+",
                InstanceAccessType.FriendsOnly => "Friends",
                InstanceAccessType.FriendsOfGuests => "Friends+",
                InstanceAccessType.Public => "Public",
                _ => "Public"
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