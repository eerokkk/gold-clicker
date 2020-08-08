using System.Collections;
using System.Collections.Generic;
using TMPro;
using uClicker;
using UnityEngine;
public class SwitchFieldUpgrades : MonoBehaviour
{
    public GameObject FieldEquipment;
    public GameObject FieldUpgrades;
    public TextMeshProUGUI MultiplyText;
    public ClickerManager GoldManager;

    [SerializeField]
    private bool Activity = true;
    private int Count = 1;

    public void Start()
    {
        GoldManager.BuyMultiply = 1;
        GoldManager.BuyMax = false;
    }

    public void switchFieldEquipment()
    {
        switch (Activity)
        {
            case true:
                FieldEquipment.SetActive(true);
                FieldUpgrades.SetActive(false);
                Activity = false;
                break;
            case false:
                break;
        }
    }

public void switchFieldUpgrades()
    {
        switch (Activity == false)
        {
            case true:
                FieldEquipment.SetActive(false);
                FieldUpgrades.SetActive(true);
                Activity = true;
                break;
            case false:
                break;
        }
    }

    public void changeTextInMultiplyButton()
    {
        switch (Count)
        {
            case 0:
                GoldManager.BuyMax = false;
                GoldManager.BuyMultiply = 1;
                MultiplyText.text = $"x{GoldManager.BuyMultiply}";
                Count++;
                break;
            case 1:
                GoldManager.BuyMultiply = 10;
                MultiplyText.text = $"x{GoldManager.BuyMultiply}";
                Count++;
                break;
            case 2:
                GoldManager.BuyMultiply = 100;
                MultiplyText.text = $"x{GoldManager.BuyMultiply}";
                Count++;
                break;
            case 3:
                GoldManager.BuyMultiply = 0;
                GoldManager.BuyMax = true;
                MultiplyText.text = "Max";
                Count = 0;
                break;
        }

        GoldManager.OnTick.Invoke();
    }
}
