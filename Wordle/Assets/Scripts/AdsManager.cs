using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdsManager : MonoBehaviour, IUnityAdsInitializationListener
{
    [SerializeField] string androidID = "X";
    [SerializeField] string iosID = "X";
    [SerializeField] bool testMode = false;
    private string gameID;

    void Awake() {
        InitializeAds();
    }

    public void InitializeAds() {
        if (!Advertisement.isInitialized && Advertisement.isSupported)
        {
            gameID = (Application.platform == RuntimePlatform.IPhonePlayer) ? iosID : androidID;
            Advertisement.Initialize(gameID, testMode, this);
        }
    }

    public void OnInitializationComplete() {
        Debug.Log("Unity ADS: Initialization complete");
        PlayerPrefs.SetInt("ADS", 1);
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message) {
        PlayerPrefs.SetInt("ADS", 0);
        Debug.Log($"Unity ADS: Initialization error {error.ToString()} - {message}");
    }
}
