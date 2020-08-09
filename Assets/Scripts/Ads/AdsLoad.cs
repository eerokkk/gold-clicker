using System;
using System.Collections;
using System.Collections.Generic;
using AppodealAds.Unity.Api;
using AppodealAds.Unity.Common;
using TMPro;
using UnityEngine;

public class AdsLoad : MonoBehaviour, IInterstitialAdListener
{
    public TextMeshProUGUI colbs; 
    
    // Start is called before the first frame update
    void Start()
    {
        Appodeal.initialize("c8b2720d60c73b49f31780a3aa1a46cdffd20c7c04e775c3",
            Appodeal.INTERSTITIAL | Appodeal.REWARDED_VIDEO,
            true);
    }

    public void onInterstitialLoaded(bool isPrecache)
    {
        colbs.text += "onInterstitialLoaded\r\n";
    }

    public void onInterstitialFailedToLoad()
    {
        colbs.text += "onInterstitialFailedToLoad\r\n";
    }

    public void onInterstitialShowFailed()
    {
        colbs.text += "onInterstitialShowFailed\r\n";
    }

    public void onInterstitialShown()
    {
        colbs.text += "onInterstitialShown\r\n";
    }

    public void onInterstitialClosed()
    {
        colbs.text += "onInterstitialClosed\r\n";
    }

    public void onInterstitialClicked()
    {
        colbs.text += "onInterstitialClicked\r\n";
    }

    public void onInterstitialExpired()
    {
        colbs.text += "onInterstitialExpired\r\n";
    }
}

