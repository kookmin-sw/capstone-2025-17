using UnityEngine;
using TMPro;

public class Player_Push_Controller : MonoBehaviour
{
    public float pushForce = 3f; // 밀치는 힘의 크기
    private bool canPush = false; // 밀칠 수 있는 상태 여부
    private Rigidbody targetPlayerRb; // 밀칠 대상 플레이어의 Rigidbody

    public Transform holdPosition; // 레이캐스트 발사 위치
    public float detectionRange = 0.05f; // 감지 거리
    public float fieldOfView = 120f; // 감지할 시야각
    public int rayCount = 8; // 감지를 위한 레이의 개수
    public TMP_Text pushUI; // 밀칠 수 있을 때 표시할 UI

    private PickUpController pickUpController; // PickUp 스크립트 참조
    private bool pickUpPriority = false; // PickUp이 우선권을 가지고 있는지 여부

    void Start()
    {
        // UI가 존재하면 비활성화
        if (pushUI != null)
        {
            pushUI.enabled = false;
        }
        
        // holdPosition이 설정되지 않았을 경우 기본값을 transform으로 설정
        if (holdPosition == null)
        {
            holdPosition = transform;
            Debug.LogWarning("holdPosition이 설정되지 않았습니다. transform을 사용합니다.");
        }

        // 현재 오브젝트에서 PickUpController 컴포넌트를 찾음
        pickUpController = GetComponent<PickUpController>();
        if (pickUpController == null) Debug.LogWarning("PickUpController를 찾을 수 없습니다!");
    }

    void Update()
    {
        DetectPlayer(); // 플레이어 감지 함수 호출

        // 플레이어를 밀칠 수 있는 상태에서 마우스 왼쪽 버튼을 누르면 밀침 실행
        if (canPush && Input.GetMouseButtonDown(0))
        {
            PushPlayer();
        }
    }

    void DetectPlayer()
    {
        targetPlayerRb = null; // 감지 초기화
        canPush = false;

        // PickUp이 우선권을 가지고 있으면 밀칠 수 없음
        if (pickUpController != null && pickUpController.IsPickableDetected())
        {
            pickUpPriority = true;
            if (pushUI != null) pushUI.enabled = false; // UI 비활성화
            return; // 감지 중단
        }
        else
        {
            pickUpPriority = false;
        }

        float halfFOV = fieldOfView / 2; // 시야각 절반 계산
        float angleStep = fieldOfView / rayCount; // 레이 간격 계산
        RaycastHit hit;

        // 여러 개의 레이를 사용하여 플레이어 감지
        for (int i = 0; i <= rayCount; i++)
        {
            float angle = -halfFOV + (angleStep * i); // 레이의 각도 계산
            Vector3 direction = Quaternion.Euler(0, angle, 0) * holdPosition.forward;

            // 레이캐스트를 사용하여 플레이어 감지
            if (Physics.Raycast(holdPosition.position, direction, out hit, detectionRange))
            {
                // "Player" 태그가 있는 다른 플레이어 감지
                if (hit.collider.CompareTag("Player") && hit.collider.gameObject != gameObject)
                {
                    targetPlayerRb = hit.collider.GetComponent<Rigidbody>();
                    if (targetPlayerRb != null)
                    {
                        canPush = true;
                        if (pushUI != null && !pickUpPriority)
                        {
                            pushUI.enabled = true;
                            pushUI.text = "Left Click to Push"; // UI 메시지 설정
                        }
                        Debug.Log("플레이어 감지됨: " + hit.collider.gameObject.name);
                        return; // 첫 번째 플레이어 감지 시 종료
                    }
                }
            }
            Debug.DrawRay(holdPosition.position, direction * detectionRange, Color.blue); // 디버그용 레이 표시
        }

        // 플레이어를 감지하지 못했을 경우 UI 숨김
        if (targetPlayerRb == null && pushUI != null)
        {
            pushUI.enabled = false;
        }
    }

    void PushPlayer()
    {
        // 밀칠 대상이 있고 PickUp이 우선권을 갖지 않은 경우 실행
        if (targetPlayerRb != null && !pickUpPriority)
        {
            Vector3 pushDirection = (targetPlayerRb.transform.position - transform.position).normalized; // 밀칠 방향 계산
            targetPlayerRb.AddForce(pushDirection * pushForce, ForceMode.Impulse); // 힘 적용하여 밀치기
            Debug.Log("플레이어를 밀쳤습니다!"); // 디버그 메시지
        }
    }

    // PickUp으로부터 우선순위 설정 함수
    public void SetPickUpPriority(bool priority)
    {
        pickUpPriority = priority;
        if (priority && pushUI != null) pushUI.enabled = false; // PickUp이 우선일 경우 UI 비활성화
    }

    // 플레이어 감지 상태 확인 함수
    public bool IsPlayerDetected()
    {
        return canPush && !pickUpPriority; // 밀칠 수 있는 상태이면서 PickUp이 우선순위가 아닐 때
    }
}
