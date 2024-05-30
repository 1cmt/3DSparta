using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public AudioClip[] footstepClips;
    private AudioSource audioSource;
    private Rigidbody _rigidbody;
    public float footstepThreshold; //리지드바디서 움직이고 있는지 아닌지 받아와서 응용, 움직이는 기준점 값을 설정
    public float footstepRate; 
    private float footStepTime;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Mathf.Abs(_rigidbody.velocity.y) < 0.1f) //땅에 붙어있을 때만 효과를 내겠다.
        {
            if(_rigidbody.velocity.magnitude > footstepThreshold)//velocity는 변화량임, 본격적으로 움직이면 변화량이 증가할거임 그 크기를 가져옴 , 그 변화량이 쓰레스홀드 값보다 커야함
            {
                if(Time.time - footStepTime > footstepThreshold)
                {
                    footStepTime = Time.time;
                    audioSource.PlayOneShot(footstepClips[Random.Range(0, footstepClips.Length)]); //먼저 재생된 리소스가 재생이 다 끝나지 않더라도 재생을 끊지않음, 중첩이 가능한거다.
                }
            }
        }
    }
}
