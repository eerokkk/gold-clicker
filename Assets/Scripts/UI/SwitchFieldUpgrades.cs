using System.Collections;
using System.Collections.Generic;
using TMPro;
using uClicker;
using UnityEditor.SceneManagement;
using UnityEngine;

public class SwitchFieldUpgrades : MonoBehaviour
{
    public GameObject FieldEquipment;
    public GameObject FieldUpgrades;
    public TextMeshProUGUI MultiplyText;
    public ClickerManager GoldManager;
    public int BuyMultiply;
    public Animator Animator;


    [SerializeField]
    private bool Activity = true;
    private int Count = 1;

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
                BuyMultiply = 1;
                MultiplyText.text = $"x{BuyMultiply}";
                Count++;
                break;
            case 1:
                BuyMultiply = 10;
                MultiplyText.text = $"x{BuyMultiply}";
                Count++;
                break;
            case 2:
                BuyMultiply = 100;
                MultiplyText.text = $"x{BuyMultiply}";
                Count++;
                break;
            case 3:
                MultiplyText.text = "Max";
                Count = 0;
                break;
        }

        GoldManager.OnTick.Invoke();
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

