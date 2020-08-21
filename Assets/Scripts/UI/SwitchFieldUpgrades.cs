using System;
using TMPro;
using uClicker;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

public class SwitchFieldUpgrades : MonoBehaviour
{
    public TextMeshProUGUI IncomePerSecond;
    public GameObject FieldEquipment;
    public GameObject FieldUpgrades;
    public TextMeshProUGUI MultiplyText;
    public ClickerManager GoldManager;
    public Animator MainAnimator;


    [SerializeField]
    private bool Activity = true;
    private int Count = 1;
    private bool AnimationCheck = true;

    public void Start()
    {
        GoldManager.BuyMultiply = 1;
        GoldManager.BuyMax = false;
        GoldManager.OnTick.AddListener(gavno);
    }

    private void gavno()
    {
        IncomePerSecond.text = $"{LargeNumber.ToString(GoldManager.State.PerSecondAmount)} per second";
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
        MainAnimator.SetBool("isSettingsButtonPush", true);
        MainAnimator.SetBool("NewState", false);
        AnimationCheck = true;
    }

    public void OnClickBackButtonIsSettingsPanel()
    {
        switch (AnimationCheck)
        {
            case true:
                MainAnimator.SetBool("isSettingsButtonPush", false);
                MainAnimator.SetBool("NewState", true);
                AnimationCheck = false;
                break;
            case false:
                MainAnimator.SetBool("isResetButtonPush", false);
                MainAnimator.SetBool("NewState", true);
                AnimationCheck = true;
                break;
        }
    }

    public void OnClickResetButton()
    {
        MainAnimator.SetBool("isResetButtonPush", true);
        MainAnimator.SetBool("NewState", false);
        AnimationCheck = false;
    }

    //public void OnClickBackResetButton()
    //{
    //    MainAnimator.SetBool("isResetButtonPush", false);
    //    MainAnimator.SetBool("NewState", true);
    //}
}