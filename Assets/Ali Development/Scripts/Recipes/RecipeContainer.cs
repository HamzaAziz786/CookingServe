using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Recipe/Recipe_Data", fileName = "Recipe_Instance", order = 0)]
public class RecipeContainer : ScriptableObject
{
    public string RecipeName;
    public List<GameObject> objects;
    public bool CheckSequence(List<GameObject> InComingList)
    {
        bool checking = true;
        for (int i = 0; i < objects.Count; i++)
        {
            GameObject inComingListObject = InComingList[i];
            if (inComingListObject == null)
            {
                checking = false;
                break;
            }
            int requiredIndex = this.objects.FindIndex(x => x == this.objects[i]);
            int inComingIndex = InComingList.FindIndex(x => x == InComingList[requiredIndex]);
            GameObject requiredObject = objects[requiredIndex];
            GameObject selectedObject = InComingList[inComingIndex];

            Debug.Log($"required object {requiredObject.name} {requiredIndex} -- selected object {selectedObject.name} {inComingIndex}");

            bool _value = requiredObject.name.Contains(selectedObject.name);
            Debug.Log("Is match " + _value);
            //---------------------------------------------------
            if (_value)
            {

            }
            else
            {
                checking = false;
                break;
            }
        }
        return checking;
    }

    public bool CheckReceipeByName(string recipeName)
    {
        bool checking = false;
        if (RecipeName == recipeName)
        {
            checking = true;
        }
        return checking;
    }
}
