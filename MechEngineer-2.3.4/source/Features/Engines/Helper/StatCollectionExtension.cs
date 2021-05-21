﻿using BattleTech;

namespace MechEngineer.Features.Engines.Helper
{
    internal static class StatCollectionExtension
    {
        internal static StatisticAdapter<float> JumpJetCountMultiplier(this StatCollection statCollection)
        {
            return new StatisticAdapter<float>("JumpJetCountMultiplier", statCollection, 1);
        }

        internal static StatisticAdapter<float> JumpCapacity(this StatCollection statCollection)
        {
            return new StatisticAdapter<float>("JumpCapacity", statCollection, 0);
        }

        internal static StatisticAdapter<float> JumpDistanceMultiplier(this StatCollection statCollection)
        {
            return new StatisticAdapter<float>("JumpDistanceMultiplier", statCollection, 1);
        }

        internal static StatisticAdapter<float> WalkSpeed(this StatCollection statCollection)
        {
            return new StatisticAdapter<float>("WalkSpeed", statCollection, 0);
        }

        internal static StatisticAdapter<float> RunSpeed(this StatCollection statCollection)
        {
            return new StatisticAdapter<float>("RunSpeed", statCollection, 0);
        }

        internal static StatisticAdapter<float> HeatSinkCapacity(this StatCollection statCollection)
        {
            return new StatisticAdapter<float>("HeatSinkCapacity", statCollection, 0);
        }

        internal static StatisticAdapter<float> HeatGenerated(this StatCollection statCollection)
        {
            return new StatisticAdapter<float>("HeatGenerated", statCollection, 0);
        }

        internal static StatisticAdapter<float> WeaponHeatMultiplier(this StatCollection statCollection)
        {
            return new StatisticAdapter<float>("WeaponHeatMultiplier", statCollection, 1);
        }
        
        internal static StatisticAdapter<float> JumpHeat(this StatCollection statCollection)
        {
            return new StatisticAdapter<float>("JumpHeat", statCollection, 0);
        }

        internal static StatisticAdapter<int> MaxHeat(this StatCollection statCollection)
        {
            return new StatisticAdapter<int>("MaxHeat", statCollection, MechStatisticsRules.Combat.Heat.MaxHeat);
        }

        internal static StatisticAdapter<int> OverheatLevel(this StatCollection statCollection)
        {
            return new StatisticAdapter<int>("OverheatLevel", statCollection, (int)(MechStatisticsRules.Combat.Heat.OverheatLevel * MechStatisticsRules.Combat.Heat.MaxHeat));
        }
    }
}