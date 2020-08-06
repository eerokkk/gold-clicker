using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;

public class RewardedAds : MonoBehaviour
{
    private RewardedAd rewardedAd;
    private string UnityID = "ca-app-pub-3940256099942544/5224354917";

    private void Start()
    {
        MobileAds.Initialize(initStatus => { });

        rewardedAd = new RewardedAd(UnityID);

        AdRequest request = new AdRequest.Builder().Build();

        

        // Called when an ad request has successfully loaded.
        this.rewardedAd.OnAdLoaded += RewardedAd_OnAdLoaded;
        // Called when an ad request failed to load.
        this.rewardedAd.OnAdFailedToLoad += RewardedAd_OnAdFailedToLoad;
        // Called when an ad is shown.
        this.rewardedAd.OnAdOpening += RewardedAd_OnAdOpening;
        // Called when an ad request failed to show.
        this.rewardedAd.OnAdFailedToShow += RewardedAd_OnAdFailedToShow;
        // Called when the user should be rewarded for interacting with the ad.
        this.rewardedAd.OnUserEarnedReward += RewardedAd_OnUserEarnedReward;
        // Called when the ad is closed.
        this.rewardedAd.OnAdClosed += RewardedAd_OnAdClosed;
    }

    private void RewardedAd_OnAdClosed(object sender, System.EventArgs e)
    {
        Debug.Log("Я сосал хуй и вызвал он ад клозед");
    }

    private void RewardedAd_OnUserEarnedReward(object sender, Reward e)
    {
        Debug.Log("Я сосал хуй и получил вознаграждение");
    }

    private void RewardedAd_OnAdFailedToShow(object sender, AdErrorEventArgs e)
    {
        Debug.Log("Я сосал хуй и вызвал он ад фаилед ту шоу");
    }

    private void RewardedAd_OnAdOpening(object sender, System.EventArgs e)
    {
        Debug.Log("Я сосал хуй и вызвал он ад опенинг");
    }

    private void RewardedAd_OnAdFailedToLoad(object sender, AdErrorEventArgs e)
    {
        Debug.Log("Я сосал хуй и вызвал он ад фаилед ту лоад");
    }

    private void RewardedAd_OnAdLoaded(object sender, System.EventArgs e)
    {
        Debug.Log("Я сосал хуй и вызвал он лоад адс");
    }

    public void OnClickAds1Button()
    {
        AdRequest request = new AdRequest.Builder().Build();
        rewardedAd.LoadAd(request);
    }
}
