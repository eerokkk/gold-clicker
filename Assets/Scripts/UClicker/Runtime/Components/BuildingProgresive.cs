using System;
using System.Collections;
using System.Collections.Generic;
using uClicker;
using UnityEngine;

[CreateAssetMenu(menuName = "uClicker/BuildingProgressive")]
[Serializable]
public class BuildingProgresive : ScriptableObject
{
    public int CountBuildings;
    public Building[] Miners;
    public Building[] Carts;
    public Building[] Factories;
    public Building[] Drills;
    public Building[] Cities;
    public Building[] Mines;

    public Dictionary<BuildingType, Building[]> ABP;

    public void FillABP()
    {
        ABP = new Dictionary<BuildingType, Building[]>();
        ABP.Add(BuildingType.Miner, Miners);
        ABP.Add(BuildingType.Transport, Carts);
        ABP.Add(BuildingType.Factory, Factories);
        ABP.Add(BuildingType.Drill, Drills);
        ABP.Add(BuildingType.City, Cities);
        ABP.Add(BuildingType.GoldMine, Mines);

    }
}
