using UnityEngine;
using TMPro;

public class PickUpController : MonoBehaviour
{
    public Transform holdPosition; // 물건을 잡을 때 위치 (플레이어의 손 위치 등)
    private GameObject heldObject; // 현재 잡고 있는 물체
    private Rigidbody heldObjectRb; // 잡고 있는 물체의 리지드바디 (물리적 특성)
    private GameObject detectedObject; // 감지된 물체

    public float detectionRange = 0.05f; // 감지 범위
    public float fieldOfView = 120f; // 물체 감지의 시야각 (각도)
    public int rayCount = 8; // 물체를 감지하기 위한 레이의 개수
    public float pickUpOffset = 0.5f; // 물체를 잡을 때 위치 오프셋
    public TMP_Text pickUpUI; // 물체를 잡을 수 있을 때 UI 텍스트

    private Player_Push_Controller pushController; // 물체 밀기 관련 스크립트
    private bool isPickableDetected; // 물체가 pickable 상태인지 여부

    public float throwForce = 2f; // 물건을 던질 힘 (강도를 낮게 설정)

    void Start()
    {
        if (pickUpUI != null) pickUpUI.enabled = false; // UI 텍스트 초기화

        // 동일한 오브젝트에서 Push 스크립트 가져오기
        pushController = GetComponent<Player_Push_Controller>();
        if (pushController == null) Debug.LogWarning("Player_Push_Controller를 찾을 수 없습니다!"); // 경고: Push 컨트롤러가 없을 경우
    }

    void Update()
    {
        DetectPickableObject(); // 물체 감지 함수 호출

        if (Input.GetKeyDown(KeyCode.F)) // F 키를 눌렀을 때
        {
            if (heldObject == null) TryPickUp(); // 물건을 잡을 수 있으면 잡기
            else DropObject(); // 이미 잡은 물건이 있으면 놓기
        }

        if (heldObject != null)
        {
            // 물체를 잡은 후 물체의 위치와 UI 텍스트 업데이트
            heldObject.transform.position = holdPosition.position + holdPosition.forward * pickUpOffset;
            if (pickUpUI != null)
            {
                pickUpUI.enabled = true;
                pickUpUI.text = "Press F to Drop";
            }

            if (Input.GetKeyDown(KeyCode.R)) RotateHeldObject(); // 물체 회전

            // 물체 던지기 (좌클릭)
            if (Input.GetMouseButtonDown(0)) // 좌클릭 시 물체 던지기
            {
                ThrowObject();
            }
        }
    }

void DetectPickableObject()
{
    detectedObject = null; // 초기화
    isPickableDetected = false; // 초기화
    float halfFOV = fieldOfView / 2; // 시야각의 반
    float angleStep = fieldOfView / rayCount; // 각도 간격
    RaycastHit hit;

    // 여러 방향으로 레이를 쏴서 감지
    for (int i = 0; i <= rayCount; i++)
    {
        float angle = -halfFOV + (angleStep * i); // 레이의 각도 계산
        Vector3 direction = Quaternion.Euler(0, angle, 0) * holdPosition.forward;

        // 레이를 쏘아서 물체를 감지
        if (Physics.Raycast(holdPosition.position, direction, out hit, detectionRange))
        {
            if (hit.collider.CompareTag("Pickable")) // "Pickable" 태그가 붙은 물체 감지
            {
                detectedObject = hit.collider.gameObject; // 물체 저장
                isPickableDetected = true; // pickable 물체 감지됨
                if (pickUpUI != null && heldObject == null)
                {
                    pickUpUI.enabled = true;
                    pickUpUI.text = "Press F to Pick Up"; // UI 텍스트 변경
                }
                Debug.Log("물건 감지중: " + detectedObject.name); // 디버그 메시지
                return; // 첫 번째 물체에서 종료
            }
        }
        Debug.DrawRay(holdPosition.position, direction * detectionRange, Color.red); // 레이 디버깅
    }

    // 물체를 감지하지 못한 경우 UI 숨김
    if (detectedObject == null && heldObject == null && pickUpUI != null)
    {
        pickUpUI.enabled = false; // UI를 숨김
        isPickableDetected = false; // 감지되지 않음
    }

    // Push 스크립트에 물체 감지 상태 전달
    if (pushController != null) pushController.SetPickUpPriority(isPickableDetected);
}


    void RotateHeldObject()
    {
        if (heldObject != null)
        {
            // 물체를 90도 회전
            Quaternion currentRotation = heldObject.transform.rotation;
            Quaternion newRotation = currentRotation * Quaternion.Euler(0, 0, -90);
            heldObject.transform.rotation = newRotation;
        }
    }

    void TryPickUp()
    {
        if (detectedObject == null) return; // 감지된 물체가 없으면 종료

        heldObject = detectedObject; // 물체 잡기
        heldObjectRb = heldObject.GetComponent<Rigidbody>(); // 물리 특성 가져오기

        if (heldObjectRb != null)
        {
            heldObjectRb.isKinematic = true; // 물리 계산 비활성화 (물체를 이동시키기 위함)
            Collider heldObjectCollider = heldObject.GetComponent<Collider>();
            if (heldObjectCollider != null && heldObject.CompareTag("Pickable"))
            {
                heldObjectCollider.isTrigger = true; // 물체의 충돌을 트리거로 설정
            }
            heldObject.transform.position = holdPosition.position + holdPosition.forward * pickUpOffset; // 물체 위치 설정
            heldObject.transform.rotation = holdPosition.rotation; // 물체 회전 설정
            heldObject.transform.parent = holdPosition; // 부모 오브젝트 설정 (플레이어)
        }
    }

    void DropObject()
    {
        if (heldObject != null)
        {
            heldObjectRb.isKinematic = false; // 물리 계산 활성화
            heldObjectRb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic; // 충돌 감지 설정
            heldObject.transform.parent = null; // 부모 오브젝트 해제

            if (heldObject.CompareTag("Pickable"))
            {
                Collider heldObjectCollider = heldObject.GetComponent<Collider>();
                if (heldObjectCollider != null)
                {
                    heldObjectCollider.isTrigger = false; // 트리거 해제
                }
            }

            heldObject.transform.position += Vector3.up * 0.1f; // 물체를 살짝 띄워서 떨어지도록 함
            heldObject = null; // 물체를 놓음
        }
    }

    // 물체 던지기
    void ThrowObject()
    {
        if (heldObject != null && heldObjectRb != null)
        {
            // 물리 충돌 감지 활성화
            Collider heldObjectCollider = heldObject.GetComponent<Collider>();
            if (heldObjectCollider != null)
            {
                heldObjectCollider.isTrigger = false; // 물리 충돌 처리 활성화
            }

            heldObjectRb.isKinematic = false; // 물리 활성화
            heldObjectRb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic; // 충돌 감지 모드 설정

            // 던지는 방향은 플레이어의 정면
            Vector3 throwDirection = holdPosition.forward;

            // 던지는 힘 설정
            heldObjectRb.AddForce(throwDirection * throwForce, ForceMode.VelocityChange);

            heldObject.transform.parent = null; // 부모 설정 해제

            // 물체가 땅 아래로 떨어지지 않도록 약간 위로 띄움
            heldObject.transform.position += Vector3.up * 0.1f;

            heldObject = null; // 물체를 놓음
        }
    }

    // 외부에서 물체 감지 상태를 확인하기 위한 함수
    public bool IsPickableDetected()
    {
        return isPickableDetected || heldObject != null; // 물체가 감지되었거나 물체를 이미 들고 있는 상태
    }
}
