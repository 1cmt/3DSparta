using UnityEngine;

public class Test : MonoBehaviour
{
    void Start()
    {
        // 예시: UIInventory 타입의 오브젝트를 찾습니다.
        // 찾고자 하는 원하는 클래스 명을 <>안에 넣어주세요.
        FindObjects<UIInventory>();
    }

    // 제네릭 메서드: T 타입의 오브젝트를 찾아 로그에 출력합니다.
    void FindObjects<T>() where T : MonoBehaviour
    {
        // 씬 내의 모든 오브젝트를 탐색하여 T 타입의 오브젝트들을 찾습니다.
        T[] scripts = FindObjectsOfType<T>();

        if (scripts.Length > 0)
        {
            foreach (T script in scripts)
            {
                Debug.Log(typeof(T).Name + " 스크립트를 가진 오브젝트의 이름: " + script.gameObject.name);
            }
        }
        else
        {
            Debug.Log(typeof(T).Name + " 스크립트를 가진 오브젝트를 찾을 수 없습니다.");
        }
    }
}