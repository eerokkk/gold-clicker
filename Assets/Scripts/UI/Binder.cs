using System.Collections;
using System.Linq;
using TMPro;
using uClicker;
using UnityEngine;
using UnityEngine.UI;

public class Binder : MonoBehaviour
{
    [SerializeField] private ClickerManager _clickerManager;

    public TextMeshProUGUI Name;
    public TextMeshProUGUI Cost;
    public TextMeshProUGUI Description;
    public TextMeshProUGUI UpdateTypeCount;
    public Sprite Miner;
    public Sprite Pick;
    public GameObject PrefabIcon;
    private UnlockableComponent _clickerComponent;
    public Button _button;

    public void Buy()
    {
        if (_clickerComponent is Upgrade)
        {
            _clickerManager.BuyUpgrade(_clickerComponent as Upgrade);
        }
        else
        {
            _clickerManager.BuyBuilding(_clickerComponent as Building);
        }
    }

    public void Bind(Upgrade availableUpgrade)
    {
        _clickerComponent = availableUpgrade;
        this.Name.text = availableUpgrade.name;
        this.Cost.text = LargeNumber.ToString(availableUpgrade.Cost.Amount);
        this.Description.text = GenerateUpgradeString(availableUpgrade.UpgradePerk);
        //_button = this.GetComponent<Button>();
        _clickerManager.OnTick.AddListener(() => IsActive());
        IsActive();
    }

    public void Bind(Building availableBuilding)
    {
        _clickerComponent = availableBuilding;
        this.Name.text = availableBuilding.name;
        this.Description.text = string.Format("+{0} {1}s per second", LargeNumber.ToString(availableBuilding.YieldAmount.Amount),
            availableBuilding.YieldAmount.Currency.name);
        try
        {
            this.UpdateTypeCount.text = _clickerManager.State.BuildingCountType[availableBuilding.BuildingType].ToString();

        }
        catch (System.Exception)
        {
            if(this.UpdateTypeCount != null)
                this.UpdateTypeCount.text = "";
            //throw;
        }

        //_button = this.GetComponent<Button>();
        _clickerManager.OnTick.AddListener(() => IsActive(availableBuilding));
        IsActive(availableBuilding);
        
    }

    public void pushSpriteInBuilding(Building availableBuilding)
    {
        string BuildingName = availableBuilding.name;
        switch (BuildingName)
        {
            case "Miner":
                PrefabIcon.GetComponent<Image>().sprite = Miner;
                break;
        }  
    }

    public void pushSpriteInUpgrade(Upgrade availableUpgrade)
    {
        string UpgradeName = availableUpgrade.name;
        switch (UpgradeName)
        {
            case "Pick":
                PrefabIcon.GetComponent<Image>().sprite = Pick;
                break;
        }
    }

    private void IsActive(Building availableBuilding = null)
    {
        if (availableBuilding != null)
            this.Cost.text = LargeNumber.ToString(_clickerManager.BuildingCost(availableBuilding).Amount);

        _button.interactable = _clickerManager.CanBuy(_clickerComponent);
    }

    private string GenerateUpgradeString(UpgradePerk[] availableUpgradeUpgradePerk)
    {
        string text = "";
        foreach (var upgradePerk in availableUpgradeUpgradePerk)
        {
            ClickerComponent component = upgradePerk.TargetBuilding ??
                                         (ClickerComponent) upgradePerk.TargetClickable ?? upgradePerk.TargetCurrency;
            text += string.Format("{0}s {1} to {2}", upgradePerk.Operation, upgradePerk.Amount, component.name);
        }

        return text;
    }
}