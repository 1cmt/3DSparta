using UnityEngine;

public class MusicZone : MonoBehaviour
{
    public AudioSource audioSource;
    public float fadeTime;
    public float maxVolume;
    private float targetVolume;

    // Start is called before the first frame update
    void Start()
    {
        targetVolume = 0;
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = targetVolume;
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        //근삿값이 아닐때 안의 로직이 실행됨
        if(!Mathf.Approximately(audioSource.volume, targetVolume)) //1.0...과 1.0...을 비교하면 아주 미세하게 다를 수가 있음 1.0000000000123 이럴 수도 있는거고... 이 사소한 값을 무시하고자 근삿값을 구한거임 근삿값 범위 내면 같은 값으로 생각
        {
            audioSource.volume = Mathf.MoveTowards(audioSource.volume, targetVolume, (maxVolume / fadeTime) * Time.deltaTime); //점진적으로 볼륨이 늘어나도록 볼륨이 너무 클 수 있으니 maxVolume 대비에 올라가도록
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            targetVolume = maxVolume;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            targetVolume = 0f;
        }
    }
}
