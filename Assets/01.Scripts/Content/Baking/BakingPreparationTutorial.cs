using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cinemachine;
using UnityEngine.InputSystem;

public class BakingPreparationTutorial : MonoBehaviour
{
    /// <summary>
    /// 필요한 내용
    /// 몬스터 처치 후 대화 진행 - 인벤토리에서는 이런 걸 할 수 있다 알려줌
    /// (튜토리얼을 위한 재료를 모두 모으기 전까지는 아무것도 없음)
    /// 필요한 재료를 모두 모은 후 대화 진행 - 대충 이제 다 모았으니 집 가자는 내용
    /// </summary>

    [SerializeField]
    private List<ItemDataIngredientSO> _necessaryIngredList; // 필요한 재료 목록
    private bool _isCompleted = false;                       // 모든 재료를 모았는지
    [SerializeField]
    private GameObject _inventoryTutorialPanel;              // 인벤토리 튜토리얼 패널(하나로 전부 설명할 예정)

    private void Start()
    {
        //GameManager.Instance.Player.onPickUpItem += TraverseInventoryHandle;
    }

    public void TraverseInventoryHandle()
    {
        if (DataManager.Instance.CheckOnFirstData.isFirstPickUpitem)
        {
            // 인벤 여는 버튼 알려주는 혼잣말 출력
            Debug.Log("인벤 열어라");
            DataManager.Instance.CheckOnFirstData.isFirstPickUpitem = false;
        }

        // 필요한 재료 목록만큼 반복
        for (int i = 0; i < _necessaryIngredList.Count; ++i)
        {
            // 만약 인벤토리 딕셔너리에 하나라도 없다면
            if (!Inventory.Instance.IsHaveItem(_necessaryIngredList[i]))
            {
                // 실패 띄우고 반복문 탈출
                _isCompleted = false;
                break;
            }
            // 만약 인벤토리 딕셔너리에 있다면
            else
            {
                // 우선 성공을 띄움
                _isCompleted = true;
            }
        }

        // 전부 돌고난 후 성공했다면(필요한 모든 재료가 있다면)
        if (_isCompleted)
        {
            // 대충 이제 집 가서 케이크 만들자는 이야기 출력
            Debug.Log("집 가서 케이크 만들어라");
            //GameManager.Instance.Player.onPickUpItem -= TraverseInventoryHandle;
        }
    }
}
