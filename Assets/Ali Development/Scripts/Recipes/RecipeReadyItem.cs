using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class RecipeReadyItem : MonoBehaviour
{
    public string _name;

    Vector3 lastScale = Vector3.one;
    Tween tweenScale;
    private void Start()
    {
        lastScale = this.transform.localScale;
        LoopScaling();
    }


    void LoopScaling()
    {
        tweenScale = this.transform.DOScale(Vector3.one * 1.1f, 0.5f).SetLoops(-1, LoopType.Yoyo);
    }
    public void ClickOnInstance(Vector3 scale)
    {
        tweenScale.Pause();
        this.transform.DOScale(scale, 0.25f).OnComplete(() =>
        {
            this.transform.DOScale(lastScale, 0.25f);
        });
    }
}
