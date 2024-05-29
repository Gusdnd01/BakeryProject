using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardManagingVisualSetter : MonoBehaviour
{
    [SerializeField] private GameObject _cardManagingVisualGroup;
    [SerializeField] private GameObject _abillityVisualGroup;

    [SerializeField] private GameObject _requestSelectCardMark;

    public void UnMarking()
    {
        _requestSelectCardMark.SetActive(true);

        _cardManagingVisualGroup.SetActive(false);
        _abillityVisualGroup.SetActive(false);
    }

    public void Marking()
    {
        _requestSelectCardMark.SetActive(false);

        _cardManagingVisualGroup.SetActive(true);
        _abillityVisualGroup.SetActive(true);
    }
}
