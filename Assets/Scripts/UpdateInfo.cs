using System;
using System.Linq;
using uClicker;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.PlayerLoop;
using System.Collections;

public class UpdateInfo : MonoBehaviour
{
    public ClickerManager Manager;
    public TextMeshProUGUI Money;
    public TextMeshProUGUI Upgrades;
    public TextMeshProUGUI Buildings;
    public TextMeshProUGUI IncomePerSecond;
    public TextMeshProUGUI CurrentUraniumBonus;
    public TextMeshProUGUI GetUranusCount;

    private float splitedOldValueGold;
    private float splitedCurrentValueGold;

    void Start()
    {
        Manager.OnTick.AddListener(OnTick);
        Manager.OnBuyBuilding.AddListener(OnBuyBuilding);
        Manager.OnBuyUpgrade.AddListener(OnBuyUpgrade);
        OnTick();
        OnBuyBuilding();
        OnBuyUpgrade();
    }

    private void OnTick()
    {
        UpdateCurrentUraniumBonus();
        UpdateManyText();
    }

    private void UpdateManyText()
    {
        if (Manager.State.CurrencyCurrentTotals != null)
            Money.text = LargeNumber.ToString(Manager.State.CurrencyCurrentTotals[Manager.Config.Currencies[0]]);
        else if (Money != null) Money.text = "0";
    }

    private void UpdateCurrentUraniumBonus()
    {
        if (Manager.State.CurrencyHistoricalTotals.ContainsKey(Manager.Config.Currencies[0]))
            if (CurrentUraniumBonus != null)
                CurrentUraniumBonus.text =
                    $"You will get: {LargeNumber.ToString(Manager.State.UraniumIncrease)} uranium";
        if (GetUranusCount != null)
            GetUranusCount.text = $"Percent uranium bonus :{Manager.State.PercentUranus}%";
    }
    
    private void OnBuyUpgrade()
    {
        if (Upgrades != null)
            Upgrades.text = "Current Upgrades: " +
                        string.Join(", ", Manager.State.EarnedUpgrades.Select(upgrade => upgrade.name).ToArray());
    }

    private void OnBuyBuilding()
    {
        if (Buildings != null)
        Buildings.text = "Current Buildings: " + string.Join(", ",
            Manager.State.EarnedBuildings.Select((kvp) =>
                string.Format("{0} {1}", kvp.Key.name, kvp.Value)).ToArray());
    }


}