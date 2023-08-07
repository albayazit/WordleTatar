using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class RewardedAd : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] private Button myButton;
    
    [SerializeField] private string androidID = "Rewarded_Android";
    [SerializeField] private string iosID = "Rewarded_iOS";

    private string adID;
    public GameObject noAds;

    public Board board;

    void Awake() { 
        adID = (Application.platform == RuntimePlatform.IPhonePlayer) ? iosID : androidID;
        LoadAd();
    }

    public void LoadAd() {
        Debug.Log("Loading Ad: " + adID);
        Advertisement.Load(adID, this);
    }

    public void ShowAd() {
        Debug.Log("Showing Ad: " + adID);
        Advertisement.Show(adID, this);
        if (PlayerPrefs.GetInt("ADS") == 0) {
            noAds.SetActive(true);
        }
    }
    public void OnInitializationComplete()
    {
        Debug.Log("Init Success");
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Init Failed: [{error}]: {message}");
    }

    public void OnUnityAdsAdLoaded(string placementId)
    {
        Debug.Log($"Load Success: {placementId}");
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Load Failed: [{error}:{placementId}] {message}");
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        Debug.Log($"OnUnityAdsShowFailure: [{error}]: {message}");
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        Debug.Log($"OnUnityAdsShowStart: {placementId}");
    }

    public void OnUnityAdsShowClick(string placementId)
    {
        Debug.Log($"OnUnityAdsShowClick: {placementId}");
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        if (showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED)) {
            Debug.Log($"Rewarded: {placementId}");
            LoadAd();
            board.WinGame();
        }
    }

    private void OnDestroy() {
        myButton.onClick.RemoveAllListeners();
    }
}
