using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Percent : MonoBehaviour
{
    public TextMeshProUGUI percent;
    public Image bar_treyg;

    void Start()
    {
        if (PlayerPrefs.GetInt("Completed") > 300)
            bar_treyg.fillAmount = 300 / 300.0f;
        else
            bar_treyg.fillAmount = PlayerPrefs.GetInt("Completed") / 300.0f;
        float count = PlayerPrefs.GetInt("Completed") / 300.0f * 100;
        int res = (int) count;
        if (res > 100)
            res = 100;
        percent.text = res.ToString() + "%";
    }
}
