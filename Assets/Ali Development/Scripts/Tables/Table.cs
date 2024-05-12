using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class chair
{
    public string chairName = "chair0";
    public Transform customerPoint;
    public Transform sitPoint;
    public Transform standPoint;
}
public class Table : MonoBehaviour
{
    string[] AvailableItem = { "Burger", "BurgerAdvance" };

    public event System.Action<bool> onTimerComplete;
    public event System.Action OnCustomerOrderComplete;
    public event System.Action OnCustomerOrderFailed;
    public event System.Action<Transform[]> OnDeliverOrder;
    public event System.Action<Transform[]> OnNotDeliverOrder;

    public bool tableReserved;
    public bool OrderCompleted;
    public bool customerOnTable;
    public int tableIndex;

    [Space(5)]
    [Header("Timer & Requirement")]

    public float timer = 20;
    public GameObject UIRequirementPage;
    public Text[] txtRequirement;
    public Image UI_IMGTimer;
    [Space(5)]
    [Header("other References")]
    public Transform OrderRecevingPoint;
    public int customerCount;
    public List<Customer> customers;
    public chair[] chairs;

    public List<string> GetRequirements;
    private TableManager tableManager { get; set; }

    // Select Customer
    public Customer SelectedCustomer { get => selectedCustomer; set => selectedCustomer = value; }
    private Customer selectedCustomer;

    // Selected Servant
    public Servant SelectedServant { get => selectedServant; set => selectedServant = value; }
    private Servant selectedServant;
    public void SetUpTable(TableManager manager)
    {
        tableManager = manager;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Customer>(out Customer customer))
        {

            if (customer.movingOut) return;



            //if (selectedCustomer != null)
            //{
            //   onTimerComplete -= OnCustomerTimerComplete;
            //}
            // selectedCustomer = customer;

            //Calling Servant here



            customer.SitOnChair(chairs[customer.chairIndex].sitPoint, customer.chairIndex);
            customers.Add(customer);

            customerCount++;
            if (customerCount == 2)
            {
                foreach (var custom in customers)
                {
                    custom.SubscribeEvent();
                }

                selectedServant = tableManager.SendServantToReciption(this);

                if (selectedServant == null)
                {
                    tableManager.OnServantFounded += TableManager_OnServantFounded;
                }

                customerOnTable = true;
                for (int i = 0; i < customerCount; i++)
                {
                    string singleCustomerRequirment = AvailableItem[Random.Range(0, AvailableItem.Length)];
                    txtRequirement[i].text = singleCustomerRequirment;
                    GetRequirements.Add(singleCustomerRequirment);
                }

                UI_IMGTimer.gameObject.SetActive(true);
                UIRequirementPage.SetActive(true);
                UIRequirementPage.transform.LookAt(UIRequirementPage.transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);

                onTimerComplete += OnCustomerTimerComplete;
                StartTimer();
            }
        }
        if (other.TryGetComponent<Servant>(out Servant servant))
        {
            //if (selectedServant != null)
            //{
            //    selectedServant.onTimerComplete -= OnCustomerTimerComplete;
            //}

            selectedServant = servant;
            if (servant.takeOrder)
            {
                tween.Pause();
                DeactivateUI();

                OnDeliverOrder?.Invoke(servant.servantManager.waitPoint.ToArray());
                OnCustomerOrderComplete?.Invoke();

                // Reset Event
                ClearTableData();
            }

            //if (servant.hasTimer)
            //{
            //    servant.onTimerComplete += OnCustomerTimerComplete;
            //}
        }
    }

    private void ClearTableData()
    {
        foreach (var custom in customers)
        {
            custom.UnSubcribeEvent();
        }
        customerCount = 0;
        customers.Clear();
        GetRequirements.Clear();

        if (selectedServant != null)
            selectedServant.Requirement.Clear();

        tableReserved = false;

        onTimerComplete -= OnCustomerTimerComplete;
    }
    private void TableManager_OnServantFounded(Servant obj)
    {
        selectedServant = obj;
        Debug.Log("Servant Founded Late");
        tableManager.OnServantFounded -= TableManager_OnServantFounded;
    }

    private IEnumerator SearchForServant(System.Action<Servant> OnComplete)
    {
        Servant servant = null;
        while (servant == null)
        {
            servant = tableManager.GetServant();
            if (servant != null)
            {
                OnComplete?.Invoke(servant);
            }
            yield return new WaitForEndOfFrame();
        }
    }

    private void OnCustomerTimerComplete(bool hastakeOrder)
    {

        if (hastakeOrder)
        {

        }
        else
        {
            if (selectedServant != null)
            {
                OnNotDeliverOrder.Invoke(selectedServant.servantManager.waitPoint.ToArray());
                //  selectedServant.SetRequirement(new string[] { });
            }

            OnCustomerOrderFailed?.Invoke();
            Invoke(nameof(ClearTableData), 1.0f);
        }
    }

    public void CustomerPointInteractivity(bool _value, int chairIndex)
    {
        chairs[chairIndex].customerPoint.gameObject.SetActive(_value);
    }

    ///////////// Timer /////////////////
    Tween tween;
    private void StartTimer()
    {
        tween = DOVirtual.Float(20, 0, 20, (floatValue) =>
        {
            timer = floatValue;
            float normalTimer = timer / 20;
            UI_IMGTimer.fillAmount = normalTimer;

        }).OnStart(() =>
        {
            UIRequirementPage.SetActive(true);
            UI_IMGTimer.gameObject.SetActive(true);

        }).OnComplete(() =>
        {
            DeactivateUI();
            onTimerComplete?.Invoke(SelectedServant.takeOrder);
        });
    }

    public void DeactivateUI()
    {
        UI_IMGTimer.gameObject.SetActive(false);
        UIRequirementPage.SetActive(false);
    }
}
