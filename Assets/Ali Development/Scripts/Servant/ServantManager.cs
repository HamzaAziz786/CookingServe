using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServantManager : MonoBehaviour
{
    public List<Servant> servants;
    public List<Transform> waitPoint;

    TableManager tableManager;
    ReceptionManager receptionManager;
    private void Start()
    {
        CheckTableManager();
        CheckReceptionManager();
        foreach (var servant in servants)
        {
            servant.SetServant(this);
        }
    }


    //---------------------------------------------------------------//
    //--------------------------- Servant -----------------------------//
    //---------------------------------------------------------------//

    public Servant CheckAvailableServant()
    {
        Servant servant = servants.Find(x => x.hasOrder == false);
        return servant;
    }


    //---------------------------------------------------------------//
    //--------------------------- Table -----------------------------//
    //---------------------------------------------------------------//

    private void CheckTableManager()
    {
        if (tableManager == null)
            tableManager = this.GetComponent<TableManager>();
    }

    private void CheckReceptionManager()
    {
        if (receptionManager == null)
            receptionManager = this.GetComponent<ReceptionManager>();
    }

    //---------------------------------------------------------------//
    //--------------------------- Reception -----------------------------//
    //---------------------------------------------------------------//

    public Reception CheckAvailableReception()
    {
        CheckReceptionManager();

        Reception reception = receptionManager.CheckAvailableReception();
        return reception;
    }

}
