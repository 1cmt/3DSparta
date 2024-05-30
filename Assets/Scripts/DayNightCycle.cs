using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Range(0.0f, 1.0f)] public float time; //time은 범위가 0 ~ 1로 0%에서 100%
    public float fullDayLength; //전체적 하루 길이
    public float startTime = 0.4f; //타임이 0.5 일 때 정오라 생각 (각도가 90도), 0% ~ 100%로 표현하기에 0.5가 12시임
    private float timeRate;
    public Vector3 noon; //Vector가 90 0 0, 해가 정중앙

    [Header("Sun")]
    public Light sun;
    public Gradient sunColor;//그라데이션으로 서서히 색이 변하도록 하고자
    public AnimationCurve sunIntensity;

    [Header("Moon")]
    public Light moon;
    public Gradient moonColor;
    public AnimationCurve moonIntensity;

    [Header("Other Lighting")]
    public AnimationCurve lightingIntensityMultiplier;
    public AnimationCurve reflectionIntensityMultiplier;

    // Start is called before the first frame update
    void Start()
    {
        timeRate = 1.0f / fullDayLength;
        time = startTime;
    }

    // Update is called once per frame
    void Update()
    {
        time = (time + timeRate * Time.deltaTime) % 1.0f; //1넘어가면 안되니까 % 1.0f

        UpdateLighting(sun, sunColor, sunIntensity);
        UpdateLighting(moon, moonColor, moonIntensity);

        RenderSettings.ambientIntensity = lightingIntensityMultiplier.Evaluate(time);
        RenderSettings.reflectionIntensity = reflectionIntensityMultiplier.Evaluate(time);
    }

    void UpdateLighting(Light lightSource, Gradient gradient, AnimationCurve intensityCurve)
    {
        float intensity = intensityCurve.Evaluate(time); //보간된 값을 받아올 거임

        lightSource.transform.eulerAngles = (time - (lightSource == sun ? 0.25f : 0.75f)) * noon * 4f;
        //time이 0.5면 정오인 시간임 각도가 90도가 돼야함, 근데 한바퀴는 360도라 0.5면 180이 되니 0.25로 조정해줘야함 고로 0.25를 빼주는거임 moon은 sun이랑 180도 관계에 있으니 0.5가 더해 0.75를 뺌
        //달은 해의 반대편이 있음
        //0.25에 90곱하면 90이 안나오니 4를 곱해줌
        lightSource.color = gradient.Evaluate(time);
        lightSource.intensity = intensity;

        GameObject go = lightSource.gameObject;
        
        if(lightSource.intensity == 0 && go.activeInHierarchy)
        {
            go.SetActive(false);
        }
        else if(lightSource.intensity > 0 && !go.activeInHierarchy)
        {
            go.SetActive(true);
        }
    }
}
