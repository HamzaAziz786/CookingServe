using System;
using TMPro;
using UnityEngine;
using DG.Tweening;
public class SAvingMAnager : MonoBehaviour
{
    public TMP_Text Moneytext;
    public static SAvingMAnager savinginstance;
    private float loadingtween;
    private void Awake()
    {
        savinginstance = this;
        if (savinginstance==null)
        {
            savinginstance = this;
        }
    }

    private void Start()
    {
        ShowMoney();
    }

    public void ShowMoney()
    {
        Moneytext.text = PlayerPrefs.GetInt("Money").ToString();
        DOTween.To(() => loadingtween, x => loadingtween = x,PlayerPrefs.GetInt("Money"), 8f)
            .OnUpdate(() =>
            {
                Moneytext.text = loadingtween.ToString("00");
            });
    }
    public void AddMoney(int moneyAdd)
    {
        PlayerPrefs.SetInt("Money",PlayerPrefs.GetInt("Money")+moneyAdd);
        ShowMoney();
    }
    public void DeductMoney(int moneydeduct)
    {
        PlayerPrefs.SetInt("Money",PlayerPrefs.GetInt("Money")-moneydeduct);
        ShowMoney();
    }
}
