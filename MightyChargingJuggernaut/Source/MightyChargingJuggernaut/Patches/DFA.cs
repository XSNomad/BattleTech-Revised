using System;
using Harmony;
using BattleTech;
using MightyChargingJuggernaut.Extensions;

namespace MightyChargingJuggernaut.Patches
{
    class DFA
    {
        // DFAs from Juggernaut can directly knock a target down
        [HarmonyPatch(typeof(MechDFASequence), "OnMeleeComplete")]
        public static class MechDFASequence_OnMeleeComplete_Patch
        {
            public static void Prefix(MechDFASequence __instance, MessageCenterMessage message)
            {
                try
                {
                    AttackCompleteMessage attackCompleteMessage = (AttackCompleteMessage)message;
                    if (attackCompleteMessage.attackSequence.attackCompletelyMissed)
                    {
                        return;
                    }

                    Pilot pilot = __instance.owningActor.GetPilot();
                    if (pilot.IsJuggernaut())
                    {
                        ICombatant DFATarget = (ICombatant)AccessTools.Property(typeof(MechDFASequence), "DFATarget").GetValue(__instance, null);

                        if (DFATarget.IsDead || DFATarget.IsFlaggedForDeath)
                        {
                            return;
                        }

                        // IMPORTANT! At this point any stab dmg is already applied to the target, normalized by entrenched or terrain...
                        if (DFATarget is Mech TargetMech)
                        {

                            if (TargetMech.CurrentStability >= TargetMech.MaxStability)
                            {
                                TargetMech.FlagForKnockdown();

                                if (!TargetMech.IsUnsteady)
                                {
                                    // Push message out
                                    TargetMech.Combat.MessageCenter.PublishMessage(new FloatieMessage(TargetMech.GUID, TargetMech.GUID, "OFF BALANCE", FloatieMessage.MessageNature.Debuff));
                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
