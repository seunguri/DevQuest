using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    [Header("Preset Fields")] 
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject splashFx;
    
    [Header("Settings")]
    [SerializeField] private float attackRange;
    [SerializeField] private float chasingRange;
    [SerializeField] private GameObject target; //TODO game manager

    Stat _stat;
    
    public enum State 
    {
        None,
        Idle,
        Attack,
        Chasing,
        Die,
    }
    
    [Header("Debug")]
    public State state = State.None;
    public State nextState = State.None;

    private bool attackDone;

    private void Start()
    { 
        state = State.None;
        nextState = State.Idle;

        _stat = gameObject.GetComponent<Stat>();
    }

    private void Update()
    {
        //1. 스테이트 전환 상황 판단
        if (nextState == State.None) 
        {
            switch (state) 
            {
                case State.Idle:
                    //1 << 6인 이유는 Player의 Layer가 6이기 때문
                    if (Physics.CheckSphere(transform.position, attackRange, 1 << 6, QueryTriggerInteraction.Ignore))
                    {
                        nextState = State.Attack;
                    }
                    else if (Physics.CheckSphere(transform.position, chasingRange, 1 << 6, QueryTriggerInteraction.Ignore))
                    {
                        nextState = State.Chasing;
                    }
                    break;
                case State.Attack:
                    if (attackDone)
                    {
                        nextState = State.Idle;
                        attackDone = false;
                    }
                    break;
                case State.Chasing:
                    {
                        if (Physics.CheckSphere(transform.position, attackRange, 1 << 6, QueryTriggerInteraction.Ignore))
                        {
                            nextState = State.Attack;
                        }
                        else if (Physics.CheckSphere(transform.position, chasingRange, 1 << 6, QueryTriggerInteraction.Ignore))
                        {
                            nextState = State.Chasing;
                        }
                        else
                        {
                            nextState = State.Idle;
                            animator.ResetTrigger("walk");
                            animator.SetTrigger("idle");
                        }
                    }
                    break;
            }
        }
        
        //2. 스테이트 초기화
        if (nextState != State.None) 
        {
            state = nextState;
            nextState = State.None;
            switch (state) 
            {
                case State.Idle:
                    break;
                case State.Attack:
                    Attack();
                    break;
                case State.Chasing:
                    Chasing();
                    break;
                case State.Die:
                    Die();
                    break;
            }
        }
        
        //3. 글로벌 & 스테이트 업데이트
        //insert code here...
    }

    private void Attack() //현재 공격은 애니메이션만 작동합니다.
    {
        animator.SetTrigger("attack");
    }

    public void InstantiateFx() //Unity Animation Event 에서 실행됩니다.
    {
        Instantiate(splashFx, transform.position, Quaternion.identity);
    }
    
    public void WhenAnimationDone() //Unity Animation Event 에서 실행됩니다.
    {
        attackDone = true;
    }

    private void Chasing()
    {
        Vector3 dir = target.transform.position - transform.position;

        NavMeshAgent nma = gameObject.GetComponent<NavMeshAgent>();
        nma.SetDestination(target.transform.position);
        nma.speed = _stat.MoveSpeed;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);

        animator.SetTrigger("walk");
    }

    private void Die()
    {
        animator.CrossFade("death", 0.1f);
    }

    public void OnDieEvent()
    {
        Managers.Destroy(gameObject);
    }


    private void OnDrawGizmosSelected()
    {
        //Gizmos를 사용하여 공격 범위를 Scene View에서 확인할 수 있게 합니다. (인게임에서는 볼 수 없습니다.)
        //해당 함수는 없어도 기능 상의 문제는 없지만, 기능 체크 및 디버깅을 용이하게 합니다.
        Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
        Gizmos.DrawSphere(transform.position, attackRange);
    }
}
