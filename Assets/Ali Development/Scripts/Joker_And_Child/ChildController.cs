using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
public class ChildController : MonoBehaviour
{
    public GameObject[] meshes;
    public string childName;
    [Header("in and out system")]
    public Transform jokerPoint;
    public Transform outPoint;
    public Table table;

    private Animator animator;
    private NavMeshAgent agent;
    private void OnEnable()
    {
        table.OnCustomerOrderComplete += Table_OnCustomerOrderComplete;
        table.OnCustomerOrderFailed += Table_OnCustomerOrderFailed;

    }
    private void OnDisable()
    {
        table.OnCustomerOrderComplete -= Table_OnCustomerOrderComplete;
        table.OnCustomerOrderFailed -= Table_OnCustomerOrderFailed;
    }

    private void Table_OnCustomerOrderFailed()
    {
        Invoke(nameof(moveToEndPoint), 10.0f);
    }

    private void Table_OnCustomerOrderComplete()
    {
        Invoke(nameof(moveToEndPoint), 6.0f);
    }
    void moveToJoker()
    {
        WalkAnimation();
        agent.SetDestination(jokerPoint.position);
        StartCoroutine(CalculateDistance(jokerPoint.position, () =>
        {
            IdleAnimation();
            Invoke(nameof(BabyHappyness), 0.3f);
        }));
    }
    public void moveTowardTable()
    {
        if (IsInvoking(nameof(WalkAnimation)) == false)
            InvokeRepeating(nameof(WalkAnimation), 0.0f, 0.4f);

        int randomChairPoint = Random.Range(0, table.chairs.Length);
        Vector3 targetPosition = table.chairs[randomChairPoint].standPoint.position;
        agent.SetDestination(targetPosition);
        StartCoroutine(CalculateDistance(targetPosition, () =>
        {
            IdleAnimation();
            Invoke(nameof(moveToJoker), 2.0f);
        }, 1.2f));
    }
    void moveToEndPoint()
    {
        WalkAnimation();

        agent.SetDestination(outPoint.position);
        StartCoroutine(CalculateDistance(outPoint.position, () =>
        {
            IdleAnimation();
            ActivateChildMesh();
            Invoke(nameof(moveTowardTable), 3.0f);
        }));
    }

    // Start is called before the first frame update
    void Start()
    {
        float timer = UnityEngine.Random.Range(2.0f, 16.0f);
        ActivateChildMesh();
    }

    public void ActivateChildMesh()
    {
        foreach (var jokerMesh in meshes)
        {
            jokerMesh.SetActive(false);
        }
        int index = UnityEngine.Random.Range(0, meshes.Length);

        meshes[index].SetActive(true);
        animator = meshes[index].GetComponent<Animator>();
        agent = this.GetComponent<NavMeshAgent>();
        ActivateAnimation();
    }
    void ActivateAnimation()
    {

    }

    IEnumerator CalculateDistance(Vector3 targetPoint, System.Action onReached)
    {
        float distance = 10000;
        int once = 0;
        while (distance > 0.4f)
        {
            distance = Vector3.Distance(targetPoint, this.transform.position);

            if (distance < 1.2f && once == 0)
            {
                once = 1;
                onReached?.Invoke();
            }
            yield return new WaitForEndOfFrame();
        }
        if (once == 0)
        {
            onReached?.Invoke();
        }
    }
    IEnumerator CalculateDistance(Vector3 targetPoint, System.Action onReached, float reachDistance = 1.2f)
    {

        float distance = 10000;
        int once = 0;
        while (distance > reachDistance - 0.1f)
        {
            distance = Vector3.Distance(targetPoint, this.transform.position);

            if (distance < reachDistance && once == 0)
            {
                once = 1;
                onReached?.Invoke();
            }
            yield return new WaitForEndOfFrame();
        }
        if (once == 0)
        {
            onReached?.Invoke();
        }
    }


    void IdleAnimation()
    {
        animator.SetFloat("speed", 0);
    }
    void WalkAnimation()
    {
        if (animator.GetFloat("speed") == 0)
        {
            animator.SetBool("Playing", false);
            animator.SetFloat("speed", 1);
            //Debug.Log($"Child Name is {animator.gameObject.name}: " + childName);
        }
        else
        {
            CancelInvoke(nameof(WalkAnimation));
        }
    }
    void BabyHappyness()
    {
        animator.SetTrigger("triggerPlay");
        animator.SetBool("Playing", true);
        this.transform.DORotateQuaternion(outPoint.rotation, 0.2f);
    }

}
