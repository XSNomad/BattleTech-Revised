using BattleTech;

namespace MightyChargingJuggernaut.Extensions
{
    public static class PilotExtensions
    {
        public static bool IsJuggernaut(this Pilot pilot)
        {
            bool isJuggernaut = false;
            var mechTags = pilot.ParentActor.GetTags();
            if (mechTags.Contains("BR_MQ_Charger"))
                isJuggernaut = true;
            if (pilot.PassiveAbilities.Find((Ability a) => a.Def.Description.Name == "JUGGERNAUT") != null)
                isJuggernaut = true;
            return isJuggernaut;
        }
    }
}
