using System.Linq;
using uClicker;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.PlayerLoop;

public class UpdateInfo : MonoBehaviour
{
    public ClickerManager Manager;
    public TextMeshProUGUI Money;
    public TextMeshProUGUI Upgrades;
    public TextMeshProUGUI Buildings;
    public TextMeshProUGUI IncomePerSecond;

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
        //Debug.Log(Manager.State.CurrencyCurrentTotals);
        Money.text = string.Join(", ",
            Manager.State.CurrencyCurrentTotals.Select((kvp) =>
                string.Format("{0}", kvp.Value)).ToArray());
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