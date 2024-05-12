using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    public Transform[] RestPoint;
    public List<Customer> customers;
    public List<ChildController> childControllers;
    public TableManager tableManager { get; set; }
    private IEnumerator Start()
    {
        tableManager = this.GetComponent<TableManager>();

        foreach (var customer in customers)
        {
            customer.SetUpCustomer(this);
        }
        SendNewCustomer();
        SendNewCustomer();
        yield return new WaitForSeconds(10.0f);
        SendNewCustomer();
        SendNewCustomer();
    }
    public Customer[] CheckAvailableCustomer()
    {
        Customer[] customers = new Customer[2];
        customers[0] = this.customers.Find(x => x.chairIndex == 0 && x.customerOnTable == false);
        customers[1] = this.customers.Find(x => x.chairIndex == 1 && x.customerOnTable == false);

        return customers;
    }
    public void SendNewCustomer()
    {
        Customer[] selectedCustomers = CheckAvailableCustomer();
        if (selectedCustomers[0] != null && selectedCustomers.Length == 2)
        {
            Table targetTable = GetTableAvailable();
            int tagTableIndex = targetTable.tableIndex;
            childControllers[tagTableIndex].ActivateChildMesh();
            childControllers[tagTableIndex].moveTowardTable();
         //if(targetTable != null)
         //{
            targetTable.tableReserved = true;
            foreach (var item in selectedCustomers)
            {
                item.SelectAgentMesh();
                item.SendCustomer(targetTable);
            }
        //}
        //else
        //{
        //    Debug.LogError("Sorry No Table Custmer Available Now");
        //}
        }
        else
        {
            //Debug.LogError("Sorry No Family Custmer Available Now");
            Invoke(nameof(SendNewCustomer), 0.5f);
        }
    }

    public Table GetTableAvailable()
    {
        return tableManager.CheckAvailableTable();
    }
}
