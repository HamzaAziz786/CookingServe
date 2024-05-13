using System;
using TMPro;
using UnityEngine;
using DG.Tweening;
public class SAvingMAnager : MonoBehaviour
{
    public TMP_Text Moneytext;
    public static SAvingMAnager savinginstance;
    private float loadingtween;

    public GameObject Upgrationpanel;
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

    public void UpgrationPanel_active()
    {
        Upgrationpanel.SetActive(true);
        Upgrationpanel.transform.DOScale(1, 1).SetEase(Ease.InBounce);
        Upgrationpanel.transform.DOScale(0, 1).SetDelay(5f).SetEase(Ease.OutBounce) .OnComplete(()=> PanelSCale());
        
    }

    public void PanelSCale()
    {
        Upgrationpanel.SetActive(false);
        Upgrationpanel.transform.DOScale(0, 1).SetEase(Ease.InBounce);
    }
}
