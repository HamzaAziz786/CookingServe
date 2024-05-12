using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class RecipeManager : MonoBehaviour
{
    public List<RecipeMixer> recipeMixers;
    public List<GameObject> ReadyToServerItems;
    public LayerMask layerMask;

    //[Header("Meat Color")]
    //public Color freshMeat;
    //public Color CookedMeat;
    //public Color OverCookedMeat;

    private Color color = Color.red;
    private ReceptionManager receptionManager;
    private float overClickHandler;

    public ReceptionManager ReceptionManager { get => receptionManager; set => receptionManager = value; }

    private void Start()
    {
        SetUpRecipeManager();
        receptionManager = this.GetComponent<ReceptionManager>();
    }
    private void Update()
    {
        Find_InteractableObject();
    }

    private void SetUpRecipeManager()
    {
        foreach (var item in recipeMixers)
        {
            item.SetReceipeManager(this);
        }
    }
    private void Find_InteractableObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float maxDistance = 10;
        Debug.DrawRay(ray.origin, ray.direction * maxDistance, color);
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, layerMask))
        {
            if (hit.collider.TryGetComponent<RecipeObject>(out RecipeObject recipeObject))
            {
                color = Color.green;
                if (Input.GetMouseButtonDown(0) && overClickHandler == 0)
                {
                    overClickHandler = 1;

                    ResetOverClick();

                    recipeObject.OnClicked(Vector3.one * 0.35f);

                    if (recipeObject.isCooked == false) return;

                    RecipeMixer recipeMixer = GetSelectedRecipe(recipeObject);

                    if (recipeMixer != null && recipeObject != null)
                    {
                        recipeMixer.UpdateRecipe(recipeObject._itemName);
                    }
                }
            }
            else if (hit.collider.TryGetComponent<RecipeObjectActivator>(out RecipeObjectActivator recipeObjectActivator))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    recipeObjectActivator.OnClicked(Vector3.one * 0.35f);
                }
            }
            else
            {
                color = Color.red;
            }
            /// Check Ready Recipe
            if (hit.collider.TryGetComponent<RecipeItem>(out RecipeItem recipeItem))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (recipeItem.recipeMixer.recipeCompleted)
                    {
                        recipeItem.recipeMixer.GenerateInstnceOfCurrentRecipe();
                    }

                    recipeItem.ClickOnInstance(Vector3.one * 1.2f);
                    recipeItem.CheckDoubleClick();
                }
            }
            if (hit.collider.TryGetComponent<RecipeInstance>(out RecipeInstance recipeInstance))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    recipeInstance.OnClicked(Vector3.one * 1.2f);
                    //recipeInstance.SendToServent();
                    recipeInstance.CheckDoubleClick();
                }
            }
        }
        else
        {
            color = Color.red;
        }
    }

    private void ResetOverClick()
    {
        Invoke(nameof(OverResetter), 0.1f);
    }

    private void OverResetter()
    {
        overClickHandler = 0;
    }

    RecipeMixer GetSelectedRecipe(RecipeObject recipeObject)
    {
        RecipeMixer SelectedMixer = null;
        foreach (var item in recipeMixers)
        {
            bool IsCompleted = false;

            IsCompleted = item.checkRecipeComplete(recipeObject);
            if (IsCompleted == false)
            {
                SelectedMixer = item;
                break;
            }
        }
        return SelectedMixer;
        //return recipeMixers[0];
    }

    public void AddReadyItem(GameObject CorrectRecipeOnly)
    {
        if (ReadyToServerItems.Contains(CorrectRecipeOnly) == false)
        {
            ReadyToServerItems.Add(CorrectRecipeOnly);
        }
    }
    public void RemoveRecipe(GameObject Recipe)
    {
        if (ReadyToServerItems.Contains(Recipe) == true)
        {
            ReadyToServerItems.Remove(Recipe);
        }
    }
    public void SendReceipeToServant(string receipeName, Servant servant)
    {
        Servant selectedServant = servant;
        if (selectedServant != null)
        {
            Transform trayPoint = selectedServant.ServantTray.transform.Find("ReceipePoint");
            Transform trayPoint1 = selectedServant.ServantTray.transform.Find("ReceipePoint (1)");
            ServRequirement serv = selectedServant.Requirement.Find(x => x.Requirement == receipeName);
            if (serv == null) return;

            string requirement = serv.Requirement;

            GameObject SelectedRecipe = ReadyToServerItems.Find(x => x.name == requirement);
            SelectedRecipe.transform.parent = null;
            SelectedRecipe.GetComponent<RecipeInstance>().StopScaling();

            Transform targetPoint = selectedServant.RequirementCompleteCount == 1 ? trayPoint : trayPoint1;

            SelectedRecipe.transform.DOJump(targetPoint.position, 0.5f, 1, 0.25f).OnComplete(() =>
            {
                SelectedRecipe.GetComponent<RecipeInstance>().recipeMixer.ActivateRecipeMixer();
                SelectedRecipe.transform.SetParent(targetPoint);
                SelectedRecipe.GetComponent<RecipeInstance>().ResetScale();
                RemoveRecipe(SelectedRecipe);

                //--------------------------------- check requirement completed -------------///
                bool isrequirementcompleted = false;
                for (int i = 0; i < selectedServant.Requirement.Count; i++)
                {
                    if (selectedServant.Requirement[i].IsTaken)
                    {
                        isrequirementcompleted = true;
                    }
                    else
                    {
                        isrequirementcompleted = false;
                        break;
                    }
                }

                if (isrequirementcompleted == false) return;

                selectedServant.takeOrder = true;
                receptionManager.Remove_InQueue(selectedServant);

                //--------------------------------- Move Back To Order Table ----------------///
                selectedServant.MoveBackTowardTable();
            });
        }
    }
}
