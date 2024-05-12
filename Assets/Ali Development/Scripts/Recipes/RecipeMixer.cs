using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using DG.Tweening;
public class RecipeMixer : MonoBehaviour
{
    public event System.Action OnRecipeComplete;
    public bool recipeProcess { get; set; }
    public bool recipeCompleted { get; set; }

    private RecipeManager recipeManager;
    [Space(12)]
    [Header("Recipe Redesign Logic")]

    public bool isRecipeFaulted;
    public string recipeStatus;
    public List<GameObject> RecipeObjectInstance;
    public List<GameObject> RecipeStatus;
    public List<RecipeContainer> recipeContainers;
    [Header("Recipe Instance Logic")]
    public GameObject instanceSlot;

    public int currentCout { get; set; }


    public void UpdateRecipe(RecipeName _Name)
    {
        StartCoroutine(RecipeToaster(_Name));
    }
    IEnumerator RecipeToaster(RecipeName _Name)
    {
        currentCout++;
        recipeProcess = true;
        GameObject objectInstance = null;
        foreach (var item in RecipeObjectInstance)
        {
            RecipeItem itemRecipe = item.GetComponent<RecipeItem>();
            if (itemRecipe._itemName == _Name)
            {
                objectInstance = item;
                break;
            }
        }
        if (RecipeStatus.Count != 0)
        {
            GameObject obj = RecipeStatus[0];
            DestroyImmediate(obj);
            RecipeStatus.Remove(obj);
        }

        GameObject instance = Instantiate(objectInstance, this.instanceSlot.transform);
        instance.GetComponent<RecipeItem>().recipeMixer = this;
        if (instance.GetComponent<RecipeItem>()._itemName == RecipeName.patty)
        {
        }
        string instanceName = instance.name;
        string removeValues = instanceName.Replace("(Clone)", "");
        instance.name = removeValues;

        instance.transform.localPosition = new Vector3(0, 0, 0);
        instance.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            //Rigidbody rigidbody = instance.AddComponent<Rigidbody>();
            //rigidbody.drag = 0.0f;
        });


        RecipeStatus.Add(instance);
        if (instance.GetComponent<RecipeItem>().checkForRecipeComplete)
        {
            CheckRecipeMatching();
        }

        yield return new WaitForSeconds(0.1f);
    }
    private void CheckRecipeMatching()
    {
        foreach (var recipeContainer in recipeContainers)
        {
            if (recipeContainer.CheckSequence(RecipeStatus))
            {
                recipeCompleted = true;

                recipeStatus = recipeContainer.RecipeName; //"recipe matched";
                isRecipeFaulted = false;
                break;
            }
            else
            {
                isRecipeFaulted = true;
                recipeStatus = "not recipe matched";
            }
        }


    }

    private void ClearRecipeData()
    {
        foreach (var item in RecipeStatus)
        {
            Destroy(item);
        }
        RecipeStatus.Clear();
        this.gameObject.SetActive(false);
    }

    public void GenerateInstnceOfCurrentRecipe()
    {
        foreach (var recipeContainer in recipeContainers)
        {
            if (recipeContainer.CheckReceipeByName(recipeStatus))
            {
                Servant selectedServant = recipeManager.ReceptionManager.GetWaitingListServent(recipeContainer.RecipeName);
                if (selectedServant != null)
                {
                  
                    GameObject RecpInst = GenerateReceipeInstance(recipeContainer.RecipeName);
                    RecpInst.GetComponent<RecipeInstance>().Init();
                    RecpInst.GetComponent<RecipeInstance>().RecipeName = recipeContainer.RecipeName;
                    RecpInst.GetComponent<RecipeInstance>().isCorrect = true;
                    recipeManager.AddReadyItem(RecpInst);
                    //---------------------------------------
                    RecpInst.GetComponent<RecipeInstance>().SendToServent(selectedServant);
                    ClearRecipeData();
                }
            }
        }

    }
    private GameObject GenerateReceipeInstance(string instanceName)
    {
        GameObject RecpInst = Instantiate(this.instanceSlot, this.instanceSlot.transform.position, this.instanceSlot.transform.rotation);
        RecpInst.name = instanceName;
        for (int i = 0; i < RecpInst.transform.childCount; i++)
        {
            Transform child = RecpInst.transform.GetChild(i);
            if (child != null)
            {
                Destroy(child.GetComponent<RecipeItem>());
                Destroy(child.GetComponent<Collider>());
                Destroy(child.GetComponent<Rigidbody>());

                child.localScale = Vector3.one;
            }
        }
        RecpInst.GetComponent<Collider>().enabled = true;
        RecpInst.GetComponent<RecipeInstance>().recipeManager = recipeManager;
        RecpInst.GetComponent<RecipeInstance>().recipeMixer = this;
        return RecpInst;
    }
    public bool checkRecipeComplete(RecipeObject recipeObject)
    {
        bool Checking = true;
        //if (recipeCompleted)
        //{
        //    Debug.Log("Recipe Completed");
        //    return Checking;
        //}
        if (RecipeStatus.Count == 0)
        {
            if (recipeObject._itemName == RecipeName.Ban)
            {
                Checking = false;
            }
        }
        else
        {
            int lastIndex = RecipeStatus.Count - 1;
            GameObject lastRecipeItem = RecipeStatus[lastIndex];
            RecipeItem recipeItem = lastRecipeItem.GetComponent<RecipeItem>();
            foreach (var item in recipeItem.NextRequirement)
            {
                if (item == recipeObject._itemName)
                {
                    Checking = false;
                    break;
                }
            }
        }

        return Checking;
    }
    public void SetReceipeManager(RecipeManager recipeManager)
    {
        this.recipeManager = recipeManager;
    }


    private void OnEnable()
    {
        //  ResetObjectStatus();
        recipeCompleted = false;
        recipeProcess = false;
        isRecipeFaulted = false;
        currentCout = 0;
    }
    private void OnDisable()
    {

    }
    public void ActivateRecipeMixer()
    {
        this.gameObject.SetActive(true);
    }

    public void AddRecipeItem(GameObject instance)
    {
        if (RecipeStatus.Contains(instance) == false)
        {
            RecipeStatus.Add(instance);
        }
    }
    public void RemoveRecipeItem(GameObject instance)
    {
        if (RecipeStatus.Contains(instance) == true)
        {
            RecipeStatus.Remove(instance);
        }
    }

    //private void Update()
    //{
    //    if (UpdateBurger)
    //    {
    //        UpdateRecipe(recipeIndex);
    //        if (recipeIndex < recipeItems.Count)
    //        {
    //            recipeIndex++;
    //        }

    //        UpdateBurger = false;
    //    }
    //}
    //private void ResetObjectStatus()
    //{
    //    recipeIndex = 0;
    //    recipeCompleted = false;
    //    recipeItems.Clear();
    //    recipeItems.AddRange(this.GetComponentsInChildren<RecipeItem>());
    //    List<RecipeItem> recipes = recipeItems.OrderBy(x => x.itemIndex).ToList();
    //    recipeItems = recipes;
    //    // ------------------ Reset All Child Object Status -----------------------///
    //    // ResetAllCompletedPoint();
    //}



    //private void CheckRecipeComplete()
    //{
    //    bool CheckCompleted = false;
    //    for (int i = 0; i < recipeItems.Count; i++)
    //    {
    //        if (recipeItems[i] != null)
    //        {
    //            CheckCompleted = recipeItems[i].isCompleted;
    //            if (CheckCompleted == false)
    //            {
    //                break;
    //            }
    //        }
    //    }
    //    if (CheckCompleted == true)
    //    {
    //        OnRecipeComplete?.Invoke();
    //        recipeCompleted = true;
    //        Invoke(nameof(GenerateInstanceOfRecipe), 1.2f);
    //    }
    //}

    //private void GenerateInstanceOfRecipe()
    //{
    //    GameObject recipeObj = Instantiate(recipeInstance, this.transform.position, this.transform.rotation);
    //    RecipeReadyItem recipeReady = recipeObj.GetComponent<RecipeReadyItem>();
    //    recipeManager.Add_InReadyItem(recipeReady);
    //    this.gameObject.SetActive(false);
    //}

    //private void ResetAllCompletedPoint()
    //{
    //    for (int i = 0; i < recipeItems.Count; i++)
    //    {
    //        if (recipeItems[i] != null)
    //        {
    //            recipeItems[i].ResetObjectStatus();
    //        }
    //    }
    //}


}
