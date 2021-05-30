using BattleTech;
using UnityEngine;

namespace MightyChargingJuggernaut
{
    class Utilities
    {
        public static float GetAdditionalStabilityDamageFromSprintDistance(Mech attackingMech, Mech targetMech, bool ignoreModifiers = false)
        {
            float result = 0;

            float receivedInstabilityMultiplier = targetMech.StatCollection.GetValue<float>("ReceivedInstabilityMultiplier");
            float entrenchedMultiplier = (targetMech as AbstractActor).EntrenchedMultiplier;

            float distanceSprinted = attackingMech.DistMovedThisRound;
            float percentSprinted = distanceSprinted / attackingMech.MaxSprintDistance;
            float finalMultiplier = Mathf.Clamp((percentSprinted - 0.35f), 0.1f, 0.5f);
            result = attackingMech.MechDef.Chassis.MeleeInstability * finalMultiplier;

            if (ignoreModifiers)
            {
                return result;
            }
            else
            {
                result *= receivedInstabilityMultiplier;
                result *= entrenchedMultiplier;
                return result;
            }
        }



        public static float GetAdditionalStabilityDamageFromJumpDistance(Mech attackingMech, Mech targetMech, bool ignoreModifiers = false)
        {
            float result = 0;

            float receivedInstabilityMultiplier = targetMech.StatCollection.GetValue<float>("ReceivedInstabilityMultiplier");
            float entrenchedMultiplier = (targetMech as AbstractActor).EntrenchedMultiplier;

            float distanceJumped = attackingMech.DistMovedThisRound;
            int installedJumpjets = attackingMech.jumpjets.Count;
            float maxJumpDistance;

            // Borrowed from Mech.JumpDistance
            if (installedJumpjets >= attackingMech.Combat.Constants.MoveConstants.MoveTable.Length)
            {
                maxJumpDistance = attackingMech.Combat.Constants.MoveConstants.MoveTable[attackingMech.Combat.Constants.MoveConstants.MoveTable.Length - 1] * attackingMech.StatCollection.GetValue<float>("JumpDistanceMultiplier");
            }
            else
            {
                maxJumpDistance = attackingMech.Combat.Constants.MoveConstants.MoveTable[installedJumpjets] * attackingMech.StatCollection.GetValue<float>("JumpDistanceMultiplier");
            }

            float percentJumped = distanceJumped / maxJumpDistance;
            float finalMultiplier = Mathf.Clamp((percentJumped - 0.35f), 0.1f, 0.5f);

            result = attackingMech.MechDef.Chassis.DFAInstability * finalMultiplier;

            if (ignoreModifiers)
            {
                return result;
            }
            else
            {
                result *= receivedInstabilityMultiplier;
                result *= entrenchedMultiplier;

                return result;
            }
        }
    }
}
