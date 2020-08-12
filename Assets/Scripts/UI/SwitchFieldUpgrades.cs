using TMPro;
using uClicker;
using UnityEngine;
using UnityEngine.UI;

public class SwitchFieldUpgrades : MonoBehaviour
{
    public GameObject FieldEquipment;
    public GameObject FieldUpgrades;
    public TextMeshProUGUI MultiplyText;
    public ClickerManager GoldManager;
    public Animator Animator;

    private int Count = 1;

    public void Start()
    {
        GoldManager.BuyMultiply = 1;
        GoldManager.BuyMax = false;
    }

    public void switchFieldEquipment()
    {
        FieldEquipment.SetActive(true);
        FieldUpgrades.SetActive(false);
    }

public void switchFieldUpgrades()
    {
        FieldUpgrades.SetActive(true);
        FieldEquipment.SetActive(false);
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
        GoldManager.OnBuyBuilding.Invoke();
        GoldManager.OnBuyUpgrade.Invoke();
    }

    public void OnClickSettingsButton()
    {
        Animator.SetBool("IsOpen", true);

    }

    public void OnClickBackButtonIsSettingsPanel()
    {
        Animator.SetBool("IsOpen", false);

    }
}

