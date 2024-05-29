using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GetCakePanel : MonoBehaviour
{
    [SerializeField] private Image _cakeVisual;
    [SerializeField] private TextMeshProUGUI _cakenameText;

    public void SetUp(ItemDataBreadSO cakeData)
    {
        gameObject.SetActive(true);

        _cakeVisual.sprite = cakeData.itemIcon;
        _cakenameText.text = cakeData.itemName;
    }
}
