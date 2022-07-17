using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using VRC;
using VRC.Core;
using VRC.DataModel;
using VRC.SDKBase;
using VRC.UI;
using VRC.UI.Elements.Menus;

namespace MerinoClient.Core.VRChat
{
    public static class PlayerExtensions
    {
        public static Player[] GetPlayers(this PlayerManager playerManager)
        {
            return playerManager.field_Private_List_1_Player_0.ToArray();
        }

        public static Player GetPlayer(this PlayerManager playerManager, string userId)
        {
            foreach (var player in playerManager.GetPlayers())
            {
                if (player == null)
                    continue;

                var apiUser = player.GetAPIUser();
                if (apiUser == null)
                    continue;

                if (apiUser.id == userId)
                    return player;
            }

            return null;
        }

        public static Player GetPlayer(this PlayerManager playerManager, int actorNr)
        {
            foreach (var player in playerManager.GetPlayers())
            {
                if (player == null)
                    continue;
                if (player.prop_Int32_0 == actorNr)
                    return player;
            }

            return null;
        }

        public static VRCPlayer GetVRCPlayer(this Player player)
        {
            return player._vrcplayer;
        }

        public static APIUser GetAPIUser(this Player player)
        {
            return player.field_Private_APIUser_0;
        }

        public static ApiAvatar GetApiAvatar(this Player player)
        {
            return player.prop_ApiAvatar_0;
        }

        public static Photon.Realtime.Player GetPlayer(this Player player)
        {
            return player.prop_Player_1;
        }

        public static VRCPlayerApi GeVRCPlayerApi(this Player player)
        {
            return player.field_Private_VRCPlayerApi_0;
        }

        public static string GetAPIUserType(this APIUser apiUser)
        {
            if (apiUser.IsStaff()) return "VRChat Team";

            return APIUser.IsFriendsWith(apiUser.id) ? "Friend" : "User";
        }

        public static int GetActorNumber(this Photon.Realtime.Player player)
        {
            return player.field_Private_Int32_0;
        }

        public static string GetPlayerType(this Photon.Realtime.Player player)
        {
            return player.prop_Boolean_1 ? "MasterClient" : "Player";
        }

        public static Player GetPlayer(this VRCPlayer vrcPlayer)
        {
            return vrcPlayer._player;
        }

        public static PlayerNet GetPlayerNet(this VRCPlayer vrcPlayer)
        {
            return vrcPlayer._playerNet;
        }

        public static GameObject GetAvatarObject(this VRCPlayer vrcPlayer)
        {
            return vrcPlayer.field_Internal_GameObject_0;
        }

        public static VRCPlayerApi GetPlayerApi(this VRCPlayer vrcPlayer)
        {
            return vrcPlayer.field_Private_VRCPlayerApi_0;
        }

        public static string GetUserId(this IUser iUser)
        {
            return iUser.prop_String_0;
        }

        public static string GetAvatarId(this IUser iUser)
        {
            var avatarId = "avtr_00000000-0000-0000-0000-000000000000";

            iUser.prop_ILoadable_1_InterfacePublicAbstractStBoSt1BoTeStDaBoObUnique_0
                .Method_Public_Abstract_Virtual_New_Void_Action_1_T_Action_1_String_0(
                    new Action<InterfacePublicAbstractStBoSt1BoTeStDaBoObUnique>(activeAvatar =>
                    {
                        avatarId = activeAvatar.prop_String_0;
                    }));

            return avatarId;
        }

        public static string GetDisplayName(this IUser iUser)
        {
            return iUser.prop_String_1;
        }

        public static bool IsSelf(this IUser iUser)
        {
            return iUser.GetUserId() == APIUser.CurrentUser.id;
        }

        public static string GetLocation(this IUser iUser)
        {
            return iUser.prop_Observable_1_String_2?.prop_TYPE_0;
        }

        public static bool IsStaff(this APIUser user)
        {
            if (user.hasModerationPowers)
                return true;
            if (user.developerType != APIUser.DeveloperType.None)
                return true;
            return user.tags.Contains("admin_moderator") || user.tags.Contains("admin_scripting_access") ||
                   user.tags.Contains("admin_official_thumbnail");
        }

        public static bool IsUserInRoom(this APIUser user)
        {
            return PageUserInfo.Method_Private_Static_Boolean_APIUser_0(user);
        }

        public static bool IsOwn(this ApiAvatar apiAvatar)
        {
            return apiAvatar.authorId == APIUser.CurrentUser.id;
        }

        public static IUser GetIUser(this SelectedUserMenuQM selectedUserMenuQm)
        {
            return selectedUserMenuQm.field_Private_IUser_0;
        }

        public static IUser GetIUser(this PageUserInfo pageUserInfo)
        {
            return pageUserInfo.field_Private_IUser_0;
        }

        public static APIUser GetAPIUser(this PageUserInfo pageUserInfo)
        {
            return pageUserInfo.field_Private_APIUser_0;
        }

        private static MethodInfo _reloadAvatarMethod;
        private static MethodInfo LoadAvatarMethod
        {
            get
            {
                if (_reloadAvatarMethod == null)
                {
                    _reloadAvatarMethod = typeof(VRCPlayer).GetMethods().First(mi => mi.Name.StartsWith("Method_Private_Void_Boolean_") && mi.Name.Length < 31 && mi.GetParameters().Any(pi => pi.IsOptional) && XrefUtils.CheckUsedBy(mi, "ReloadAvatarNetworkedRPC"));
                }

                return _reloadAvatarMethod;
            }
        }

        private static MethodInfo _reloadAllAvatarsMethod;
        private static MethodInfo ReloadAllAvatarsMethod
        {
            get
            {
                if (_reloadAllAvatarsMethod == null)
                {
                    _reloadAllAvatarsMethod = typeof(VRCPlayer).GetMethods().First(mi => mi.Name.StartsWith("Method_Public_Void_Boolean_") && mi.Name.Length < 30 && mi.GetParameters().All(pi => pi.IsOptional) && XrefUtils.CheckUsedBy(mi, "Method_Public_Void_", typeof(FeaturePermissionManager)));// Both methods seem to do the same thing;
                }

                return _reloadAllAvatarsMethod;
            }
        }
        public static void ReloadAvatar(this VRCPlayer instance)
        {
            LoadAvatarMethod.Invoke(instance, new object[] { true }); // parameter is forceLoad and has to be true
        }

        public static void ReloadAllAvatars(this VRCPlayer instance, bool ignoreSelf = false)
        {
            ReloadAllAvatarsMethod.Invoke(instance, new object[] { ignoreSelf });
        }

        public static void ChangeToAvatar(this VRCPlayer instance, string avatarId)
        {
            if (!instance.GetPlayer().GetAPIUser().IsSelf)
            {
                throw new ArgumentException("You can't change other peoples avatar.", nameof(instance));
            }

            var apiModelContainer = new ApiModelContainer<ApiAvatar>
            {
                OnSuccess = new Action<ApiContainer>(_ =>
                {
                    var pageAvatar = Resources.FindObjectsOfTypeAll<PageAvatar>()[0];
                    var apiAvatar = new ApiAvatar
                    {
                        id = avatarId
                    };
                    pageAvatar.field_Public_SimpleAvatarPedestal_0.field_Internal_ApiAvatar_0 = apiAvatar;
                    pageAvatar.ChangeToSelectedAvatar();
                })
            };
            API.SendRequest($"avatars/{avatarId}", 0, apiModelContainer, null, true, true, 3600f, 2);
        }


        public static TrustLevel GetTrustLevel(this APIUser user)
        {
            if (user.IsStaff()) return TrustLevel.VRChatTeam;

            if (user.hasVeteranTrustLevel)
                return TrustLevel.Trusted;

            if (user.hasTrustedTrustLevel)
                return TrustLevel.Known;

            if (user.hasKnownTrustLevel)
                return TrustLevel.User;

            if (user.hasBasicTrustLevel)
                return TrustLevel.New;

            if (user.isUntrusted)
                return TrustLevel.Visitor;

            if (user.hasNegativeTrustLevel || user.hasVeryNegativeTrustLevel)
                return TrustLevel.Nuisance;

            return TrustLevel.Visitor;
        }

        public enum TrustLevel
        {
            VRChatTeam,
            Trusted,
            Known,
            User,
            New,
            Visitor,
            Nuisance
        }
    }
}
