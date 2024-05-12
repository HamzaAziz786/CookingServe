using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableManager : MonoBehaviour
{
    public event System.Action<Servant> OnServantFounded;
    public List<Table> tables;
    ServantManager servantManager;


    private void Start()
    {
        CheckServantManager();
        foreach (var table in tables)
        {
            table.SetUpTable(this);
        }
    }
    public Table CheckAvailableTable()
    {
        Table Selected_tables = tables.Find(x => x.tableReserved == false);
        return Selected_tables;
    }
    //public Table GetTableWhereCustomer()
    //{
    //    Table Selected_tables = tables.Find(x => x.customerOnTable == false);
    //    return Selected_tables;
    //}

    public Servant SendServantToReciption(Table table)
    {
        // We Activate the an Servant;
        Table SelectedTable = table;
        Servant servant = GetServant();
        if (servant != null)
        {
            servant.hasOrder = true;
            servant.MoveTowardGetOrder(SelectedTable);
        }
        else
        {
            StartCoroutine(SearchForServant((servant) =>
            {
                if (servant != null)
                {
                    Debug.Log("Table Servant Name: " + servant.gameObject.name);
                    servant.hasOrder = true;
                    servant.MoveTowardGetOrder(SelectedTable, 1.0f);
                    OnServantFounded?.Invoke(servant);
                }
                else
                {
                    Debug.LogError("No Servant Available");
                }
            }));
        }



        return servant;
    }

    private IEnumerator SearchForServant(System.Action<Servant> OnComplete)
    {
        Servant servant = null;
        while (servant == null)
        {
            servant = GetServant();
            if (servant != null)
            {
                OnComplete?.Invoke(servant);
            }
            yield return new WaitForEndOfFrame();
        }
    }

    private void CheckServantManager()
    {
        if (servantManager == null)
            servantManager = this.GetComponent<ServantManager>();
    }

    public Servant GetServant()
    {
        CheckServantManager();
        return servantManager.CheckAvailableServant();
    }
}
