using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace RentTycoon
{
    [System.Serializable]
    public struct LevelMultiplier
    {
        public int LevelMax;
        public float Multiplier;
    }

    [System.Serializable]
    public struct LevelCost
    {
        public int CountOfLevels;
        public int MaxLevel;
        public float Cost;
    }
    
    [System.Serializable]
    public struct UpgradeData
    {
        public List<LevelMultiplier> LevelMultipliers;

        public LevelMultiplier GetLevelMultiplier(int level)
        {
            LevelMultiplier levelMultiplier = new LevelMultiplier()
            {
                LevelMax = 0,
                Multiplier = 1
            };

            foreach (var lvlMultiplier in LevelMultipliers)
            {
                if(level < lvlMultiplier.LevelMax)
                {
                    levelMultiplier = lvlMultiplier;
                    break;
                }
            }
            
            Debug.LogError("Reward multiplier for level " + level + " is " + levelMultiplier.Multiplier);
            
            return levelMultiplier;
        }

        public LevelMultiplier GetPrevLevelMultiplier(LevelMultiplier curLvlMultiplier)
        {
            int lvlMultiplierIndex = LevelMultipliers.IndexOf(curLvlMultiplier);
            if(lvlMultiplierIndex == 0)
            {
                return new LevelMultiplier()
                {
                    LevelMax = 0,
                    Multiplier = 1
                };
            }
            else
            {
                return LevelMultipliers[lvlMultiplierIndex - 1];
            }
        }
    }
}

