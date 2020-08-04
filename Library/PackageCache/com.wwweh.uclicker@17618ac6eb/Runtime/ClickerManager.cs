﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace uClicker
{
    /// <summary>
    /// The "Game.cs" for this library. Deals with game logic, saving, loading... you name it - it does it!
    /// Also responsible for triggering the GUIDContainer system - anything referenced in the ManagerConfig will be loaded by this object
    /// This will populate the runtime GUIDContainer DB and make saving and loading work - so if you use the runtime DB, make sure this is loaded before you use it! 
    /// </summary>
    [CreateAssetMenu(menuName = "uClicker/Manager")]
    public class ClickerManager : ClickerComponent
    {
        public ManagerSaveSettings SaveSettings = new ManagerSaveSettings();
        public ManagerConfig Config;
        public ManagerState State;
        public BuildingProgresive Progresive;

        public UnityEvent OnTick;
        public UnityEvent OnBuyUpgrade;
        public UnityEvent OnBuyBuilding;


        #region Unity Events

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.WebGL &&
                SaveSettings.SaveType == ManagerSaveSettings.SaveTypeEnum.SaveToFile)
            {
                Debug.LogWarning("Cannot save to file on WebGL, changing to SaveToPlayerPrefs");
                SaveSettings.SaveType = ManagerSaveSettings.SaveTypeEnum.SaveToPlayerPrefs;
            }
        }
#endif

        private void OnDisable()
        {
            // Clear save on unload so we don't try deserializing the save between play/stop
            State = new ManagerState();
        }

        #endregion

        #region Public Game Logic

        public void Click(Clickable clickable)
        {
            double amount = clickable.Amount;
            Currency currency = clickable.Currency;

            ApplyClickPerks(clickable, ref amount);
            ApplyCurrencyPerk(currency, ref amount);

            amount += amount * 0.01f * State.CurrencyCurrentTotals[Config.Currencies[1]];
            bool updated = UpdateTotal(currency, amount);
            UpdateUnlocks();
            if (updated)
            {
                OnTick.Invoke();
            }
        }

        public void Tick()
        {

            bool updated = false;
            foreach (Currency currency in Config.Currencies)
            {
                double amount = PerSecondAmount(currency);

                updated = UpdateTotal(currency, amount);
            }

            UpdateUnlocks();

            if (updated)
            {
                OnTick.Invoke();
            }
        }

        public void BuildingChanger()
        {
            Debug.Log(State.BuildingCountType);
            foreach (var b in State.BuildingCountType)
            {
                Debug.Log(b);
                var lvl = (int)Math.Truncate((decimal)(b.Value) / Progresive.CountBuildings);
                var build = Config.AvailableBuildings.First(x => x.BuildingType == b.Key);
                var currentUpg = Progresive.ABP.First(x => x.Key == b.Key).Value[lvl];
                var ind = Config.AvailableBuildings.ToList().IndexOf(build);
                //Debug.Log($"{b.Key} : {lvl}");

                if(build != currentUpg)
                    Config.AvailableBuildings[ind] = currentUpg;
            }
        }

        public void BuyBuilding(Building building)
        {
            if (building == null)
            {
                return;
            }

            if (!BuildingAvailable(building))
            {
                return;
            }

            CurrencyTuple cost = BuildingCost(building);
            if (!Deduct(cost.Currency, cost.Amount))
            {
                return;
            }

            int buildingCount;
            State.EarnedBuildings.TryGetValue(building, out buildingCount);
            State.EarnedBuildings[building] = buildingCount + 1;
            if (State.BuildingCountType.ContainsKey(building.BuildingType))
                State.BuildingCountType[building.BuildingType] = State.BuildingCountType[building.BuildingType]+1;
            else
                State.StartBuildingCount();
            Debug.Log($"Buy building type : {building.BuildingType} \r\n BuildingCountType : {State.BuildingCountType[building.BuildingType]}");
            foreach (var item in State.BuildingCountType)
            {
                Debug.Log(item);
                
            }
            BuildingChanger();

            UpdateUnlocks();
            OnBuyBuilding.Invoke();
        }

        public void BuyUpgrade(Upgrade upgrade)
        {
            if (upgrade == null)
            {
                return;
            }

            if (!UpgradeAvailable(upgrade))
            {
                return;
            }

            if (!Deduct(upgrade.Cost.Currency, upgrade.Cost.Amount))
            {
                return;
            }

            State.EarnedUpgrades.Add(upgrade);
            UpdateUnlocks();
            OnBuyUpgrade.Invoke();
        }

        public bool CanBuy(ClickerComponent component)
        {
            CurrencyTuple cost;
            if (component is Building)
            {
                Building building = component as Building;
                if (!BuildingAvailable(building))
                {
                    return false;
                }

                cost = BuildingCost(building);
            }
            else if (component is Upgrade)
            {
                Upgrade upgrade = (component as Upgrade);
                if (!UpgradeAvailable(upgrade))
                {
                    return false;
                }

                cost = upgrade.Cost;
            }
            else
            {
                return true;
            }

            double amount;
            State.CurrencyCurrentTotals.TryGetValue(cost.Currency, out amount);
            return amount >= cost.Amount;
        }

        public CurrencyTuple BuildingCost(Building building)
        {
            int count;
            if (!State.EarnedBuildings.TryGetValue(building, out count))
            {
                count = 0;
            }

            CurrencyTuple currencyTuple = building.Cost;
            currencyTuple.Amount = (int) currencyTuple.Amount * Mathf.Pow(1 + Config.BuildingCostIncrease, count);
            return currencyTuple;
        }

        public void SaveProgress()
        {
            string value = JsonUtility.ToJson(State, true);
            switch (SaveSettings.SaveType)
            {
                case ManagerSaveSettings.SaveTypeEnum.SaveToPlayerPrefs:
                    PlayerPrefs.SetString(SaveSettings.SaveName, value);
                    break;
                case ManagerSaveSettings.SaveTypeEnum.SaveToFile:
                    File.WriteAllText(SaveSettings.FullSavePath, value);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void LoadProgress()
        {
            string json;
            switch (SaveSettings.SaveType)
            {
                case ManagerSaveSettings.SaveTypeEnum.SaveToPlayerPrefs:
                    json = PlayerPrefs.GetString(SaveSettings.SaveName);
                    break;
                case ManagerSaveSettings.SaveTypeEnum.SaveToFile:
                    if (!File.Exists(SaveSettings.FullSavePath))
                    {
                        return;
                    }

#if DEBUG
                    Debug.LogFormat("Loading Save from file: {0}", SaveSettings.FullSavePath);
#endif

                    json = File.ReadAllText(SaveSettings.FullSavePath);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            JsonUtility.FromJsonOverwrite(json, State);
            UpdateUnlocks();
            Progresive.FillABP();
            OnTick.Invoke();
            OnBuyBuilding.Invoke();
            OnBuyUpgrade.Invoke();

        }

        #endregion

        #region Internal Logic

        private bool Deduct(Currency costCurrency, double cost)
        {
            //Debug.Log(costCurrency);
            if (State.CurrencyCurrentTotals[costCurrency] < cost)
            {
                return false;
            }

            bool updated = UpdateTotal(costCurrency, -cost);
            if (updated)
            {
                OnTick.Invoke();
            }

            return true;
        }

        private bool UpdateTotal(Currency currency, double amount)
        {
            double total;
            State.CurrencyCurrentTotals.TryGetValue(currency, out total);
            total += amount;
            State.CurrencyCurrentTotals[currency] = total;

            if (amount > 0 || currency.name != "Gold")
            {
                double historicalTotal;
                State.CurrencyHistoricalTotals.TryGetValue(currency, out historicalTotal);
                State.CurrencyHistoricalTotals[currency] = historicalTotal + amount;
            }

            return true;
        }

        private double PerSecondAmount(Currency currency)
        {
            if (currency.name == "Gold")
            {
                double amount = 0;

                ApplyBuildingPerks(currency, ref amount);
                ApplyCurrencyPerk(currency, ref amount);
                if (State.CurrencyCurrentTotals.Count < 1)
                {
                    return amount;
                }
                return amount+= amount * 0.01f * State.CurrencyCurrentTotals[Config.Currencies[1]];
            }

            return 0;
        }

        private void UpdateUnlocks()
        {
            foreach (Building availableBuilding in Config.AvailableBuildings)
            {
                availableBuilding.Unlocked |= BuildingAvailable(availableBuilding);
            }

            foreach (Upgrade availableUpgrade in Config.AvailableUpgrades)
            {
                availableUpgrade.Unlocked |= UpgradeAvailable(availableUpgrade);
            }
        }

        private bool BuildingAvailable(Building building)
        {
            return IsUnlocked(building.RequirementGroups);
        }

        private bool UpgradeAvailable(Upgrade upgrade)
        {
            bool unlocked = true;
            unlocked &= !State.EarnedUpgrades.Contains(upgrade);
            unlocked &= IsUnlocked(upgrade.RequirementGroups);

            return unlocked;
        }

        private bool IsUnlocked(RequirementGroup[] requirementGroups)
        {
            bool compareStarted = false;
            // if empty it's unlocked
            bool groupsUnlocked = requirementGroups.Length == 0;
            foreach (RequirementGroup requirementGroup in requirementGroups)
            {
                if (!compareStarted)
                {
                    compareStarted = true;
                    groupsUnlocked = requirementGroup.GroupOperand == RequirementOperand.And;
                }

                bool unlocked = true;
                foreach (Requirement requirement in requirementGroup.Requirements)
                {
                    switch (requirement.RequirementType)
                    {
                        case RequirementType.Currency:
                            unlocked &= requirement.UnlockAmount.Currency == null ||
                                        (State.CurrencyHistoricalTotals.ContainsKey(requirement.UnlockAmount
                                             .Currency) &&
                                         State.CurrencyHistoricalTotals[requirement.UnlockAmount.Currency] >=
                                         requirement.UnlockAmount.Amount);
                            break;
                        case RequirementType.Building:
                            unlocked &= requirement.UnlockBuilding.Building == null ||
                                        State.EarnedBuildings.ContainsKey(requirement.UnlockBuilding.Building) &&
                                        State.EarnedBuildings[requirement.UnlockBuilding.Building] >=
                                        requirement.UnlockBuilding.Amount;
                            break;
                        case RequirementType.Upgrade:
                            unlocked &= requirement.UnlockUpgrade == null ||
                                        State.EarnedUpgrades.Contains(requirement.UnlockUpgrade);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                if (requirementGroup.GroupOperand == RequirementOperand.And)
                {
                    groupsUnlocked &= unlocked;
                }
                else
                {
                    groupsUnlocked |= unlocked;
                }
            }

            return groupsUnlocked;
        }

        private void ApplyClickPerks(Clickable clickable, ref double amount)
        {
            foreach (Upgrade upgrade in State.EarnedUpgrades)
            {
                foreach (UpgradePerk upgradePerk in upgrade.UpgradePerk)
                {
                    if (upgradePerk.TargetClickable != clickable)
                    {
                        continue;
                    }

                    switch (upgradePerk.Operation)
                    {
                        case Operation.Add:
                            amount += upgradePerk.Amount;
                            break;
                        case Operation.Multiply:
                            amount *= upgradePerk.Amount;
                            break;
                    }
                }
            }
        }

        private void ApplyBuildingPerks(Currency currency, ref double amount)
        {
            foreach (KeyValuePair<Building, int> kvp in State.EarnedBuildings)
            {
                Building building = kvp.Key;
                if (building.YieldAmount.Currency != currency)
                {
                    continue;
                }

                int buildingCount = kvp.Value;
                amount += building.YieldAmount.Amount * buildingCount;

                foreach (Upgrade upgrade in State.EarnedUpgrades)
                {
                    foreach (UpgradePerk upgradePerk in upgrade.UpgradePerk)
                    {
                        if (upgradePerk.TargetBuilding != building)
                        {
                            continue;
                        }

                        switch (upgradePerk.Operation)
                        {
                            case Operation.Add:
                                amount += upgradePerk.Amount;
                                break;
                            case Operation.Multiply:
                                amount *= upgradePerk.Amount;
                                break;
                        }
                    }
                }
            }
        }

        private void ApplyCurrencyPerk(Currency currency, ref double amount)
        {
            foreach (Upgrade upgrade in State.EarnedUpgrades)
            {
                foreach (UpgradePerk upgradePerk in upgrade.UpgradePerk)
                {
                    if (upgradePerk.TargetCurrency != currency)
                    {
                        continue;
                    }

                    switch (upgradePerk.Operation)
                    {
                        case Operation.Add:
                            amount += upgradePerk.Amount;
                            break;
                        case Operation.Multiply:
                            amount *= upgradePerk.Amount;
                            break;
                    }
                }
            }
        }

        #endregion


       


    }
}