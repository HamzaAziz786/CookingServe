using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using UnityEngine.UI;

public class Customer : MonoBehaviour
{
    public string DebugCustomerName;
    public int chairIndex;
    public bool customerOnTable;
    public bool movingOut;
    public List<Animator> customers;


    [Space(5)]
    [Header("Debug")]
    public bool debug;
    public float remaingDistance;


    [SerializeField] Table customerTargetTable;
    [SerializeField] Transform restPoint;
    NavMeshAgent agent;
    Animator animator;
    Transform PreviousParent;
    float agentSpeed;
    int Speed = Animator.StringToHash("Speed");
    private CustomerManager customerManager { get; set; }

    private void Start()
    {
        // characterCount = UnityEngine.Random.Range(0, customers.Count);
        SelectAgentMesh();
    }

    private void Update()
    {
        if (agent == null || animator == null) return;

        agentSpeed = (agent.velocity.magnitude / agent.speed) * 3.0f;
        animator.SetFloat(Speed, agentSpeed);
    }

    static int characterCount;

    public void SelectAgentMesh()
    {
        foreach (var item in customers)
        {
            item.gameObject.SetActive(false);
            item.gameObject.SetActive(false);
        }
        int selectedIndex = UnityEngine.Random.Range(0, customers.Count); //characterCount++;
        if (characterCount > customers.Count)
        {
            characterCount = 0;
            selectedIndex = 0;
        }
        customers[selectedIndex].gameObject.SetActive(true);

        //if (customers[selectedIndex].TryGetComponent<MaleTPPrefabMaker>(out MaleTPPrefabMaker maleTPPrefab))
        //{
        //    maleTPPrefab.Getready();
        //    maleTPPrefab.Randomize();
        //}
        //if (customers[selectedIndex].TryGetComponent<FemaleTPPrefabMaker>(out FemaleTPPrefabMaker femaleTPPrefab))
        //{
        //    femaleTPPrefab.Getready();
        //    femaleTPPrefab.Randomize();
        //}

        animator = customers[selectedIndex];
    }
    private void OnOrderComplete()
    {
        /// sit to stand
        /// then move to rest point
        StartCoroutine(MoveBackToRestPoint(true));
        //   customerTargetTable.OnCustomerOrderComplete -= OnOrderComplete;
    }
    private void OnOrderFailed()
    {
        StartCoroutine(MoveBackToRestPoint(false));
        //  customerTargetTable.OnCustomerOrderFailed -= OnOrderFailed;
    }

    public void SendCustomer(Table targetTable)
    {

        // customerTargetTable = targetTable;//GetTableAvailable();
        //-----------------
        if (customerTargetTable != null)
        {
            customerOnTable = true;

            Vector3 targetPosition = customerTargetTable.chairs[chairIndex].customerPoint.position;
            agent.SetDestination(targetPosition);
            Debug.Log("Customer Moving Toward Target " + DebugCustomerName);
        }
        else
        {
            Debug.LogError("Sorry No More Table Is Available");
        }
    }

    public void UnSubcribeEvent()
    {
        customerTargetTable.OnCustomerOrderComplete -= OnOrderComplete;
        customerTargetTable.OnCustomerOrderFailed -= OnOrderFailed;
    }
    public void SubscribeEvent()
    {
        customerTargetTable.OnCustomerOrderComplete += OnOrderComplete;
        customerTargetTable.OnCustomerOrderFailed += OnOrderFailed;
    }

    public void SetUpCustomer(CustomerManager manager)
    {
        customerManager = manager;
        checkNavMeshAgent();
        //-----------------

    }
    private void checkNavMeshAgent()
    {
        if (agent == null)
        {
            agent = this.GetComponent<NavMeshAgent>();
        }
    }



    public IEnumerator MoveBackToRestPoint(bool OrderComplete)
    {
        movingOut = true;
        this.transform.parent = null;
        this.transform.SetParent(PreviousParent);

        animator.SetBool("Sit", false);
        yield return new WaitForSeconds(2.0f);
        this.transform.DOMove(customerTargetTable.chairs[chairIndex].standPoint.position, 1.0f);

        // Check Order Complte Or not
        if (OrderComplete == false)
        {
            animator.SetTrigger("Angry");
            yield return new WaitForSeconds(4.8f);
            SAvingMAnager.savinginstance.DeductMoney(50);
        }
        else
        {
            SAvingMAnager.savinginstance.AddMoney(100);
            yield return new WaitForSeconds(1.2f);
        }

        agent.enabled = true;
        //-------------------
        int index = UnityEngine.Random.Range(0, customerManager.RestPoint.Length);
        Vector3 targetPosition = restPoint.position;//customerManager.RestPoint[index].position;
        agent.SetDestination(targetPosition);
        StartCoroutine(CalculateDistance(/*customerManager.RestPoint[index]*/restPoint, () =>
        {
            customerTargetTable.CustomerPointInteractivity(true, chairIndex);
            customerOnTable = false;
            movingOut = false;
            Invoke(nameof(SendNextFamily), 1.2f);
        }));

        yield return new WaitForSeconds(3.0f);
        // customerTargetTable.CustomerPointInteractivity(true);
    }
    void SendNextFamily()
    {
        if (chairIndex == 1)
            customerManager.SendNewCustomer();
    }
    public void SitOnChair(Transform sitPoint, int chairIndex)
    {

        agent.enabled = false;
        // change parent
        PreviousParent = this.transform.parent;
        this.transform.parent = null;
        this.transform.SetParent(sitPoint);

        // Apply sitting 
        animator.SetBool("Sit", true);
        customerTargetTable.CustomerPointInteractivity(false, chairIndex);
        this.transform.DOLocalMove(Vector3.zero, 0.50f).OnComplete(() =>
        {

        });
        this.transform.DOLocalRotateQuaternion(Quaternion.identity, 0.50f).OnComplete(() =>
        {

        });
    }



    private IEnumerator CalculateDistance(Transform targetPoint, Action OnMiddeWay, Action onReached)
    {

        float distance = 10000;
        float totalDistance = Vector3.Distance(targetPoint.position, this.transform.position);
        while (distance > 0.4f)
        {
            distance = Vector3.Distance(targetPoint.position, this.transform.position);

            float _Value = distance / totalDistance;

            if (_Value > 0.5f)
            {

            }
            if (debug)
                remaingDistance = distance;
            yield return new WaitForEndOfFrame();
        }
        onReached?.Invoke();
    }
    private IEnumerator CalculateDistance(Transform targetPoint, Action onReached)
    {

        float distance = 10000;
        int once = 0;
        while (distance > 0.4f)
        {
            distance = Vector3.Distance(targetPoint.position, this.transform.position);

            if (distance < 1.2f && once == 0)
            {
                once = 1;
                onReached?.Invoke();
            }
            if (debug)
                remaingDistance = distance;
            yield return new WaitForEndOfFrame();
        }

    }
}
