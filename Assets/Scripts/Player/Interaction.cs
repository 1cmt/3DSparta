using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interaction : MonoBehaviour
{
    //카메라 기준으로 레이 쏴야함
    public float checkRate = 0.05f; //얼마나 자주 최신화하여 레이를 쏠지 사긴을 정함
    private float lastCheckTime; //마지막으로 쏜게 언젠지
    public float maxCheckDistance; //레이를 쏠 최대거리
    public LayerMask layerMask; //레이를 쏠 때 레이어마스크를 두어 불필요한 충돌을 거를 수 있도록

    //캐싱하는 자료가 두 변수에 들어있음
    public GameObject curInteractGameObject;
    private IInteractable curInteractable;

    public TextMeshProUGUI promptText;
    private Camera _camera;

    // Start is called before the first frame update
    private void Start()
    {
        _camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        //매프레임마다 레이 쏘는건 방지해
        if (Time.time - lastCheckTime > checkRate)
        {
            lastCheckTime = Time.time;

            //카메라 기준으로 레이를 쏨
            Ray ray = _camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2)); //정중앙서 쏜다.

            if (Physics.Raycast(ray, out RaycastHit hit, maxCheckDistance, layerMask))
            {
                //현재 interact하고 있는 오브젝트가 아니라면
                if (hit.collider.gameObject != curInteractGameObject)
                {
                    //레이가 충돌한 오브젝트를 현재 interact하는 게임 오브젝트로 저장
                    curInteractGameObject = hit.collider.gameObject;
                    //GetComponent로 인터페이스를 가져올 수 있으며 인터페이스 내 구현된 함수를 바로 쓸 수 있다는 장점이 있다.
                    curInteractable = hit.collider.GetComponent<IInteractable>();
                    SetPrompText(); //프롬포트에 출력해줘라
                }
            }
            else //레이에 충돌한 오브젝트가 없다면?
            {
                //다 null로 로 만들고 프롬프트도 숨겨준다.
                curInteractGameObject = null;
                curInteractable = null;
                promptText.gameObject.SetActive(false);
            }
        }
    }

    private void SetPrompText()
    {
        promptText.gameObject.SetActive(true);
        promptText.text = curInteractable.GetIneractPrompt(); 
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        //OnInteract의 경우엔 e를 눌러야 된다.
        if(context.phase == InputActionPhase.Started && curInteractable != null)
        {
            curInteractable.OnIneteract();
            curInteractGameObject = null;
            curInteractable = null;
            promptText.gameObject.SetActive(false);
        }
    }
}
