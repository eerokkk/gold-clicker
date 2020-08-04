﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uClicker;

public class ResetProgress : MonoBehaviour
{
    ClickerManager manager;
    public void Reset(ClickerManager manager)
    {


        manager.State.EarnedBuildings.Clear();
        manager.State.EarnedUpgrades.Clear();
        foreach (Building availableBuilding in manager.Config.AvailableBuildings)
        {
            availableBuilding.Unlocked = false;
        }

        foreach (Upgrade availableUpgrade in manager.Config.AvailableUpgrades)
        {
            availableUpgrade.Unlocked = false;
        }

        var sumUran = Mathf.Round(manager.State.CurrencyCurrentTotals[manager.Config.Currencies[0]] / 1000000);
        manager.State.CurrencyCurrentTotals[manager.Config.Currencies[1]] += sumUran;
        manager.State.CurrencyCurrentTotals[manager.Config.Currencies[0]] = 0;
        manager.State.BuildingCountType.Clear();

        manager.Config.AvailableBuildings[0] = manager.Progresive.Miners[0];
        manager.Config.AvailableBuildings[1] = manager.Progresive.Carts[0];
        manager.Config.AvailableBuildings[2] = manager.Progresive.Factories[0];
        manager.Config.AvailableBuildings[3] = manager.Progresive.Drills[0];
        manager.Config.AvailableBuildings[4] = manager.Progresive.Cities[0];
        manager.Config.AvailableBuildings[5] = manager.Progresive.Mines[0];



    }



   
}