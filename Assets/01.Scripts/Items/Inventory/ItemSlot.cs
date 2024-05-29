using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Image _itemImage;
    [SerializeField] private TextMeshProUGUI _itemAmountText;
    [SerializeField] private Sprite _emptySlotSprite;

    public InventoryItem item;

    public void UpdateSlot(InventoryItem newItem)
    {
        item = newItem;

        if (item != null)
        {
            _itemImage.sprite = item.itemDataSO.itemIcon;
            if (_itemAmountText != null)
            {
                if (item.stackSize > 1)
                {
                    _itemAmountText.text = item.stackSize.ToString();
                }
                else
                {
                    _itemAmountText.text = string.Empty;
                }
            }
        }
        else
        {
            CleanUpSlot();
        }
    }

    public void CleanUpSlot()
    {
        item = null;
        _itemImage.sprite = _emptySlotSprite;
        if (_itemAmountText != null)
        {
            _itemAmountText.text = string.Empty;
        }
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (item == null) return;

        ItemDataIngredientSO ingredientSO = (ItemDataIngredientSO)item.itemDataSO;

        if (transform.parent.name == "UseIngredientParent")
        {
            Debug.Log("사용됐고 사용한 재료 Stash의 Dictionary에 들어있다");

            ((ItemDataIngredientSO)item.itemDataSO).isUsed = false;
            //Inventory.Instance.AddItem(item.itemDataSO);
            Debug.Log("isUsed false로 해주고 사용한 재료에서 빼주고 인벤토리의 재료 인벤토리에 넣어준다");

            return;
        }

        //if (ingredientSO.isUsed
        //    && BakingManager.Instance.usedIngredientStash.usedIngredDictionary.TryGetValue(ingredientSO.ingredientType, out InventoryItem usedInvenItem))
        //{
            
        //}

        // 클릭 시 제빵 팝업이 열려있고, 현재 슬롯의 타입이 Ingredient(재료)라면
        if (item.itemDataSO.itemType == ItemType.Ingredient)
        {
            
            return;
        }

        if (Keyboard.current.ctrlKey.isPressed)
        {
            Inventory.Instance.RemoveItem(item.itemDataSO);
            return;
        }

       /* if(item.itemDataSO.itemType == ItemType.Bread)
        {
            //Inventory.Instance.breadWindow.EquipItem(item.itemDataSO as ItemDataEquipmentSO);

            Inventory.Instance.RemoveItem(item.itemDataSO);
        }*/

    }
}