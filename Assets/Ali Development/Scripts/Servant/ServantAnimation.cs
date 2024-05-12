using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ServantAnimation : MonoBehaviour
{
    private float agentSpeed;
    private Animator animator;
    private NavMeshAgent agent;
    private int Speed = Animator.StringToHash("Speed");

    // Start is called before the first frame update
    void Start()
    {
        animator = this.GetComponent<Animator>();
        agent = this.GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        agentSpeed = agent.velocity.magnitude / agent.speed;
        animator.SetFloat(Speed, agentSpeed);
    }
}
