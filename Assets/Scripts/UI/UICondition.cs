using UnityEngine;

public class UICondition : MonoBehaviour
{
    //UI 컨디션으로 체력, 배고픔, 스테미나에 대한 컨디션 변수를 들고 있도록
    //강의에선 UI 컨디션이라 했지만 플레이어 컨디션이라 해도 무방해보이고 실제 플레이어 컨디션 코드에서 이들을 그대로 가져와서 사용한다.
    public Condition health;
    public Condition hunger;
    public Condition stamina;

    // Start is called before the first frame update
    void Start()
    {
        //플레이어 컨디션의 UI컨디션으로 추가해준다.
        CharacterManager.Instance.Player.condition.uiCondition = this;        
    }
}
