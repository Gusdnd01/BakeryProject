using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.UI;
using TMPro;
using System.Xml.Serialization;

public class TeaTimeUI : SceneUI
{
    [SerializeField] private EatRange _eatRange;
    public EatRange EatRange => _eatRange;

    [SerializeField] private TeaTimeCreamStand _creamStand;
    public TeaTimeCreamStand TeaTimeCreamStand => _creamStand;

    [SerializeField] private CakeCollocation _cakeCollocation;
    public CakeCollocation CakeCollocation => _cakeCollocation;

    [SerializeField] private GetCard _getCard;
    public GetCard GetCard => _getCard;

    [SerializeField]
    private PlayableDirector director;

    [SerializeField]
    private Image cardImage;
    [SerializeField]
    private TextMeshProUGUI cardName;

    public void SetCard(CardInfo cardInfo)
    {
        cardImage.sprite = cardInfo.CardVisual;
        cardName.text = cardInfo.CardName;

        GetCard.GetCakeInfo(cardInfo);
    }

    public void DirectorStart()
    {
        director.Play();
    }

}
