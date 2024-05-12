using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeObjectActivator : MonoBehaviour, IRecipeObject
{
    Vector3 previousScale = Vector3.one;
    public List<GameObject> targetObject;
    private void Start()
    {
        previousScale = this.transform.localScale;
    }
    public void OnClicked(Vector3 scaleValue)
    {
        this.transform.DOScale(scaleValue, 0.1f).OnComplete(() =>
        {
            GameObject SelectedTarget = targetObject.Find(x => x.gameObject.activeInHierarchy == false);
            if (SelectedTarget)
            {
                SelectedTarget.SetActive(true);
                SelectedTarget.GetComponent<RecipeObject>().OnClickedByActivator(scaleValue);
                this.transform.DOScale(previousScale, 0.1f);
            }
        });
    }
}
