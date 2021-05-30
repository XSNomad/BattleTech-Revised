using System;
using BattleTech.UI;
using Harmony;

namespace MightyChargingJuggernaut.Patches
{
    class Coil
    {
        [HarmonyPatch(typeof(CombatHUDEvasiveBarPips), "CacheActorData")]
        public static class CombatHUDEvasiveBarPips_CacheActorData_Patch
        {
            public static void Postfix(CombatHUDEvasiveBarPips __instance, ref bool ___ShouldShowCOILPips)
            {
                try
                {
                    if (Fields.JuggernautCharges && ___ShouldShowCOILPips)
                    {
                        ___ShouldShowCOILPips = false;
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        [HarmonyPatch(typeof(CombatHUDWeaponSlot), "RefreshDisplayedWeapon")]
        public static class CombatHUDWeaponSlot_RefreshDisplayedWeapon_Patch
        {
            public static void Prefix(CombatHUDWeaponSlot __instance, ref bool sprinting)
            {
                try
                {
                    if (Fields.JuggernautCharges && !sprinting)
                    {
                        sprinting = true;
                    }
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
