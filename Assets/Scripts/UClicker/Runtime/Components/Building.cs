using System;
using UnityEngine;
using uClicker;

namespace uClicker
{
    [CreateAssetMenu(menuName = "uClicker/Building")]
    public class Building : UnlockableComponent
    {
        public CurrencyTuple Cost;
        public CurrencyTuple YieldAmount;
        public BuildingType BuildingType;

    }



    public class TypeBuilding
    {
        public Building Building;
        public int LVLAmount;
    }


    [Serializable]
    public struct BuildingTuple
    {
        public Building Building;
        public int Amount;
    }


    public enum BuildingType
    {
        Miner,
        Transport,
        Factory,
        Drill,
        City,
        GoldMine
    }
}