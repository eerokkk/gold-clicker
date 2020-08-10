using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AppodealAds.Unity.Api;
using AppodealAds.Unity.Common;

public class AdsAppo : MonoBehaviour
{
    private void Start()
    {
        Appodeal.initialize("5ead9cf76450f91ff20489b355e0dab3a9a02ea7c448bd44", Appodeal.REWARDED_VIDEO, true);
    }

    public void OnClickOneButton()
    {
        Appodeal.show(Appodeal.REWARDED_VIDEO);
    }
}
