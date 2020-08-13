using System;
using System.Collections;
using System.Collections.Generic;
using AppodealAds.Unity.Api;
using AppodealAds.Unity.Common;
using TMPro;
using UnityEngine;

public class RewardAds : MonoBehaviour, IRewardedVideoAdListener
{
    public TextMeshProUGUI colbs;

    private void Start()
    {
        Appodeal.setRewardedVideoCallbacks(this);
    }

    public void ShowRewardAds()
    {
        colbs.text = "";
        Appodeal.show(Appodeal.REWARDED_VIDEO);
    }

    #region Rewarded Video callback handlers

    public void onRewardedVideoLoaded(bool isPrecache)
    {
        print("Video loaded");
        colbs.text += "onRewardedVideoLoaded\r\n";

    }

    public void onRewardedVideoFailedToLoad()
    {
        print("Video failed");
        colbs.text += "onRewardedVideoFailedToLoad\r\n";

    } // Вызывается, когда видео с наградой за просмотр не загрузилось

    public void onRewardedVideoShowFailed()
    {
        print("RewardedVideo show failed");
        colbs.text += "onRewardedVideoShowFailed\r\n";

    } // Вызывается, когда видео с наградой загрузилось, но не может быть показано (внутренние ошибки сети, настройки плейсментов или неверный креатив)

    public void onRewardedVideoShown()
    {
        print("Video shown");
        colbs.text += "onRewardedVideoShown\r\n";

    } // Вызывается после показа видео с наградой за просмотр

    public void onRewardedVideoClicked()
    {
        print("Video clicked");
        colbs.text += "onRewardedVideoClicked\r\n";

    } // Вызывается при клике на видео с наградой за просмотр

    public void onRewardedVideoClosed(bool finished)
    {
        print("Video closed");
        colbs.text += "onRewardedVideoClosed\r\n";

    } // Вызывается при закрытии видео с наградой за просмотр

    public void onRewardedVideoFinished(double amount, string name)
    {
        print("Reward: " + amount + " " + name);
        colbs.text += "onRewardedVideoFinished\r\n";

    } // Вызывается, если видео с наградой за просмотр просмотрено полностью

    public void onRewardedVideoExpired()
    {
        print("Video expired");
        colbs.text += "onRewardedVideoExpired\r\n";

    } // Вызывается, когда видео с наградой за просмотр больше не доступно.
    #endregion
}
