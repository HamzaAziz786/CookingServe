using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReceptionManager : MonoBehaviour
{
    public List<Reception> receptions;

    public List<Servant> waitingList;



    private void Start()
    {
        foreach (var item in receptions)
        {
            item.SetReceptionManager(this);
        }
    }

    public Reception CheckAvailableReception()
    {
        Reception reception = receptions.Find(x => x.haveOtherServant == false);
        return reception;
    }
    public void Add_InQueue(Servant servant)
    {
        if (waitingList.Contains(servant) == false)
        {
            waitingList.Add(servant);
        }
    }
    public void Remove_InQueue(Servant servant)
    {
        if (waitingList.Contains(servant) == true)
        {
            waitingList.Remove(servant);
        }
    }

    public Servant GetWaitingListServent(string orderRequirement)
    {
        Servant selectedServant = null;
        bool breakOuterLoop = false;
        foreach (var servant in waitingList)
        {
            if (servant.takeOrder == false)
            {
                for (int i = 0; i < servant.Requirement.Count; i++)
                {
                    if (servant.Requirement[i].Requirement.Contains(orderRequirement))
                    {
                        int requirementIndex = servant.Requirement.FindIndex(x => x.Requirement == orderRequirement && x.IsTaken == false);
                        Debug.Log("requiredIndex " + requirementIndex);
                        if (requirementIndex == -1)
                        {

                            break;
                        }
                        if (servant.Requirement[requirementIndex].IsTaken == true)
                        {
                            continue;
                        }

                        servant.Requirement[requirementIndex].IsTaken = true;
                        selectedServant = servant;
                        breakOuterLoop = true;
                        break;
                    }
                }
            }
            if (breakOuterLoop)
            {
                break;
            }
        }
        // Servant servant = waitingList.Find(x => x.Requirement.ToArray() == orderRequirement && x.takeOrder == false);
        return selectedServant;
    }
}
