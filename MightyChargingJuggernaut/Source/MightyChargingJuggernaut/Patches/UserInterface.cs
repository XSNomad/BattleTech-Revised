using System;
using System.Reflection;
using BattleTech;
using BattleTech.UI;
using Harmony;
using Localize;

namespace MightyChargingJuggernaut.Patches
{
    class UserInterface
    {

        [HarmonyPatch(typeof(SelectionStateMove), "FireButtonString", MethodType.Getter)]
        public static class SelectionStateMove_FireButtonString_Patch
        {
            public static void Postfix(SelectionStateMove __instance, ref string __result)
            {
                try
                {
                    if (__instance.HasDestination && Fields.JuggernautCharges)
                    {
                        __result = Strings.T("Sprint to TACKLE the target using Piloting skill to hit. Ignores EVASIVE. Hit removes GUARDED, deals damage and stability damage.");
                    }
                }
                catch (Exception)
                {
                }
            }
        }



        [HarmonyPatch(typeof(CombatHUDFireButton), "CurrentFireMode", MethodType.Setter)]
        public static class CombatHUDFireButton_CurrentFireMode_Patch
        {
            public static void Postfix(CombatHUDFireButton __instance, CombatHUDFireButton.FireMode value)
            {
                try
                {
                    if (value == CombatHUDFireButton.FireMode.Engage && Fields.JuggernautCharges)
                    {
                        __instance.FireText.SetText("CHARGE!");
                    }
                }
                catch (Exception)
                {
                }
            }
        }



        [HarmonyPatch(typeof(SelectionStateMove), "ProjectedStabilityForState", MethodType.Getter)]
        public static class SelectionStateMove_ProjectedStabilityForState_Patch
        {
            public static void Postfix(SelectionStateMove __instance, ref float __result)
            {
                try
                {
                    if ((__instance.SelectedActor is Mech mech) && Fields.JuggernautCharges)
                    {
                        
                        // This would be vanilla: No stability change when sprinting
                        //__result = mech.CurrentStability;

                        // Charge and tackle causes slight instability
                        __result = mech.GetMinStability(mech.CurrentStability, -1);
                    }
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
