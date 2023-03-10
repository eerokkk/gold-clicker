using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uClicker;

public class ResetProgress : MonoBehaviour
{
    private ClickerManager manager;

    public void Reset(ClickerManager manager)
    {


        manager.State.EarnedBuildings.Clear();
        manager.State.EarnedUpgrades.Clear();
        foreach (var availableBuilding in manager.Config.AvailableBuildings)
        {
            availableBuilding.Unlocked = false;
        }

        foreach (var availableUpgrade in manager.Config.AvailableUpgrades)
        {
            availableUpgrade.Unlocked = false;
        }

        manager.State.CurrencyCurrentTotals[manager.Config.Currencies[1]] += manager.State.UraniumIncrease;
        manager.State.CurrencyCurrentTotals[manager.Config.Currencies[0]] = 0d;
        manager.State.CurrencyHistoricalTotals[manager.Config.Currencies[0]] = 0d;
        manager.State.BuildingCountType.Clear();

        manager.Config.AvailableBuildings[0] = manager.Progresive.Miners[0];
        manager.Config.AvailableBuildings[1] = manager.Progresive.Carts[0];
        manager.Config.AvailableBuildings[2] = manager.Progresive.Factories[0];
        manager.Config.AvailableBuildings[3] = manager.Progresive.Drills[0];
        manager.Config.AvailableBuildings[4] = manager.Progresive.Cities[0];
        manager.Config.AvailableBuildings[5] = manager.Progresive.Mines[0];

        manager.State.PercentUranus = 0.01d;

    }
}
