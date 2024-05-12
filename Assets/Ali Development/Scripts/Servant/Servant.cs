using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using UnityEngine.Animations.Rigging;
using UnityEngine.UI;


[System.Serializable]
public class ServRequirement
{
    public bool IsTaken;
    public string Requirement;
}

public class Servant : MonoBehaviour
{
    public event System.Action<ServRequirement[]> OnReachReciption;
    public event System.Action OnOrderDelivered;
    public bool hasOrder;
    public bool takeOrder;
    public int restPointIndex = 0;
    public Rig rig;
    public GameObject ServantTray;

    private NavMeshAgent agent;
    private Table TargetTable;
    private Reception Targetreception;


    public List<ServRequirement> Requirement;

    [Header("Use Timer")]
    public bool hasTimer;
    public float totalTime;
    public float remainingTime;
    public Image IMGTimer;
    public ServantManager servantManager { get; private set; }

    public void SetServant(ServantManager manager)
    {
        servantManager = manager;
    }
    public void MoveTowardGetOrder(Table _table, float waitTime = 0.2f)
    {
        StartCoroutine(Move_toward_table(_table, waitTime));
    }

    public void SetRequirement(string[] req)
    {
        Requirement.Clear();
        for (int i = 0; i < req.Length; i++)
        {
            Requirement.Add(new ServRequirement() { IsTaken = false, Requirement = req[i] });
        }
    }

    public int RequirementCompleteCount
    {
        get
        {
            int count = 0;
            for (int i = 0; i < Requirement.Count; i++)
            {
                if (Requirement[i].IsTaken)
                {
                    count++;
                }
            }
            return count;
        }
    }

    private void Start()
    {
        CheckAgent();
    }

    private IEnumerator Move_toward_table(Table _table, float waitTime = 0.2f)
    {
        yield return new WaitForSeconds(waitTime);
        if (TargetTable != null)
        {
            TargetTable.OnDeliverOrder -= TargetTable_OnOrderComplete;
            TargetTable.OnNotDeliverOrder -= TargetTable_OnOrderNotComplete;
        }
        // here we move toward the table
        TargetTable = _table;
        TargetTable.OnDeliverOrder += TargetTable_OnOrderComplete;
        TargetTable.OnNotDeliverOrder += TargetTable_OnOrderNotComplete;
        //---------------------------------------------------------
        CheckAgent();
        //------------------------
        Reception reception = servantManager.CheckAvailableReception();
        if (reception != null)
        {
            reception.haveOtherServant = true;
            Targetreception = reception;
            Transform nextPosition = Targetreception.GetTransform;
            SetDistination(nextPosition.position);
            StartCoroutine(CheckRemaingDistanceWithReciption(() =>
            {
                SetRequirement(TargetTable.GetRequirements.ToArray());
            }));
        }
        else
        {
            Debug.LogError("your Reciption is Not Empty");
        }
    }

    private IEnumerator CheckRemaingDistanceWithCustomer(bool takeRequirement)
    {
        float distance = 10000;

        while (distance > 0.2f)
        {
            distance = Vector3.Distance(TargetTable.OrderRecevingPoint.position, this.transform.position);
            yield return new WaitForEndOfFrame();
        }

        if (takeRequirement)
        {

        }
        else
        {
            Debug.Log("Move toward table to delivery order ");
        }

    }

    private void TargetTable_OnOrderComplete(Transform[] points)
    {
        // search for new order.
        // move toward last point.
        int restPointIndex = this.restPointIndex;//UnityEngine.Random.Range(0, points.Length);
        agent.enabled = true;
        SetDistination(points[restPointIndex].position);
        StartCoroutine(StartCalcuateDistance(points[restPointIndex],
        () =>
        {
            OnOrderDelivered?.Invoke();

            hasOrder = false;
            takeOrder = false;
            SetRequirement(new string[] { });
            TargetTable.tableReserved = false;
            Targetreception.haveOtherServant = false;
            rig.weight = 0.0f;
            ServantTray.SetActive(false);
            RecipeInstance[] recipeInstances = ServantTray.GetComponentsInChildren<RecipeInstance>();

            foreach (var recipeInstance in recipeInstances)
            {
                if (recipeInstance != null)
                {
                    recipeInstance.OnDestroyRecipe();
                }
            }
        },
        () =>
        {
            this.transform.DORotateQuaternion(points[restPointIndex].rotation, 0.3f);
        }));

        Debug.Log("move to rest Point");

    }

    private void TargetTable_OnOrderNotComplete(Transform[] points)
    {
        // move toward last point.
        int restPointIndex = this.restPointIndex; //UnityEngine.Random.Range(0, points.Length);

        if (agent.enabled == false) agent.enabled = true; // Restore Agent Interactivity

        SetDistination(points[restPointIndex].position);
        StartCoroutine(StartCalcuateDistance(points[restPointIndex],
        () =>
        {
            Debug.Log("Servant Name: " + gameObject.name);
            hasOrder = false;
            takeOrder = false;
            TargetTable.tableReserved = false;
            SetRequirement(new string[] { });
            Targetreception.haveOtherServant = false;
            RecipeInstance instance = ServantTray.GetComponentInChildren<RecipeInstance>();
            if (instance)
            {
                instance.OnDestroyRecipe();
            }
            Invoke(nameof(ResetServantTray), 0.0f);
        },
        () =>
        {
            this.transform.DORotateQuaternion(points[restPointIndex].rotation, 0.3f);
        }));
        Debug.Log("move to rest Point");
    }
    private void ResetServantTray()
    {
        ServantTray.SetActive(false);
        rig.weight = 0.0f;
    }
    public float distance;
    private IEnumerator CheckRemaingDistanceWithReciption(System.Action OnReached)
    {
        distance = 10000;

        while (distance > 0.23f)
        {
            distance = Vector3.Distance(Targetreception.GetTransform.position, this.transform.position);
            yield return new WaitForEndOfFrame();
        }
        //this.transform.LookAt(TargetTable.lookAtPoint);
        Debug.Log("Servant Reach To Reception");

        OnReached?.Invoke();
        OnReachReciption?.Invoke(Requirement.ToArray());

        agent.enabled = false;
        this.transform.DOMoveX(Targetreception.GetTransform.position.x, 0.3f);
        this.transform.DOMoveZ(Targetreception.GetTransform.position.z, 0.3f);
        this.transform.DOLookAt(Camera.main.transform.GetChild(0).position, 0.4f, AxisConstraint.X).OnComplete(() =>
        {
            ServantTray.SetActive(true);
            rig.weight = 1;
            OnReached?.Invoke();
        });
    }

    private IEnumerator StartCalcuateDistance(Transform targetPoint, Action onstartUp, Action onReached)
    {
        float distance = 10000;
        onstartUp?.Invoke();
        while (distance > 0.3f)
        {
            distance = Vector3.Distance(targetPoint.position, this.transform.position);

            yield return new WaitForEndOfFrame();
        }
        onReached?.Invoke();
    }
    private void SetDistination(Vector3 Position)
    {
        agent.SetDestination(Position);
    }

    private void CheckAgent()
    {
        if (agent == null)
        {
            agent = this.GetComponent<NavMeshAgent>();
        }
    }

    ///////////////--------------- Move Back To Order Table --------------------///

    public void MoveBackTowardTable()
    {
        if (TargetTable != null)
        {

            CheckAgent();
            agent.enabled = true;
            SetDistination(TargetTable.OrderRecevingPoint.position);
            StartCoroutine(CheckRemaingDistanceWithCustomer(false));
        }
        else
        {
            Debug.LogError("servant Target Table is null");
        }

    }

    ///-------------------------- Order Timer -----------------------------///

    Tween timerTween;
    //private void OrderTimer()
    //{
    //    IMGTimer.gameObject.SetActive(true);
    //    timerTween = DOVirtual.Float(0, totalTime, totalTime, OnTimerUpdate).OnComplete(() =>
    //    {
    //        if (takeOrder)
    //        {

    //        }
    //        else
    //        {
    //            onTimerComplete?.Invoke(takeOrder);
    //        }
    //    });
    //}

    //private void OnTimerUpdate(float value)
    //{
    //    remainingTime = value;
    //    float normalTimer = remainingTime / totalTime;
    //    IMGTimer.fillAmount = 1.0f - normalTimer;
    //    if (takeOrder)
    //    {
    //        timerTween.Pause();
    //    }
    //}
}

