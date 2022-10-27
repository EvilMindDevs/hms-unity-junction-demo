using HuaweiMobileServices.IAP;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HmsPlugin;

public class StoreManagement : MonoBehaviour
{
    public GameObject product1, product2, product3;

    private int Counter = 0; 

    public void OnObtainProductInfoSuccess(IList<ProductInfoResult> list)
    {
        foreach (ProductInfoResult res in list)
            for(int i = 0; i < res.ProductInfoList.Count; i++) 
            {
                switch (Counter++)
                {
                    case 0:
                        FillProduct(product1, res.ProductInfoList[i]);
                        break;
                    case 1:
                        FillProduct(product2, res.ProductInfoList[i]);
                        break;
                    case 2:
                        FillProduct(product3, res.ProductInfoList[i]);
                        break;
                }
            }
    }

    private void FillProduct(GameObject product, ProductInfo productInfo) 
    {
        Debug.Log("FillProduct" + product.name + " - productInfo" + productInfo.ProductName);
        product.SetActive(true);
        product.transform.GetChild(0).gameObject.GetComponent<Text>().text = productInfo.ProductName;
        product.transform.GetChild(1).gameObject.GetComponent<Text>().text = productInfo.ProductDesc;
        product.transform.GetChild(3).gameObject.GetComponent<Text>().text = productInfo.Price + productInfo.Currency;
        product.transform.GetChild(4).gameObject.GetComponent<Button>().onClick.AddListener(delegate { BuyProduct(productInfo.ProductId); }); 
    }

    private void BuyProduct(string productId)
    {
        HMSIAPManager.Instance.OnBuyProductSuccess = OnBuyProductSuccess;
        HMSIAPManager.Instance.OnBuyProductFailure = OnBuyProductFailure;
        HMSIAPManager.Instance.BuyProduct(productId);
    }

    private void OnBuyProductFailure(int obj)
    {
        Debug.LogError("OnBuyProductFailure:" + obj);
    }

    private void OnBuyProductSuccess(PurchaseResultInfo obj)
    {
        Debug.Log("OnBuyProductSuccess:" + obj.InAppPurchaseData.ProductName);
        if (obj.InAppPurchaseData.ProductId == HMSIAPConstants.NoAds)
        {
            // Hide banner Ad for example
            HMSAdsKitManager.Instance.HideBannerAd();
            PlayerPrefs.SetInt("NoAds", 1);
            //Create your own logic to hide your purchased product in the store panel.
        }
        else if (obj.InAppPurchaseData.ProductId == HMSIAPConstants._100Coin)
        {
            // Give your player coins here.
        }
        else if (obj.InAppPurchaseData.ProductId == HMSIAPConstants.Premium)
        {
            // Grant your player premium feature.
            //In this example premium reveals hiddenAchievements
            HMSAchievementsManager.Instance.RevealAchievement(HMSAchievementConstants.allCompleted);
        }
    }
}
