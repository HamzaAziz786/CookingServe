using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class ServantRequirement : MonoBehaviour
{
    Servant servant;


    [Header("---UI---")]
    public GameObject UIRequirement;
    public Text txtRequirement;
    Vector3 DefaultUIScale = new Vector3(0.007966162f, 0.008433727f, 0.01300598f);
    // Start is called before the first frame update
    void Start()
    {
        servant = this.GetComponent<Servant>();
        servant.OnReachReciption += Servant_OnReachReciption;
        servant.OnOrderDelivered += Servant_OnOrderDelivered;
    }

    private void Servant_onTimerComplete(bool obj)
    {

        HideRequirement();
    }

    private void Servant_OnOrderDelivered()
    {
        HideRequirement();
    }

    private void Servant_OnReachReciption(ServRequirement[] reqirement)
    {
        //ShowRequirement();
        //txtRequirement.text = reqirement;
    }

    private void OnDisable()
    {
        servant.OnReachReciption -= Servant_OnReachReciption;
        servant.OnOrderDelivered -= Servant_OnOrderDelivered;
    }

    void ShowRequirement()
    {
        UIRequirement.transform.DOScale(DefaultUIScale, 0.3f).SetEase(Ease.OutBack);
    }
    void HideRequirement()
    {
        UIRequirement.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.OutBack);
    }
}
