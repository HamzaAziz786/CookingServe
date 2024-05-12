using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.UI;

public class RecipeObject : MonoBehaviour, IRecipeObject
{
    public bool HideAble;


    Vector3 previousScale = Vector3.one;

    public RecipeName _itemName;

    [Space(10)]
    [Header("Below Reference For Cooking Process Item Only")]
    public MeshRenderer unCookMeat;
    public MeshRenderer CookMeat;

    [Header("Timer UI")]
    public Image timerIMG;
    private float CookedTimer = 4.0f;



    public bool isCooked { get; set; }
    public bool isOverCooked { get; set; }
    RecipeManager recipeManager;
    private void Start()
    {
        CheckRecipeManager();
        previousScale = this.transform.localScale;

        if (unCookMeat != null)
        {

        }
        else
        {
            isCooked = true;
        }
    }
    public void OnClicked(Vector3 scaleValue)
    {
        this.transform.DOScale(scaleValue, 0.1f).OnComplete(() =>
        {
            this.transform.DOScale(previousScale, 0.1f).OnComplete(() =>
            {
                if (HideAble && isCooked)
                {
                    // meshRenderer.material.color = recipeManager.freshMeat;
                    isCooked = false;
                    CookedTimer = 4.0f;
                    tween.Pause();
                    gameObject.SetActive(false);

                    unCookMeat.gameObject.SetActive(true);
                    CookMeat.gameObject.SetActive(false);
                }
            });
        });
    }
    public void OnClickedByActivator(Vector3 scaleValue)
    {
        this.transform.DOScale(scaleValue, 0.1f).OnComplete(() =>
        {
            this.transform.DOScale(previousScale, 0.1f);
        });
        CookTimer();
    }

    private void CookTimer()
    {
        CheckRecipeManager();
        timerIMG.color = Color.blue;
        //meshRenderer.material.DOColor(recipeManager.CookedMeat, CookedTimer);
        DOVirtual.Float(CookedTimer, 0.0f, CookedTimer, OnTimerUpdate).OnComplete(() =>
        {
            Debug.Log("Meat is Cooled");
            isCooked = true;
            unCookMeat.gameObject.SetActive(false);
            CookMeat.gameObject.SetActive(true);
            OverCookTimer();
        });
    }
    Tween tween;
    private void OverCookTimer()
    {
        timerIMG.color = Color.red;
        tween = DOVirtual.Float(CookedTimer, 0.0f, CookedTimer, OnTimerUpdate).OnComplete(() =>
        {
            Debug.Log("Meat is Over Cooled");
            isOverCooked = true;
        });
    }

    private void OnTimerUpdate(float value)
    {
        float normalValue = value / CookedTimer;
        timerIMG.fillAmount = normalValue;
    }

    private void CheckRecipeManager()
    {
        if (recipeManager == null)
        {
            recipeManager = FindObjectOfType<RecipeManager>();
        }
    }
}
