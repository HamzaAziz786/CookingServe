using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reception : MonoBehaviour
{
    public Transform GetTransform => this.transform;
    public bool haveOtherServant;

    private ReceptionManager Manager { get; set; }
    public void SetReceptionManager(ReceptionManager manager)
    {
        Manager = manager;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Servant>(out Servant servant))
        {
            Debug.Log("Servant Enter Reception");
            //servant.ServantTray.SetActive(true);
            //servant.rig.weight = 1;
            Manager.Add_InQueue(servant);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Servant>(out Servant servant))
        {
            Debug.Log("Servant leave Reception");
            //haveOtherServant = false;
           // Manager.Remove_InQueue(servant);
        }
    }
}
