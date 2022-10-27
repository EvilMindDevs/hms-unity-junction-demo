using UnityEngine;
using UnityEngine.UI;
using HmsPlugin;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int Coin = 0;

    public Text coin;
    public GameObject gameOverPanel,rewardAdsPanel;
    public Button home, rewardAd;

    private bool NoAds = true;
    private bool didRewardedEarned = false;

    private void Start()
    {
        if (PlayerPrefs.GetInt("NoAds") == 0) 
        {
            NoAds = false;
           // HMSAdsKitManager.Instance.HideBannerAd();
            HMSAdsKitManager.Instance.OnRewardAdCompleted = OnRewardAdCompleted;
            HMSAdsKitManager.Instance.OnRewardedAdLoaded = OnRewardedAdLoaded;
        }
    }

    public void ButtonClicked(string key) 
    {
        if(key == "RewardedAds") 
        {
            HMSAdsKitManager.Instance.ShowRewardedAd();
        }
        else if(key == "Home") 
        {
            HMSLeaderboardManager.Instance.SubmitScore(HMSLeaderboardConstants.LeaderBoard, Coin);
            SceneManager.LoadScene(0);
        }
    }

    internal void Finish()
    {
        HMSAchievementsManager.Instance.Grow(HMSAchievementConstants.nineLives,1);

        gameOverPanel.gameObject.SetActive(true);
        coin.text = Coin.ToString();
        if (!NoAds) 
        {
            HMSAdsKitManager.Instance.ShowBannerAd();
            if (HMSAdsKitManager.Instance.IsInterstitialAdLoaded) 
            {
                HMSAdsKitManager.Instance.ShowInterstitialAd();
            }

            if (HMSAdsKitManager.Instance.IsRewardedAdLoaded) 
            {
                rewardAdsPanel.gameObject.SetActive(true);
            }
            else 
            {
                HMSAdsKitManager.Instance.LoadRewardedAd();
            }
        }
    }

    private void OnRewardedAdLoaded()
    {
        if(!didRewardedEarned) // With this, you can prevent your users from watching your rewarded ads repeatedly. Will be reload rewarded ads after watching it.
            rewardAdsPanel.gameObject.SetActive(true);
    }

    private void OnRewardAdCompleted()
    {
        rewardAdsPanel.gameObject.SetActive(false);
        Coin++;
        coin.text = Coin.ToString();
        didRewardedEarned = true;
    }
}
