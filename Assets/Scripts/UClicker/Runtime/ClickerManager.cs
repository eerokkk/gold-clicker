using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
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
        public int BuyMultiply = 1;
        public bool BuyMax = false;

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
        public void StartBuyMax()
        {
            for(int i = 0; i < 6; i++)
            {
                State.BuildingMaxBuy.Add(Config.AvailableBuildings[i], 0);
                //Debug.Log($"Config.AvailableBuildings[{i}]+++++++++++++++{Config.AvailableBuildings[i]}");
                //Debug.Log($"State.BuildingMaxBuy+++++++++++++++{State.BuildingMaxBuy[Config.AvailableBuildings[i]]} and {i}");
                //State.BuildingMaxBuy[Config.AvailableBuildings[i]] = i;
                //Debug.Log($"State.BuildingMaxBuy[Config.AvailableBuildings[{i}]]+++++++++++++++{State.BuildingMaxBuy[Config.AvailableBuildings[i]]}");
            }
        }


        private void OnDisable()
        {
            // Clear save on unload so we don't try deserializing the save between play/stop
            State = new ManagerState();
        }

        #endregion

        #region Public Game Logic

        public void Click(Clickable clickable)
        {
            double percent = State.PercentUranus;
            double amount = clickable.Amount;
            Currency currency = clickable.Currency;

            ApplyClickPerks(clickable, ref amount);
            ApplyCurrencyPerk(currency, ref amount, ref percent);

            //Debug.Log($"percent ==============={percent}");
            //Debug.Log($"State.CurrencyCurrentTotals[Config.Currencies[1]] ==============={State.CurrencyCurrentTotals[Config.Currencies[1]]}");
            //Debug.Log($"amount * (percent ==============={amount}");

            amount += amount * (percent * State.CurrencyCurrentTotals[Config.Currencies[1]]);
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
                if (currency.name == "Gold")
                    State.PerSecondAmount = amount;
                if (State.CurrencyHistoricalTotals.ContainsKey(Config.Currencies[0]))
                    State.UraniumIncrease = Math.Round(State.CurrencyHistoricalTotals[Config.Currencies[0]] / 1000000d);
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
            foreach (var b in State.BuildingCountType)
            {
                var lvl = (int)Math.Truncate((decimal)(b.Value) / Progresive.CountBuildings);
                var build = Config.AvailableBuildings.First(x => x.BuildingType == b.Key);
                if (lvl < Progresive.ABP.First(x => x.Key == b.Key).Value.Length)
                {
                    var currentUpg = Progresive.ABP.First(x => x.Key == b.Key).Value[lvl];
                    var ind = Config.AvailableBuildings.ToList().IndexOf(build);

                    if(build != currentUpg)
                        Config.AvailableBuildings[ind] = currentUpg;
                }
                
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
            BuyMultiply = State.BuildingMaxBuy[building];
            CurrencyTuple cost = BuildingCost(building);
            if (!Deduct(cost.Currency, cost.Amount))
            {
                return;
            }

            
            int buildingCount;

            State.EarnedBuildings.TryGetValue(building, out buildingCount);

            State.EarnedBuildings[building] = buildingCount + BuyMultiply;

            if (State.BuildingCountType.ContainsKey(building.BuildingType))
                State.BuildingCountType[building.BuildingType] = State.BuildingCountType[building.BuildingType] + BuyMultiply;
            else;
                State.StartBuildingCount();
            //Debug.Log($"Buy building type : {building.BuildingType} \r\n BuildingCountType : {State.BuildingCountType[building.BuildingType]}");

            State.BuildingMaxBuy[building] = 0;
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
            if (!State.EarnedBuildings.TryGetValue(building, out var count))
            {
                count = 0;
            }

            CurrencyTuple currencyTuple = new CurrencyTuple
            {
                Amount = building.Cost.Amount, 
                Currency = building.Cost.Currency
            };
            double summ = 0;

            if(!BuyMax)
                for (var i = 1; i <= BuyMultiply; i++)
                {
                    currencyTuple.Amount = 15 * Math.Pow(1 + Config.BuildingCostIncrease, count);
                    count++;
                    summ += currencyTuple.Amount;
                    State.BuildingMaxBuy[building] = BuyMultiply;
                }
            else
            {
                int up = 0;
                for (int i = 1; i > up; i++)
                {
                    summ += currencyTuple.Amount;
                    if (summ >= State.CurrencyCurrentTotals[currencyTuple.Currency])
                    {
                        if ((summ - currencyTuple.Amount)<= 0)
                        {
                            summ = currencyTuple.Amount;
                        }
                        else
                        {
                            summ -= currencyTuple.Amount;
                        }

                        if (State.CurrencyCurrentTotals[currencyTuple.Currency] == currencyTuple.Amount)
                        {
                            State.BuildingMaxBuy[building] = 1;
                        }
                        else 
                        {
                            State.BuildingMaxBuy[building] = i - 1;
                        }

                        //Debug.Log($"building ====  { building},  i ======{i}" );
                        break;
                    }

                    currencyTuple.Amount *= (1 + Config.BuildingCostIncrease);
                }
            }

            currencyTuple.Amount = summ;
            //Debug.Log($"currencyTuple.Amount = {currencyTuple.Amount}");
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
               // Debug.Log($"FALSE , State.CurrencyCurrentTotals[costCurrency] ===== {State.CurrencyCurrentTotals[costCurrency]},   cost ================{cost}");
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

            if (total != 0 && amount < 0)
            {
                //Debug.Log($"total = {total}");
                //Debug.Log($"amount = {amount}");
            }
           //Debug.Log($"Вычитаем total ==== {total} -------------- amount = {amount} ======================= {total + amount}");
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
                double percent = State.PercentUranus;

                ApplyBuildingPerks(currency, ref amount);
                ApplyCurrencyPerk(currency, ref amount, ref percent);
                if (State.CurrencyCurrentTotals.Count < 1)
                {
                    return amount;
                }
                return amount+= amount * (percent * State.CurrencyCurrentTotals[Config.Currencies[1]]);
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

        private void ApplyCurrencyPerk(Currency currency, ref double amount, ref double percent)
        {
            foreach (Upgrade upgrade in State.EarnedUpgrades)
            {
                foreach (UpgradePerk upgradePerk in upgrade.UpgradePerk)
                {
                    if (upgradePerk.TargetCurrency != currency)
                    {
                        Debug.Log($"upgradePerk.TargetCurrency === {upgradePerk.TargetCurrency}  and currency ====== {currency}");
                        continue;
                    }

                    switch (upgradePerk.Operation)
                    {
                        case Operation.Add:
                            percent += upgradePerk.PerCent;
                            Debug.Log( $"Прибавляю процент к урану ============================ {percent}");
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