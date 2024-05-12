using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeInstance : MonoBehaviour
{
    public string RecipeName;
    public bool isFaulted;
    public bool isCorrect;
    [HideInInspector] public RecipeManager recipeManager;
    [HideInInspector] public RecipeMixer recipeMixer;
    private int count;

    Vector3 previousScale;
    Tween OnScale, OnNormal;
    Tween RecursiveScaleX, RecursiveScaleY;
    private bool Scaling;
    private void Start()
    {
        previousScale = this.transform.localScale;
    }

    public void Init()
    {
        Scaling = true;
        RecursiveScaling();
    }
    public void RecursiveScaling()
    {
        this.transform.DOScaleX(1.05f, 0.5f).OnComplete(() =>
        {
            this.transform.DOScaleX(1.05f, 0.5f);
            this.transform.DOScaleY(1.05f, 0.5f).OnComplete(() =>
            {
                this.transform.transform.DOScaleY(1.0f, 0.5f).OnComplete(() =>
                {
                    if (Scaling == true)
                        RecursiveScaling();
                });
            });
        });
    }

    public void StopScaling()
    {
        Scaling = false;
    }
    public void ResetScale()
    {
        this.transform.DOScale(Vector3.one, 0.4f);
    }

    public void OnClicked(Vector3 scaleValue)
    {
        OnScale = this.transform.DOScale(scaleValue, 0.1f).SetLoops(1, LoopType.Yoyo).OnComplete(() =>
         {
             OnNormal = this.transform.DOScale(previousScale, 0.1f).OnComplete(() =>
             {

             });
         });
    }

    public void CheckDoubleClick()
    {
        count++;
        Invoke(nameof(ResetCounter), 0.3f);
        if (count >= 2)
        {
            OnScale.Pause();
            OnNormal.Pause();
            CancelInvoke(nameof(ResetCounter));
            OnDestroyRecipe();
        }
    }

    public void OnDestroyRecipe()
    {
        this.GetComponent<Collider>().enabled = false;
        recipeManager.RemoveRecipe(this.gameObject);
        this.transform.DOScale(Vector3.zero, 0.5f).OnComplete(() =>
        {

            Destroy(this.gameObject);
        });
    }
    public void SendToServent(Servant servant)
    {
        recipeManager.SendReceipeToServant(RecipeName,servant);
    }
    private void ResetCounter()
    {
        count = 0;
    }


}
