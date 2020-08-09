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
        Appodeal.show(Appodeal.REWARDED_VIDEO);
    }

    #region Rewarded Video callback handlers

    public void onRewardedVideoLoaded(bool isPrecache)
    {
        print("Video loaded");
    }

    public void onRewardedVideoFailedToLoad()
    {
        print("Video failed");
    } // Вызывается, когда видео с наградой за просмотр не загрузилось

    public void onRewardedVideoShowFailed()
    {
        print("RewardedVideo show failed");
    } // Вызывается, когда видео с наградой загрузилось, но не может быть показано (внутренние ошибки сети, настройки плейсментов или неверный креатив)

    public void onRewardedVideoShown()
    {
        print("Video shown");
    } // Вызывается после показа видео с наградой за просмотр

    public void onRewardedVideoClicked()
    {
        print("Video clicked");
    } // Вызывается при клике на видео с наградой за просмотр

    public void onRewardedVideoClosed(bool finished)
    {
        print("Video closed");
    } // Вызывается при закрытии видео с наградой за просмотр

    public void onRewardedVideoFinished(double amount, string name)
    {
        print("Reward: " + amount + " " + name);
    } // Вызывается, если видео с наградой за просмотр просмотрено полностью

    public void onRewardedVideoExpired()
    {
        print("Video expired");
    } // Вызывается, когда видео с наградой за просмотр больше не доступно.
    #endregion
}
