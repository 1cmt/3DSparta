using UnityEngine;

public class EquipTool : Equip
{
    public float attackRate; //공격 주기
    private bool attacking;
    public float attackDistance; //최대 공격 가능 거리
    public float useStamina;

    //리소스를 체취할  수 있는지
    [Header("Resource Gathering")]
    public bool doesGatherResources;

    [Header("Combat")]
    public bool DealDamage;
    public int damage;

    private Animator animator;
    private Camera camera;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        camera = Camera.main;
    }

    public override void OnAttackInput()
    {
        if (!attacking)
        {
            if (CharacterManager.Instance.Player.condition.UseStamina(useStamina))
            {
                attacking = true;
                animator.SetTrigger("Attack");
                Invoke("OnCanAttack", attackRate);
            }
        }
    }

    void OnCanAttack()
    {
        attacking = false;
    }

    //애니메이션 수행과 동시에 OnHit를 해버리면 애니메이션이 나가기도 전에 OnHit이 발생될 수 있음
    //애니메이션이 진행되는 와중에 공격하는 그 동작이 나오는 키 프레임 위치에 OnHit이라는 함수를 호출해야함
    public void OnHit()
    {
        //레이를 쏠 것임, 가운데 에임을 기준으로 때렸을 때 EquipTool마다 attackDistance가 있음
        //그 거리 안에 있으면 해당되는 Resource의 Gather함수가 호출될 수 있게 한다. 
        //레이는 애임에서 쏠거고 에임은 화면 가운데 있었으니 카메라를 가져옴
        Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, attackDistance))
        {
            if (doesGatherResources && hit.collider.TryGetComponent(out Resource resource))
            {
                resource.Gather(hit.point, hit.normal);
            }
        }
    }

}
