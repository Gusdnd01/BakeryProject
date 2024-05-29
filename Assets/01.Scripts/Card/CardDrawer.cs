using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDrawer : MonoBehaviour
{
    [SerializeField] private Transform _cardSpawnTrm;
    [SerializeField] private Transform _cardParent;
    private Queue<CardBase> _toDrawCatalog = new Queue<CardBase>();

    private bool canDraw;
    public bool CanDraw
    {
        get { return canDraw; }
        set 
        { 
            canDraw = value;
            if(canDraw && _toDrawCatalog.Count != 0)
            {
                DrawCardLogic(_toDrawCatalog.Dequeue());
            }
        }
    }
    private BattleController _battleController;
    public BattleController BattleController
    {
        get
        {
            if (_battleController != null) return _battleController;
            _battleController = FindObjectOfType<BattleController>();
            return _battleController;
        }
    }
    int idx;
#if UNITY_EDITOR
    public void TestDraw(CardBase card)
    {
        _toDrawCatalog.Enqueue(card);
        DrawCardLogic(_toDrawCatalog.Dequeue());
    }
#endif
    public void DrawCard(int count, bool isRandom = true)
    {
        CanDraw = false;

        if(isRandom)
        {
            for (int i = 0; i < count; i++)
            {
                CardBase selectInfo = CardReader.GetRandomCardInDeck();

                _toDrawCatalog.Enqueue(selectInfo);
            }
        }
        else
        {
            for (int i = 0; i < count; i++)
            {
                CardBase selectInfo = CardReader.GetCardInDeck();

                _toDrawCatalog.Enqueue(selectInfo);
            }
        }

        DrawCardLogic(_toDrawCatalog.Dequeue());
    }

    private void DrawCardLogic(CardBase selectInfo)
    {
        CardBase spawnCard = Instantiate(selectInfo, _cardParent);

        CardReader.CardProductionMaster.OnCardIdling(spawnCard);

        spawnCard.OnPointerSetCardAction += CardReader.CardProductionMaster.OnSelectCard;
        spawnCard.OnPointerInitCardAction += CardReader.CardProductionMaster.QuitSelectCard;

        spawnCard.name = idx.ToString();
        spawnCard.battleController = this.BattleController;
        idx++;

        CardReader.AddCardInHand(spawnCard);
        
        spawnCard.transform.position = _cardSpawnTrm.position;

        spawnCard.SetUpCard(CardReader.GetPosOnTopDrawCard(), true);
    }

}
