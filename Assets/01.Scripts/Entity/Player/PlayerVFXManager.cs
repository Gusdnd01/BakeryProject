using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Particle;

[Serializable]
public struct PlayerVFXData
{
    public CardInfo info;
    public ParticleSystem[] particle;
    public ParticlePoolObject poolObject;
}

public class PlayerVFXManager : MonoBehaviour
{
    [SerializeField] private List<PlayerVFXData> cardAndEffects = new();
    private Dictionary<CardInfo, ParticleSystem[]> _cardByEffects = new();
    private Dictionary<CardInfo, ParticlePoolObject> _cardByEffects2 = new();
    //���ݽ� ����Ʈ ������ ����

    public Action OnEffectEvent;
    public Action OnEndEffectEvent;
    //public Action OnEffectEvent;
    [SerializeField] private Player p;
    [SerializeField] private SpriteRenderer[] backgrounds;
    private SpriteRenderer currentBackground;

    private void Awake()
    {
        foreach (var c in cardAndEffects)
        {
            if (!_cardByEffects.ContainsKey(c.info))
            {
                _cardByEffects.Add(c.info, c.particle);
                _cardByEffects2.Add(c.info, c.poolObject);
            }
            else
            {
                Debug.LogError("�ߺ��� �־��");
            }
        }

    }

    private void Start()
    {
        foreach (var b in backgrounds)
        {
            if (b.gameObject.activeSelf == true)
            {
                currentBackground = b;
            }
        }
    }

    internal void EndParticle(CardInfo cardInfo, int combineLevel)
    {
        if (!_cardByEffects.ContainsKey(cardInfo))
        {
            Debug.LogError("����Ʈ�� �����");
            return;
        }
        _cardByEffects[cardInfo][combineLevel].Stop();
    }

    public void PlayParticle(CardInfo card, Vector3 pos, int combineLevel)
    {
        if (!_cardByEffects.ContainsKey(card))
        {
            Debug.LogError("����Ʈ�� �����");
            return;
        }
        _cardByEffects[card][combineLevel].transform.position = pos;
        _cardByEffects[card][combineLevel].gameObject.SetActive(true);
        currentBackground.DOColor(Color.gray, 1.0f);
        ParticleSystem.MainModule mainModule = _cardByEffects[card][combineLevel].main;
        StartCoroutine(EndEffectCo(mainModule.startLifetime.constantMax / mainModule.simulationSpeed));
        _cardByEffects[card][combineLevel].Play();
    }
    public void PlayParticle(CardBase card, Vector3 pos)
    {
        int level = (int)card.CombineLevel;
        ParticlePoolObject obj = PoolManager.Instance.Pop(_cardByEffects2[card.CardInfo].poolingType) as ParticlePoolObject;
        obj.transform.position = pos;
        obj[level].owner = p;
        obj[level].damages = card.GetDamage(card.CombineLevel);
        foreach (var t in p.GetSkillTargetEnemyList[card])
        {
            obj[level].AddTriggerTarget(t);
        }
        obj.Active(level, OnEffectEvent, OnEndEffectEvent);
    }
    public void PlayParticle(CardBase card, Vector3 pos, out ParticlePoolObject particle)
    {
        int level = (int)card.CombineLevel;
        ParticlePoolObject obj = PoolManager.Instance.Pop(_cardByEffects2[card.CardInfo].poolingType) as ParticlePoolObject;
        obj.transform.position = pos;
        obj[level].owner = p;
        obj[level].damages = card.GetDamage(card.CombineLevel);
        foreach (var t in p.GetSkillTargetEnemyList[card])
        {
            obj[level].AddTriggerTarget(t);
        }
        obj.Active(level, null, OnEndEffectEvent);
        particle = obj;
    }

    public void PlayParticle(CardInfo card, int combineLevel)
    {
        if (!_cardByEffects.ContainsKey(card))
        {
            Debug.LogError("����Ʈ�� �����");
            return;
        }

        _cardByEffects[card][combineLevel].gameObject.SetActive(true);
        currentBackground.DOColor(Color.gray, 1.0f);
        ParticleSystem.MainModule mainModule = _cardByEffects[card][combineLevel].main;
        StartCoroutine(EndEffectCo(mainModule.startLifetime.constantMax / mainModule.simulationSpeed));
        _cardByEffects[card][combineLevel].Play();
    }

    private IEnumerator EndEffectCo(float f)
    {
        yield return new WaitForSeconds(f);
        currentBackground.DOColor(Color.white, 1.0f);
        OnEndEffectEvent?.Invoke();
    }

    public void SetBackgroundColor(Color color)
    {
        currentBackground.DOColor(color, 0.5f);
    }
}
