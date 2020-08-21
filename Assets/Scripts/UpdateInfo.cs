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

    private string oldValueGold;
    private string currentValueGold;
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
        currentValueGold = LargeNumber.ToString(Manager.State.CurrencyCurrentTotals[Manager.Config.Currencies[0]]);
        for (int i = 0; i < 10; i++)
        {
            var adfsniu = Manager.State.CurrencyCurrentTotals[Manager.Config.Currencies[0]];
            Money.text = LargeNumber.ToString(Manager.State.CurrencyCurrentTotals[Manager.Config.Currencies[0]]);
        }
        //if (!string.IsNullOrEmpty(oldValueGold))
        //{
        //    var splitParseValue = oldValueGold.Split(' ');
        //    splitedOldValueGold = float.Parse(splitParseValue[0]);
        //    splitedCurrentValueGold = float.Parse(currentValueGold.Split(' ')[0]);
        //}
        ////StartCoroutine(Lerpatel());
        ////Debug.Log(Manager.State.CurrencyCurrentTotals);
        ////Money.text = LargeNumber.ToString(Manager.State.CurrencyCurrentTotals[Manager.Config.Currencies[0]]);
        oldValueGold = LargeNumber.ToString(Manager.State.CurrencyCurrentTotals[Manager.Config.Currencies[0]]);
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

    //IEnumerator Lerpatel()
    //{

    //        yield return new WaitForSeconds(0.1f);
    //        //var Lerp = Mathf.Lerp(splitedOldValueGold, splitedCurrentValueGold, 0.3f);
    //        //Debug.Log(Lerp);
    //        //Money.text = Lerp.ToString();
    //}
}