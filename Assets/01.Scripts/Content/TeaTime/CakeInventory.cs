using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CakeInventory : MonoBehaviour
{
    [SerializeField] private RectTransform _content;
    [SerializeField] private float _contentStretchValue = 260;
    [SerializeField] private CakeInventoryElement _cakeElementPrefab;
    [SerializeField] private CakeCollocation _cakeCollocation;
    [SerializeField] private CakeInventoryPanel _cakeInvenPanel;

    private void Start()
    {
        List<ItemDataSO> cakeList = Inventory.Instance.GetSpecificTypeItemList(ItemType.Bread);

        for(int i = 0; i < cakeList.Count; i++)
        {
            if(i % 5 == 0)
            {
                _content.sizeDelta = 
                new Vector2(_content.sizeDelta.x, _content.sizeDelta.y + _contentStretchValue);
            }

            CakeInventoryElement cie =Instantiate(_cakeElementPrefab, _content);

            cie.SetInfo(cakeList[i], _cakeCollocation, _cakeInvenPanel);
        }
    }
}
