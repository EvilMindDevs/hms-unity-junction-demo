using HmsPlugin;
using HuaweiMobileServices.Ads;
using HuaweiMobileServices.IAP;
using HuaweiMobileServices.Id;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public Text welcomeText;
    public GameObject storePanel, mainMenuPanel;

    private bool IapAvailable = false;
    void Start()
    {
        if(PlayerPrefs.GetInt("NoAds")==0)
        {
            HMSAdsKitManager.Instance.ConsentOnFail = OnConsentFail;
            HMSAdsKitManager.Instance.ConsentOnSuccess = OnConsentSuccess;
            HMSAdsKitManager.Instance.RequestConsentUpdate();
            #region SetNonPersonalizedAd , SetRequestLocation

            var builder = HwAds.RequestOptions.ToBuilder();

            builder
                .SetConsent("tcfString")
                .SetNonPersonalizedAd((int)NonPersonalizedAd.ALLOW_ALL)
                .Build();

            bool requestLocation = true;
            var requestOptions = builder.SetConsent("testConsent").SetRequestLocation(requestLocation).Build();
            #endregion
        }

        HMSAccountKitManager.Instance.OnSignInSuccess = OnSignInSuccess;
        Login();
    }

    private void OnSignInSuccess(AuthAccount obj)
    {
        Debug.Log("SignInSuccess Success");
        welcomeText.text = "Welcome " + obj.DisplayName;

        //Init IAP after OnSignInSuccess
        InitIAP();

        //You need to call init first for gameService
        HMSGameServiceManager.Instance.Init();
    }

    private void InitIAP() 
    {
        //Uncheck Init on start box in IAP page
        HMSIAPManager.Instance.OnCheckIapAvailabilitySuccess = OnCheckIapAvailabilitySuccess;
        HMSIAPManager.Instance.OnObtainProductInfoSuccess = GetComponent<StoreManagement>().OnObtainProductInfoSuccess;
        HMSIAPManager.Instance.CheckIapAvailability();
    }

    private void OnCheckIapAvailabilitySuccess()
    {
        IapAvailable = true;
        HMSIAPManager.Instance.OnObtainOwnedPurchasesSuccess = OnObtainOwnedPurchasesSuccess;
        HMSIAPManager.Instance.ObtainAllOwnedPurchases();
    }

    private void OnObtainOwnedPurchasesSuccess(OwnedPurchasesResult result)
    {
        Debug.Log("OnObtainOwnedPurchasesSuccess");

        if (result != null)
        {
            foreach (var obj in result.InAppPurchaseDataList)
            {
                if (obj.ProductId == HMSIAPConstants.NoAds)
                {
                    PlayerPrefs.SetInt("NoAds", 1);
                    //Create your own logic to hide your purchased product in the store panel.
                }
                else if (obj.ProductId == HMSIAPConstants._100Coin)
                {
                    //Consume your Consumable products. It is Mandatory
                    //https://developer.huawei.com/consumer/en/doc/development/HMSCore-Guides/redelivering-consumables-0000001051356573
                    //https://github.com/EvilMindDevs/hms-unity-plugin/blob/master/Assets/Huawei/Demos/IAP/IapDemoManager.cs#L88
                }
            }
        }
    }

    private void OnConsentSuccess(ConsentStatus arg1, bool arg2, IList<AdProvider> arg3)
    {
        Debug.Log("Consent Success");
    }

    private void OnConsentFail(string error)
    {
        Debug.LogError("Consent Failed:"+ error);
    }
    internal static class Buttons
    {
        public const int Play = 0;
        public const int Store = 1;
        public const int LeaderBoard = 2;
        public const int Achievement = 3;
        public const int Login = 4;
        public const int Back = 5;
    }
    public void OnButtonClicked(int key) 
    {
        switch (key) 
        {
            case Buttons.Play:
                SceneManager.LoadScene(1);
                break;
            case Buttons.Store:
                StoreButton();
                break;
            case Buttons.LeaderBoard:
                LeaderBoardButton();
                break;
            case Buttons.Achievement:
                AchievementButton();
                break;
            case Buttons.Login:
                Login();
                break;
            case Buttons.Back:
                Back();
                break;
            default:
                Debug.Log("OnButtonClicked default case:" + key);
                break;
        }
    }

    private void Back()
    {
        mainMenuPanel.SetActive(true);
        storePanel.SetActive(false);
    }

    private void AchievementButton()
    {
        HMSAchievementsManager.Instance.ShowAchievements();
    }

    private void LeaderBoardButton()
    {
        HMSLeaderboardManager.Instance.ShowLeaderboards();
    }

    private void StoreButton()
    {
        if (IapAvailable) 
        {
            mainMenuPanel.SetActive(false);
            storePanel.SetActive(true);
        }
        else 
        {
            Debug.LogError("StoreButton IapAvailable false");
        }
    }

    private void Login()
    {
        var account = HMSAccountKitManager.Instance;
        if (account.IsSignedIn) 
        {
            Debug.Log("account SignedIn");
            welcomeText.text = "Welcome " + account.HuaweiId.DisplayName;

            //Init IAP for Obtain products
            InitIAP();
        }
        else 
        {
            Debug.Log("Login SignIn");
            account.SignIn();
        }
    }

    



}
