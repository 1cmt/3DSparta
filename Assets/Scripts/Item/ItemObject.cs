using UnityEngine;

//carrot인지 판단하고 sword인지 판단하고 하면 복잡함.. 아이템이 100가지면 100개에 대한 걸 다 검사하긴 무리
////지금은 모든 ItemObject로 아이템을 퉁쳐서 만들었지만 아이템마다 다른 정보를 표현해줄 수도 있는 것이다. 고로 인터페이스를 사용
public interface IInteractable
{
    public string GetIneractPrompt(); //화면에 띄울 프롬프트에 관련된 함수
    public void OnIneteract(); //interact 됐을 때 어떤 효과를 발생시킬 것인지
}

public class ItemObject : MonoBehaviour, IInteractable
{
    public ItemData data;

    public string GetIneractPrompt()
    {
        string str = $"{data.displayName}\n{data.description}"; //맨윗줄에 이름, 아랫줄에 설명란이 뜨는 것을 반환
        return str;
    }

    public void OnIneteract() //e키 눌렀을 때 호출된 함수, 인터렉션을 하겠다는 거임 
    {
        //아이템 데이터를 넘겨주는 과정
        CharacterManager.Instance.Player.itemData = data; 
        CharacterManager.Instance.Player.addItem?.Invoke();
        Destroy(gameObject); //e키 누르면 아이템은 인벤토리로 이동시킬 것임 고로 맵 위에 있는 것은 사라짐
    }
}
