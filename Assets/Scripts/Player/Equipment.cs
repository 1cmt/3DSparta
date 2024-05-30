using UnityEngine;
using UnityEngine.InputSystem;

public class Equipment : MonoBehaviour
{
    //EquipTool 스크립트가 붙어있는 장비를 장착시켜주는 기능을 하늨 클래스
    public Equip curEquip; //현재 장착하고 있는 정보
    public Transform equipParent; //장비를 달아줄 위치 (카메라 위치가 되겠지)

    private PlayerController controller;
    private PlayerCondition condition;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<PlayerController>();
        condition = GetComponent<PlayerCondition>();
    }

    //장착시키는 함수
    public void EquipNew(ItemData data)
    {
        UnEquip(); //기존에 장비가 있다면 해제해야함
        curEquip = Instantiate(data.equipPrefab, equipParent).GetComponent<Equip>();
    }

    public void UnEquip()
    {
        if(curEquip != null)
        {
            Destroy(curEquip.gameObject);
            curEquip = null;
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Performed && curEquip != null && controller.canLook) //canLook이 true인건 인벤토리가 꺼진 상태임
        {
            curEquip.OnAttackInput();
        }
    }
}
