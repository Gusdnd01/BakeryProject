using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardDefine;
using UnityEngine.EventSystems;
using System;

public class ActivationChecker : MonoBehaviour
{
    [SerializeField] private RectTransform _waitZone;
    private Vector2 _mousePos;

    private int _selectIDX;

    public bool IsMouseInWaitZone()
    {
        _mousePos = MaestrOffice.GetWorldPosToScreenPos(Input.mousePosition);
        return UIFunction.IsMouseInRectTransform(_mousePos, _waitZone);
    }

    private void Update()
    {
        CheckActivation();

        if (!IsPointerOnCard()) return;
        BindMouse();
    }

    private void BindMouse()
    {
        if (Input.GetMouseButton(0) && CardReader.OnBinding && CardReader.OnPointerCard.CanUseThisCard)
        {
            Transform cardTrm = CardReader.OnPointerCard.transform;
            Vector3  mousePos = MaestrOffice.GetWorldPosToScreenPos(Input.mousePosition);

            float distance = (mousePos - cardTrm.position).x;
            float rotation = Mathf.Clamp(distance * 50, -30, 30);

            Vector3 euler = cardTrm.eulerAngles;
            euler.z = rotation;

            cardTrm.eulerAngles = Vector3.Lerp(cardTrm.eulerAngles, euler, Time.time * 20);
            cardTrm.position = Vector3.Lerp(cardTrm.position, mousePos, Time.deltaTime * 20);
        }
    }

    private void SelectOnPointerCard()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, results);

        foreach(RaycastResult result in results)
        {
            if(result.gameObject.transform.parent.TryGetComponent<CardBase>(out CardBase c))
            {
                if (!CardReader.OnBinding || c.CanUseThisCard)
                {
                    RectTransform rt = c.transform as RectTransform;
                    CardReader.OnPointerCard = c;
                    rt.SetAsLastSibling();
                }
                break;
            }
        }
    }

    private void CheckActivation()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SelectOnPointerCard();
            if (!CardReader.OnPointerCard || !CardReader.OnPointerCard.CanUseThisCard) return;

            _selectIDX = CardReader.GetIdx(CardReader.OnPointerCard);
            CardReader.CaptureHand();

            CardReader.OnBinding = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            CardReader.OnBinding = false;
            Activation();
        }
    }

    private void Activation()
    {
        if (!IsPointerOnCard() || !CardReader.OnPointerCard.CanUseThisCard) return;

        if (IsMouseInWaitZone())
        {
            if(!CostCalculator.CanUseCost(CardReader.OnPointerCard.AbilityCost, CardReader.OnPointerCard.CardInfo.CardType == CardType.SKILL))
            {
                CardReader.InGameError.ErrorSituation("코스트가 부족합니다!");

                foreach(CardBase cb in CardReader.captureHandList)
                {
                    Debug.Log(cb.CardInfo.CardName);
                }
                CardReader.ResetByCaptureHand();
                CardReader.OnPointerCard.SetUpCard(CardReader.GetHandPos(CardReader.OnPointerCard), true);
                return;
            }
            
            CostCalculator.UseCost(CardReader.OnPointerCard.AbilityCost, CardReader.OnPointerCard.CardInfo.CardType == CardType.SKILL);

            if (CardReader.OnPointerCard.CardInfo.CardType == CardType.SKILL)
            {
                CardReader.SkillCardManagement.SetSkillCardInCardZone(CardReader.OnPointerCard);
            }
            else
            {
                CardReader.SpellCardManagement.UseAbility(CardReader.OnPointerCard);
            }
        }
        else //셔플
        {
            if(CardReader.GetIdx(CardReader.OnPointerCard) == _selectIDX
            || CardReader.OnPointerCard == CardReader.ShufflingCard)
            {
                CardReader.OnPointerCard.SetUpCard(CardReader.GetHandPos(CardReader.OnPointerCard), true);
                return;
            }

            if(!CostCalculator.CanUseCost(1, true))
            {
                CardReader.ShuffleInHandCard(CardReader.OnPointerCard, CardReader.ShufflingCard);
                CardReader.InGameError.ErrorSituation("코스트가 부족합니다!");
                CardReader.OnPointerCard.SetUpCard(CardReader.GetHandPos(CardReader.OnPointerCard), true);
                return;
            }

            CostCalculator.UseCost(1, true);
            CardReader.OnPointerCard.SetUpCard(CardReader.GetHandPos(CardReader.OnPointerCard), true);
        }
    }

    private bool IsPointerOnCard()
    {
        return CardReader.OnPointerCard != null;
    }
}
