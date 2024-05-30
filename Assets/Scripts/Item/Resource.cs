using UnityEngine;

public class Resource : MonoBehaviour
{
    public ItemData itemToGive; //나무를 도끼로 채취할 때 어떤 아이템을 줄 것인지
    public int quantityPerHit = 1; //지금은 한번 밸 때 아이템 하나 나옴, 2면 한 번 밸 때 2가지 아이템이 나옴
    public int capacity; //총 몇번때릴 수 있는지
    
    public void Gather(Vector3 hitPoint, Vector3 hitNormal)
    {
        for(int i = 0; i < quantityPerHit; i++)
        {
            if(capacity <= 0) break;
            capacity -= 1;
            //떄린 위치서 조금 위에서 떠러지도록 Vector3.up을 더함 
            Instantiate(itemToGive.dropPrefab, hitPoint + Vector3.up, Quaternion.LookRotation(hitNormal, Vector3.up));
        }
    }
}
