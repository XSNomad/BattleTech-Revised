using System;
using Harmony;
using BattleTech;
using MightyChargingJuggernaut.Extensions;

namespace MightyChargingJuggernaut.Patches
{
    class Melee
    {

        [HarmonyPatch(typeof(Weapon), "Instability")]
        public static class Weapon_Instability_Patch
        {
            static void Postfix(Weapon __instance, ref float __result)
            {
                try
                {
                    if (__instance.Type != WeaponType.Melee || !(__instance.parent is Mech mech))
                    {
                        return;
                    }

                    Pilot pilot = mech.GetPilot();
                    
                    // Charge
                    if (pilot.IsJuggernaut() && Fields.JuggernautCharges)
                    {
                        __result *= 1.2f;
                        return;
                    }

                    // DFA
                    if (pilot.IsJuggernaut() && __instance.WeaponSubType == WeaponSubType.DFA)
                    {
                        __result *= 1.2f;
                    }
                }
                catch (Exception)
                {
                }
            }
        }



        [HarmonyPatch(typeof(MechMeleeSequence), "GenerateMeleePath")]
        public static class MechMeleeSequence_GenerateMeleePath_Patch
        {
            static void Postfix(MechMeleeSequence __instance, ref ActorMovementSequence ___moveSequence)
            {
                try
                {
                    Pilot pilot = __instance.owningActor.GetPilot();
                    if (pilot.IsJuggernaut() && Fields.JuggernautCharges)
                    {

                        // Setting this prevents the footstep effects from Coils to be displayed when a Juggernauts charges
                        new Traverse(___moveSequence).Property("isSprinting").SetValue(true);
                        ___moveSequence.IgnoreEndSmoothing = true;
                        ___moveSequence.meleeType = MeleeAttackType.Charge;

                    }
                }
                catch (Exception)
                {
                }
            }
        }



        [HarmonyPatch(typeof(MechMeleeSequence), "BuildMeleeDirectorSequence")]
        public static class MechMeleeSequence_BuildMeleeDirectorSequence_Patch
        {
            static void Prefix(MechMeleeSequence __instance)
            {
                try
                {
                    Pilot pilot = __instance.owningActor.GetPilot();
                    if (pilot.IsJuggernaut() && Fields.JuggernautCharges)
                    {
                        MeleeAttackType selectedMeleeType = (MeleeAttackType)AccessTools.Property(typeof(MechMeleeSequence), "selectedMeleeType").GetValue(__instance, null);
                        selectedMeleeType = MeleeAttackType.Charge;
                    }
                }
                catch (Exception)
                {
                }
            }
        }



        [HarmonyPatch(typeof(MechMeleeSequence), "ExecuteMove")]
        public static class MechMeleeSequence_ExecuteMove_Patch
        {
            static void Prefix(MechMeleeSequence __instance)
            {
                try
                {
                    Pilot pilot = __instance.owningActor.GetPilot();
                    if (pilot.IsJuggernaut() && Fields.JuggernautCharges)
                    {
                        __instance.OwningMech.SprintedLastRound = true;

                        // Push message out
                        AbstractActor actor = __instance.owningActor;
                        actor.Combat.MessageCenter.PublishMessage(new FloatieMessage(actor.GUID, actor.GUID, "CHARGING", FloatieMessage.MessageNature.Buff));
                    }
                }
                catch (Exception)
                {
                }
            }
        }



        [HarmonyPatch(typeof(MechMeleeSequence), "OnMoveComplete")]
        public static class MechMeleeSequence_OnMoveComplete_Patch
        {
            static void Prefix(MechMeleeSequence __instance)
            {
                try
                {
                    Pilot pilot = __instance.owningActor.GetPilot();
                    if (pilot.IsJuggernaut())
                    {

                        // Juggernauts only gain GUARDED on regular melee attack...
                        if (!__instance.owningActor.SprintedLastRound)
                        {
                            __instance.owningActor.BracedLastRound = true;

                            // Include stability reduction only when Mech remained "stationary"
                            if (__instance.OwningMech.DistMovedThisRound < 10f)
                            {
                                // @ToDo: Check if this will be applied from Core already and thus will result in a doubled reduction...
                                __instance.OwningMech.ApplyInstabilityReduction(StabilityChangeSource.RemainingStationary);
                            }
                        }
                        // ...but not when charging
                        else
                        {
                        }
                    }
                }
                catch (Exception)
                {
                }
            }
        }



        [HarmonyPatch(typeof(MechMeleeSequence), "OnMeleeComplete")]
        public static class MechMeleeSequence_OnMeleeComplete_Patch
        {
            static void Postfix(MechMeleeSequence __instance, MessageCenterMessage message)
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
                        // Get melee target
                        ICombatant MeleeTarget = (ICombatant)AccessTools.Property(typeof(MechMeleeSequence), "MeleeTarget").GetValue(__instance, null);

                        // Reapplying "MeleeHitPushBackPhases" here as it doesn't seem to work anymore when only defined in AbilityDef
                        (MeleeTarget as AbstractActor).ForceUnitOnePhaseDown(__instance.owningActor.GUID, __instance.SequenceGUID, false);


                        if (Fields.JuggernautCharges)
                        {
                            // Charge and tackle causes slight instability
                            
                            //float stabilityDamageSelf = __instance.OwningMech.GetMinStability(0, -1);
                            //__instance.OwningMech.AddAbsoluteInstability(stabilityDamageSelf, StabilityChangeSource.NotSet, __instance.owningActor.GUID);

                            float resultingStability = __instance.OwningMech.GetMinStability(__instance.OwningMech.CurrentStability, -1);
                            __instance.OwningMech.StatCollection.Set<float>("Stability", resultingStability);
                            __instance.OwningMech.NeedsInstabilityCheck = true;

                            if (MeleeTarget.IsDead || MeleeTarget.IsFlaggedForDeath)
                            {
                                return;
                            }

                            if (MeleeTarget is Mech TargetMech)
                            {
                                // Remove Entrenched from target when charging
                                if (TargetMech.IsEntrenched)
                                {
                                    TargetMech.IsEntrenched = false;
                                    TargetMech.Combat.MessageCenter.PublishMessage(new FloatieMessage(TargetMech.GUID, TargetMech.GUID, "LOST: ENTRENCHED", FloatieMessage.MessageNature.Debuff));
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



        [HarmonyPatch(typeof(MechMeleeSequence), "CompleteOrders")]
        public static class MechMeleeSequence_CompleteOrders_Patch
        {
            static void Postfix(MechMeleeSequence __instance)
            {
                try
                {
                    if (Fields.JuggernautCharges)
                    {
                        // Charge and tackle causes slight instability, check for unsteady
                        __instance.OwningMech.CheckForInstability();
                    }

                    // Just to be sure
                    Fields.JuggernautCharges = false;
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
