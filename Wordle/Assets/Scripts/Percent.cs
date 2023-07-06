using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Percent : MonoBehaviour
{
    public TextMeshProUGUI percent;
    public Image bar;

    void Start()
    {
        float count = PlayerPrefs.GetInt("Completed") / 1500.0f * 100;
        bar.fillAmount = PlayerPrefs.GetInt("Completed") / 1500.0f;
        int res = (int) count;
        percent.text = res.ToString() + "%";
    }
}
