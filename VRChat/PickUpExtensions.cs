using System.Collections.Generic;
using UnityEngine;
using VRC.SDK3.Components;
using VRC_Pickup = VRCSDK2.VRC_Pickup;

namespace MerinoClient.Core.VRChat;

public static class PickUpExtensions
{
    public static void Destroy(this List<VRC_Pickup> pickups)
    {
        if (pickups == null) return;
        foreach (var pickup in pickups)
        {
            if (pickup == null) return;

            Object.DestroyImmediate(pickup);
        }
    }

    public static void Destroy(this List<VRCPickup> pickups)
    {
        if (pickups == null) return;
        foreach (var pickup in pickups)
        {
            if (pickup == null) return;

            Object.DestroyImmediate(pickup.gameObject);
        }
    }

    public static void SetActive(this List<VRC_Pickup> pickups, bool setActive)
    {
        if (pickups == null) return;
        foreach (var pickup in pickups)
        {
            if (pickup == null) return;

            pickup.gameObject.SetActive(setActive);
        }
    }

    public static void SetActive(this List<VRCPickup> pickups, bool setActive)
    {
        if (pickups == null) return;
        foreach (var pickup in pickups)
        {
            if (pickup == null) return;

            pickup.gameObject.SetActive(setActive);
        }
    }
}