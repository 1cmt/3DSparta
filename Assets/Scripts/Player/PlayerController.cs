using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")] 
    public float moveSpeed;
    public float jumpPower;
    private Vector2 curMovementInput; //Move 액션 발생시 Input System을 통해 들어오는 Vector2값을 넣어주는 변수이다.
    public LayerMask groundLayerMask; // 공중 점프를 방지하기 위해 땅을 검출하기 위한 레이어마스크이다.

    [Header("Look")]
    public Transform cameraContainer; //왜 컨테이녀인가...? 카메라 컨테이너엔 무기만 따로 찍는 카메라가 추가적으로 있음
    private float camCurXRot; //카메라의 x회전 각도, 마우스의 y 델타값에 따라 카메라를 상하로 회전시켜 케릭터가 고개를 상하로 움직이는 모습을 구현한다.
    public float minXLook; //고개를 한없이 내릴 수는 없기에 최소 값을 주는 것이다.
    public float maxXLook; //고개를 한없이 올릴 수는 없기에 최소 값을 주는 것이다.
    public float lookSensitivity; //회전할 때 민감도
    private Vector2 mouseDelta; //Look 액션 발생시 Input System을 통해 들어오는 Vector2값을 넣어주는 변수이다.

    [Header("Inventory")]
    public Action inventory;
    [HideInInspector] public bool canLook = true; //고개를 돌릴 수 있냐의 의미로 받아들이면 쉽다, 인벤토리를 컨트롤할 때는 고개를 돌릴 수 없게할 것이다.
    
    private Rigidbody _rigidbody;
    public Rigidbody Rigidbody { get { return _rigidbody; } }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        //인벤토리 여는 상황 아니면 커서는 보이면 안된다. 마우스 커서를 숨기는 코드이다. 초기 상태엔 숨겨야함
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        //Move함수서 rigidbody 관련 연산을 하고 있어 물리 기능을 사용하고 있어 FixedUpdate에서 처리
        Move();
    }

    private void LateUpdate()
    {
        //케릭터의 이동이 끝난 후에 카메라가 이동하는 것이 자연스럽기에 LateUpdate에 작성
        if (canLook) CameraLook();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        //CallbackContext에선 현재 Input 상태를 받아 올 수 있음
        //phase는 분기점, 키가 딱 눌린건 Started, 키가 눌린 상태에서도 값을 받을 수 있게 Performed로 값을 설정
        if (context.phase == InputActionPhase.Performed)
        {
            curMovementInput = context.ReadValue<Vector2>();
        }
        else if (context.phase == InputActionPhase.Canceled) //키가 떨어졌을 때는 가만히 있어야함
        {
            curMovementInput = Vector2.zero;
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        //버튼은 눌렀을 때 딱 돼야하니 Started, 공중 점프를 방지하기 위해 IsGrounded 함수를 통해 땅고
        if (context.phase == InputActionPhase.Started && IsGrounded())
        {
            //힘을 한번에 줘야하니 ForceMode는 Impulse로
            _rigidbody.AddForce(Vector2.up * jumpPower, ForceMode.Impulse);
        }
    }

    public void OnInventory(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            inventory?.Invoke();
            ToggleCursor(); //인벤토리를 볼떄는 커서가 등장해야한다.
        }
    }

    private void Move()
    {
       //curMovementInput.y는 인풋시스템서는 위아래라 y값이지만 케릭터는 앞뒤로 움직여야하기 때문에 z방향(forward)에 curMovementInput.y를 곱해주는 것이다.
        Vector3 dir = transform.forward * curMovementInput.y + transform.right * curMovementInput.x;
        dir *= moveSpeed;
        dir.y = _rigidbody.velocity.y; //점프를 했을 때만 y값을 조절함으로 기존의 값을 유지시키고자 기존의 y의 값을 넣어주는 것.
        _rigidbody.velocity = dir; 
    }

    void CameraLook()
    {
        //케릭터에 카메라가 부착되어있으니 좌우는 케릭터를 회전시키고 상하는 카메라를 회전시키는 것이다.
        //마우스의 y 위치가 위에있으면 위를 보고 아래에 있으면 아래를 보게 됨 고개를 상하로 움직인다 생각하면 된다.
        //고로 카메라의 상하 회전은 마우스 델타의 y 값에 달려있고 카메라 회전은 왼손좌표계이므로 카메라의 x축 기준으로 해야함
        camCurXRot += mouseDelta.y * lookSensitivity; // = 대입이 아니라 기존값에 더해주고 대입한 이유는 delta 값이기 때문이다.
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook); //고개 돌리는데도 제한이 필요해 Clamp 함수로 제한값을 벗어나지 않도록 한 것이다.
        cameraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0);
        //유니티는 왼손좌표계라 엄지를 축이라 생각하고 감아지는 방향이 +임 즉 +방향으로 갈수록 카메라는 아래를 보는데 실제 마우스 delta y값은 위로 올릴 때 +가 되니 부호를 바꿔주는 것이다.

        //좌우 회전은 마우스 델타의 x 값에 달려있고 좌우 회전은 카메라가 아닌 케릭터의 y축 기준으로 해야함
        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0);
    }

    //공중 점프를 막기 위해 땅에 붙어있는지를 체크
    private bool IsGrounded()
    {
        //플레이어 기준으로 책상 다리 4개를 만들어 다리가 땅에 닿는 지를 확인하는 것임
        Ray[] rays = new Ray[4]
        {
            //transform.position 중심으로 부터 앞쪽(forward), 뒤쪽(-forward)으로 하나씩 레이를 만들고 오른쪽(right), 왼쪽(-right)에 하나씩 레이를 만들어 총 4개의 레이를 만듬
            //transform.up * 0.01f를 더한 이유, 땅에 닿아있을 때 레이를 바로 플레이어 있는데서 쏘게 되면 Ground를 인지못하고 Ground에서 레이를 쏘게되는 경우가 생길 수 있음
            new Ray(transform.position + (transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (transform.right * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.right * 0.2f) + (transform.up * 0.01f), Vector3.down)
        };

        for (int i = 0; i < rays.Length; i++)
        {
            if (Physics.Raycast(rays[i], 0.1f, groundLayerMask)) //길이를 0.1로 짧게 레이를 쏘고 ground 레이마스크에 해당되는 애들만 검출함, ground 레이어 마스크는 everthing서 player만 빼준상태 
            {
                return true;
            }
        }

        return false;
    }

    void ToggleCursor()
    {
        bool toggle = Cursor.lockState == CursorLockMode.Locked; //락이 걸려져있는건 마우스 커서가 숨겨져있는 것
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked; //toggle이 true로 락이 되어있다면 None으로 안걸려있다면 락을 걸어줘 커서를 숨김 반대로 false로 락이 없다면 락을 걸어줌
        canLook = !toggle; //toggle과 반대로 설정
        //즉 해당 코드는 삼항연산자와 !연산자를 통해 tab을 눌러 인벤토리를 키고 다시 tab을 눌러 인벤토리를 끄는 과정 모두에서 커서와 Look에 대한 처리할 수 있도록 설계되어있다.
    }
}

// public class PlayerController : MonoBehaviour
// {
//     [Header("Movement")]
//     public float moveSpeed;
//     public float jumpPower;
//     private Vector2 curMovementInput;
//     public LayerMask groundLayerMask;

//     [Header("Look")]
//     public Transform cameraContainer;
//     public float minXLook;
//     public float maxXLook;
//     private float camCurXRot; //input action서 마우스의 delta 값을 받아오는데 그 값을 넣음
//     public float lookSensitivity; //회전할 때 민감도
//     private Vector2 mouseDelta;
//     public bool canLook = true;  //인벤토리를 컨트롤할 커서가 필요함, 커서가 없어서 볼 수 있는 상태임

//     public Action inventory;
//     private Rigidbody _rigidbody;

//     private void Awake()
//     {
//         _rigidbody = GetComponent<Rigidbody>();
//     }

//     private void Start()
//     {
//         //플레이시 마우스 모양이 보이지 않음, 마우스 모양을 숨기는 코드이다.
//         Cursor.lockState = CursorLockMode.Locked;
//     }

//     private void FixedUpdate()
//     {
//         //Move함수서 rigidbody 관련 연산을 하고 있으니 FixedUpdate서
//         Move();
//     }

//     private void LateUpdate()
//     {
//         if (canLook) CameraLook();
//     }

//     private void Move()
//     {
//         //curMovementInput.y는 인풋시스템서는 위아래라 y값이지만 케릭터는 앞뒤로 움직여야하기 때문에 z방향(forward)에 curMovementInput.y를 곱해주는 것이다.
//         Vector3 dir = transform.forward * curMovementInput.y + transform.right * curMovementInput.x;
//         dir *= moveSpeed;
//         dir.y = _rigidbody.velocity.y; //점프를 했을 때만 위아래로 움직여야함으로 기존의 값을 유지시키고자
//         _rigidbody.velocity = dir;
//     }

//     void CameraLook()
//     {
//         //케릭터에 카메라가 부착되어있으니 좌우는 케릭터를 회전시키고 상하는 카메라를 회전시키는 것이다.
//         //상하 회전은 마우스 델타의 y 값에 달려있고 카메라 회전은 카메라의 x축 기준으로 해야함
//         camCurXRot += mouseDelta.y * lookSensitivity;
//         camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook);
//         cameraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0);
//         //유니티는 왼손좌표계라 엄지를 축이라 생각하고 감아지는 방향이 +임 즉 +방향으로 갈수록 카메라는 아래를 보는데 실제 마우스 delta y값은 위로 올릴 때 +가 되니 부호를 바꿔주는 것이다.

//         //좌우 회전은 마우스 델타의 x 값에 달려있고 회전은 케릭터의 y축 기준으로 해야함
//         transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0);
//     }

//     public void OnMove(InputAction.CallbackContext context)
//     {
//         //CallbackContext에선 현재 상태를 받아 올 수 있음
//         //phase는 분기점, 키가 눌린건 Started, 키가 눌린 상태에서도 값을 받을 수 있게
//         if (context.phase == InputActionPhase.Performed)
//         {
//             curMovementInput = context.ReadValue<Vector2>();
//         }
//         else if (context.phase == InputActionPhase.Canceled) //키가 떨어졌을 때는 가만히 있어야함
//         {
//             curMovementInput = Vector2.zero;
//         }
//     }

//     public void OnLook(InputAction.CallbackContext context)
//     {
//         //mouseDelta는 마지막 프레임과 현재 프레임 사이의 마우스 포인터 위치의 차이, 즉 이전 좌표(벡터) 기준으로 계산
//         mouseDelta = context.ReadValue<Vector2>(); //마우스는 phase 없이 값이 유지됨
//     }

//     public void OnJump(InputAction.CallbackContext context)
//     {
//         //버튼은 눌렀을 때 딱 돼야하니 Started
//         if (context.phase == InputActionPhase.Started && IsGrounded())
//         {
//             _rigidbody.AddForce(Vector2.up * jumpPower, ForceMode.Impulse);
//         }
//     }

//     //공중 점프를 막기 위해 땅에 붙어있는지를 체크
//     private bool IsGrounded()
//     {
//         //플레이어 기준으로 책상 다리 4개를 만듬
//         Ray[] rays = new Ray[4]
//         {
//             //transform.position 중심으로 부터 z축 방향으로 앞쪽, 뒤쪽으로 하나씩 만들고 x축 방향으로 앞쪽, 뒤쪽 하나씩 다리 만들고 아래로 쏘는 레이 만듬
//             //transform.up * 0.01f를 한 이유, 땅에 닿아있을 때 레이를 바로 플레이어 있는데서 쏘게 되면 Ground를 인지못하고 Ground에서 레이를 쏘게되는 경우가 생길 수 있음
//             new Ray(transform.position + (transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
//             new Ray(transform.position + (-transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
//             new Ray(transform.position + (transform.right * 0.2f) + (transform.up * 0.01f), Vector3.down),
//             new Ray(transform.position + (-transform.right * 0.2f) + (transform.up * 0.01f), Vector3.down)
//         };

//         for (int i = 0; i < rays.Length; i++)
//         {
//             if (Physics.Raycast(rays[i], 0.1f, groundLayerMask)) //0.1로 짧게 레이를 쏘고 ground 레이마스크에 해당되는 애들만 검출함, ground 레이어 마스크는 everthing서 player만 뺌 
//             {
//                 return true;
//             }
//         }

//         return false;
//     }

//     public void OnInventory(InputAction.CallbackContext context)
//     {
//         if (context.phase == InputActionPhase.Started)
//         {
//             inventory?.Invoke();
//             ToggleCursor();
//         }
//     }

//     void ToggleCursor()
//     {
//         bool toggle = Cursor.lockState == CursorLockMode.Locked; //락이 걸려져있는건 마우스 커서가 숨겨져있는 것
//         Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked; //toggle이 true로 락이 되어있다면 None으로 안걸려있다면 락을 걸어줘 커서를 숨김
//         canLook = !toggle; //toggle과 반대로
//     }
// }
