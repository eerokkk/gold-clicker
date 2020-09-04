using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace uClicker
{
    [Serializable]
    public class ManagerState : ISerializationCallbackReceiver
    {
        [NonSerialized] public Dictionary<Building, int> EarnedBuildings = new Dictionary<Building, int>();
        [NonSerialized] public List<Upgrade> EarnedUpgrades = new List<Upgrade>();
        [NonSerialized] public Dictionary<Currency, double> CurrencyCurrentTotals = new Dictionary<Currency, double>();
        [NonSerialized] public Dictionary<Currency, double> CurrencyHistoricalTotals = new Dictionary<Currency, double>();
        public Dictionary<BuildingType, int> BuildingCountType = new Dictionary<BuildingType, int>();
        public double PercentUranus;
        [NonSerialized]public Dictionary<Building, int> BuildingMaxBuy = new Dictionary<Building, int>();
        public double PerSecondAmount;
        [SerializeField] public double UraniumIncrease = 1;

        [SerializeField] private List<GUIDContainer> _earnedBuildings = new List<GUIDContainer>();
        [SerializeField] private List<int> _earnedBuildingsCount = new List<int>();
        [SerializeField] private List<GUIDContainer> _earnedUpgrades = new List<GUIDContainer>();
        [SerializeField] private List<GUIDContainer> _currencies = new List<GUIDContainer>();
        [SerializeField] private List<double> _currencyCurrentTotals = new List<double>();
        [SerializeField] private List<double> _currencyHistoricalTotals = new List<double>();

        public void StartBuildingCount()
        {
            BuildingCountType.Clear();
            BuildingMaxBuy.Clear();
            foreach (var a in EarnedBuildings)
            {
                if (!BuildingCountType.ContainsKey(a.Key.BuildingType))
                    BuildingCountType.Add(a.Key.BuildingType, a.Value);
                else
                    BuildingCountType[a.Key.BuildingType] = BuildingCountType[a.Key.BuildingType] + a.Value;
                //Debug.Log(a);
            }
        }
        public void OnBeforeSerialize()
        {
            _earnedBuildings.Clear();
            _earnedBuildingsCount.Clear();
            foreach (KeyValuePair<Building, int> kvp in EarnedBuildings)
            {
                _earnedBuildings.Add(kvp.Key.GUIDContainer);
                _earnedBuildingsCount.Add(kvp.Value);
            }

            _currencies.Clear();
            _currencyCurrentTotals.Clear();
            _currencyHistoricalTotals.Clear();
            foreach (KeyValuePair<Currency, double> kvp in CurrencyCurrentTotals)
            {
                _currencies.Add(kvp.Key.GUIDContainer);
                _currencyCurrentTotals.Add(kvp.Value);
                double historicalTotal;
                CurrencyHistoricalTotals.TryGetValue(kvp.Key, out historicalTotal);
                _currencyHistoricalTotals.Add(historicalTotal);
            }

            _earnedUpgrades = EarnedUpgrades.ConvertAll(input => input.GUIDContainer);
        }

        public void OnAfterDeserialize()
        {
            for (int i = 0; i < _earnedBuildings.Count; i++)
            {
                EarnedBuildings[(Building) ClickerComponent.RuntimeLookup[_earnedBuildings[i].Guid]] =
                    _earnedBuildingsCount[i];
            }

            for (int i = 0; i < _currencies.Count; i++)
            {
                CurrencyCurrentTotals[(Currency) ClickerComponent.RuntimeLookup[_currencies[i].Guid]] =
                    _currencyCurrentTotals[i];
                CurrencyHistoricalTotals[(Currency) ClickerComponent.RuntimeLookup[_currencies[i].Guid]] =
                    _currencyHistoricalTotals[i];
            }

            EarnedUpgrades = _earnedUpgrades.ConvertAll(input => (Upgrade) ClickerComponent.RuntimeLookup[input.Guid]);
            StartBuildingCount();
        }
    }
}