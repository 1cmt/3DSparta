using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum AIState
{
    Idle,
    Wandering, //임의로 목표지점을 찍어 자동으로 움직일 수 있게
    Attacking
}

public class NPC : MonoBehaviour, IDamagable
{
    [Header("Stats")]
    public int health;
    public float walkSpeed;
    public float runSpeed;
    public ItemData[] dropOnDeath; //죽을 때 템 드랍

    [Header("AI")]
    private NavMeshAgent agent;
    public float detectDistance; //자동으로 위치를 찍고 목표 지점까지의 최소 거리
    private AIState aiState;

    [Header("Wandering")] //랜덤으로 목표지점을 찍어 자동으로 움직일 수 있게
    public float minWanderDistance;
    public float maxWanderDistance;
    public float minWanderWaitTime; //새로운 목표지점을 찍을 때 기다리는 최소시간
    public float maxWanderWaitTime; //새로운 목표지점을 찍을 때 기다리는 최대시간

    [Header("Combat")] //공격 관련
    public int damage;
    public float attackRate; //공격간의 텀
    private float lastAttackTime;
    public float attackDistance; //공격 가능 거리

    private float playerDistance; //플레이어와의 거리

    public float fieldOfView = 120f; //시야각, 몬스터를 기준으로 120도 범위안에 있을 때만 공격하도록, 360도로 하면 엉덩이쪽을 보는데 공격하는 참사가

    private Animator animator;
    private SkinnedMeshRenderer[] meshRenderers;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        meshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>(); //몬스터가 가지고 있는 각종 매쉬 정보들을 가져온다. 프리펩서 가져올거라 자식임, 공격받을 때 색깔을 바꿔주려고
    }
    // Start is called before the first frame update
    void Start()
    {
        SetState(AIState.Wandering);
    }

    // Update is called once per frame
    void Update()
    {
        playerDistance = Vector3.Distance(transform.position, CharacterManager.Instance.Player.transform.position);

        animator.SetBool("Moving", aiState != AIState.Idle); //Idle인지 아닌지가 기준, Idle이면 false가 나와 Moving 하지 않음

        switch (aiState)
        {
            case AIState.Idle:
            case AIState.Wandering:
                PassiveUpdate();
                break;
            case AIState.Attacking:
                AttackingUpdate();
                break;
        }
    }

    public void SetState(AIState state)
    {
        aiState = state;

        switch (aiState)
        {
            case AIState.Idle:
                //nav mesh agent에 접근해 상태 수정
                agent.speed = walkSpeed; //speed가 필요없긴하지만 그냥 넣어줘봄
                agent.isStopped = true;
                break;
            case AIState.Wandering: //목표 지점을 찍고 이동
                agent.speed = walkSpeed;
                agent.isStopped = false;
                break;
            case AIState.Attacking: //공격 범위 내
                agent.speed = runSpeed;
                agent.isStopped = false;
                break;
        }

        animator.speed = agent.speed / walkSpeed; //만일 runSpeed면 애니메이터의 스피드도 빠르게하여 달리는 효과 연출
    }

    void PassiveUpdate()
    {
        //어떨 때 Idle이며, Wandering이며, 어떨 때 Attacking으로 갈지 정해줌 0.1 미만으로 남으면 그냥 Idle 상태로 가도록
        if (aiState == AIState.Wandering && agent.remainingDistance < 0.1f)
        {
            SetState(AIState.Idle);
            Invoke("WanderToNewLocation", Random.Range(minWanderWaitTime, maxWanderWaitTime)); //랜덤값이 2가 나오면 Idle 상태서 2초 뒤에 해당함수를 호출함
        }

        if (playerDistance < detectDistance)
        {
            SetState(AIState.Attacking);
        }
    }

    void WanderToNewLocation()
    {
        if (aiState != AIState.Idle) return;

        SetState(AIState.Wandering);
        agent.SetDestination(GetWanderLocation());
    }

    Vector3 GetWanderLocation()
    {
        NavMeshHit hit;
        //sourePosition에 일정한 영역을 지정해주면 그 포지션 안에서 이동할 수 있는 경로에 한해서 최단 경로를 반환함 그 정보가 hit에 담길것
        //onUnitSphere 는 반지름이 1인구임, 가상의 구를 생성해 영역을 만들어 그 중 임의의 점을 반환하며 1의 반지름 값에 랜덤값을 곱해줬음. 일단 모든 레이어 대상으로 할거니 NavMesh.AllAreas로 
        NavMesh.SamplePosition(transform.position + (Random.onUnitSphere * Random.Range(minWanderDistance, maxWanderDistance)), out hit, maxWanderDistance, NavMesh.AllAreas);
        //지정한 영역에서 이동할 수 있는 최단거리의 경로가 hit라는 변수에 저장이 되는 것이다. 

        //hit의 위치와 내가 있는 위치 사이의 거리가 최소범위 안에 있으면 너무 가까이있다 판단하고 while문으로 최대 30회 정도 돌림, 30회나 돌려도 안되면 계속 멈춰있을 순 없으니 이동한다, do while문으로 나중에 함 바꿔보면 왜 쓰는지 알 수 있게 됨
        int i = 0;
        while (Vector3.Distance(transform.position, hit.position) < detectDistance)
        {
            NavMesh.SamplePosition(transform.position + (Random.onUnitSphere * Random.Range(minWanderDistance, maxWanderDistance)), out hit, maxWanderDistance, NavMesh.AllAreas);
            i++;
            if (i == 30) break;
        }

        //목표 지점 반환
        return hit.position;
    }

    void AttackingUpdate()
    {
        if (playerDistance < attackDistance && IsPlayerInFieldOfView()) //공격사거리안에 들어와야하고 시야각 내에 있어야한다.
        {
            agent.isStopped = true; //공격할 때 움직이면 이상하니 true
            if (Time.time - lastAttackTime > attackRate) //공격도 애니메이션이 안끝났는데 공격하면 이상하잖아... 
            {
                lastAttackTime = Time.time;
                CharacterManager.Instance.Player.controller.GetComponent<IDamagable>().TakePysicalDamage(damage);
                animator.speed = 1; //공격할땐 뛰는게 아니니 스피드 1로 
                animator.SetTrigger("Attack");
            }
        }
        else //공격 사거리 밖이거나 시야각에 안들어온 경우   
        {
            if (playerDistance < detectDistance) //추적 범위 안에 있다면?
            {
                agent.isStopped = false;
                NavMeshPath path = new NavMeshPath(); //path안에는 세분화가능한 내용들이 있음 
                if (agent.CalculatePath(CharacterManager.Instance.Player.transform.position, path)) //경로를 게산하여 갈 수 있는 곳이면 true, 갈 수 없는 곳이면 false를 반환한다.
                {
                    //갈 수 있으면 계속 쫓아가야지
                    agent.SetDestination(CharacterManager.Instance.Player.transform.position);
                }
                else
                {
                    //false는 강과 같이 갈 수 없는 지역인 경우임
                    agent.SetDestination(transform.position); //지금 내자리로 목표 설정해두고 멈추고 상태를 Wandering으로 바꾼다.
                    agent.isStopped = true;
                    SetState(AIState.Wandering);
                }
            }
            else //플레이어가 추적 범위 밖으로 멀어진 경우이다. 그러면 추격을 멈춰야겠지
            {
                agent.SetDestination(transform.position); //지금 내자리로 목표 설정해두고 멈추고 상태를 Wandering으로 바꾼다.
                agent.isStopped = true;
                SetState(AIState.Wandering);
            }
        }
    }

    bool IsPlayerInFieldOfView()
    {
        //목표 지점서 내 위치를 빼면 방향이 나옴 
        Vector3 directionToPlayer = CharacterManager.Instance.Player.transform.position - transform.position;
        //몬스터가 정면으로 바라보고 있는 벡터와 위에서 구한 벡터의 사이각을 구해주면 됨
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        return angle < fieldOfView * 0.5; //총각이 120도니 왼쪽/오른쪽을 나누어 0.5를 곱함
    }

    public void TakePysicalDamage(int damage)
    {
        //sword 사용시 데미지 효과가 일어나는지, 몬스터가 죽으면서 아이템을 생성하는지까지 확인하면 됨
        
        health -= damage;
        if(health <= 0)
        {
            //죽는다.
            Die();
        }
        //데미지 효과
        StartCoroutine(DamageFlash());
    }

    void Die()
    {
        //죽으면 템뿌림
        for(int i = 0; i < dropOnDeath.Length; i++)
        {
            Instantiate(dropOnDeath[i].dropPrefab, transform.position + Vector3.up * 2, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    IEnumerator DamageFlash()
    {
        //모든 메쉬 렌더러 색상을 바꾼다.
        for(int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].material.color = new Color(1.0f, 0.6f, 0.6f);
        }

        yield return new WaitForSeconds(0.1f);

        for(int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].material.color = Color.white; //하얀 섬광이 일어나게 
        }
    }
}
