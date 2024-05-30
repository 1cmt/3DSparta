using UnityEngine;
using UnityEngine.UI;

public class Condition : MonoBehaviour
{
    public float curValue; //현재값
    public float startValue; //초기값
    public float maxValue; //최대값
    public float passiveValue;//시간에 따라 줄어들고 회복하고 주기적인 값도 있을 것임 체젠,마젠 같은거임
    public Image uiBar; //게이지 바 이미지를 인스팩터서 넣어주면 해당 오브젝트의 Imgae 컴포넌트가 들어간다. Fill Amount 값이 0이면 빈 게이지고, 1이면 꽉찬 게이지가 된다.

    private void Start()
    {
        curValue = startValue;
    }

    private void Update()
    {
        // ui 업데이트
        uiBar.fillAmount = GetPercentage(); //Imgae 컴포넌트의 fillAmount 값이 1이면 꽉찬 게이지 0이면 빈 게이지임
    }

    //curValue를 maxValue로 나눠주면 0과 1 사이의 값을 얻을 수 있음
    private float GetPercentage()
    {
        return curValue / maxValue;
    }

    public void Add(float value)
    {
        curValue = Mathf.Min(curValue + value, maxValue); //값을 추가할 땐 둘중 작은 것을 넣어줌 최대값을 초과되면 안되니까
    }

    public void Subtract(float value)
    {
        curValue = Mathf.Max(curValue - value, 0); //값을 빼는 것은 minValue 대신 0하면 됨 어차피 0이하의 게이지는 없으니까
    }
}
