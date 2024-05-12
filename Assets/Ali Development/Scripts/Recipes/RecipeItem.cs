using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class RecipeItem : MonoBehaviour
{
    public bool checkForRecipeComplete;
    public RecipeName _itemName;
    public List<RecipeName> NextRequirement;
    public int mininumListCount;
    [HideInInspector] public RecipeMixer recipeMixer;

    private int count;
    Vector3 previousScale = new Vector3();
    Tween OnNormal, OnScale;
    private void OnEnable()
    {
        ResetObjectStatus();
    }
    public void ResetObjectStatus()
    {
        previousScale = this.transform.localScale;

        this.transform.localScale = Vector3.zero;
    }
    public void ClickOnInstance(Vector3 scaleValue)
    {
        try
        {
            OnScale = this.transform.DOScale(scaleValue, 0.1f).SetLoops(1, LoopType.Yoyo).OnComplete(() =>
            {
                OnNormal = this.transform.DOScale(previousScale, 0.1f).OnComplete(() =>
                {

                });
            });
        }
        catch (Exception)
        {

        }

    }
    public void CheckDoubleClick()
    {
        //count++;
        //Invoke(nameof(ResetCounter), 0.3f);
        //if (count >= 2)
        //{
        //    OnScale.Pause();
        //    OnNormal.Pause();
        //    CancelInvoke(nameof(ResetCounter));
        //    this.GetComponent<Collider>().enabled = false;

        //    this.transform.DOScale(Vector3.zero, 0.5f).OnComplete(() =>
        //    {
        //        if (checkForRecipeComplete)
        //        {
        //            recipeMixer.isRecipeFaulted = false;
        //        }
        //        recipeMixer.currentCout -= 1;
        //        recipeMixer.RemoveRecipeItem(this.gameObject);
        //        Destroy(this.gameObject);
        //    });
        //}
    }
    private void ResetCounter()
    {
        count = 0;
    }
}
