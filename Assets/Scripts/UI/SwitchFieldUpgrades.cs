using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SwitchFieldUpgrades : MonoBehaviour
{
    public GameObject FieldEquipment;
    public GameObject FieldUpgrades;

    [SerializeField]
    private bool Activity = true;

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
}
