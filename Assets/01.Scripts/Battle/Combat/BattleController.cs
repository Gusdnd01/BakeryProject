using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

[Serializable]
public class SEList<T>
{
    public List<T> list;
}

public class BattleController : MonoSingleton<BattleController>
{
    [SerializeField] private BattleCutSlider _battleCutter;
    public BattleCutSlider BattleCutSlider => _battleCutter;

    [SerializeField] private SEList<SEList<bool>> isStuck;

    public Enemy[] onFieldMonsterList;
    public List<Enemy> DeathEnemyList { get; private set; } = new List<Enemy>();
    public List<Enemy> SpawnEnemyList { get; private set; } = new List<Enemy>();

    [HideInInspector] private HpBarMaker _hpBarMaker;

    [Header("���� ��")]
    [SerializeField] [Range(0.01f, 0.1f)] private float _spawnTurm;

    [SerializeField] private EnemyGroupSO _enemyGroup;

    [SerializeField]private List<Transform> enemySpawnTrm = new();
    [HideInInspector] public List<Vector3> enemySpawnPos = new();
    [SerializeField]private Transform enemyGroupCenter;
    [HideInInspector] public Vector3 enemyGroupPos;
    private Queue<PoolingType> _enemyQue = new Queue<PoolingType>();

    [SerializeField] private Player _player;
    public Player Player
    {
        get
        {
            if (_player != null) return _player;
            _player = FindObjectOfType<Player>();
            return _player;
        }
    }
    private bool _isGameEnd;
    public bool IsGameEnd
    {
        get => _isGameEnd;
        set
        {
            _isGameEnd = value;
            if (_isGameEnd)
            {
                for (int i = 0; i < onFieldMonsterList.Length; i++)
                {
                    Enemy e = onFieldMonsterList[i];
                    if (e == null) continue;

                    onFieldMonsterList[i] = null;
                    e.turnStatus = TurnStatus.End;
                    e.GotoPool();
                    //PoolManager.Instance.Push(e);
                }

                OnGameEndEvent?.Invoke();
                CostCalculator.Init();
                //SelectPlayerTarget(null, null);

                //UIManager.Instance.GetSceneUI<BattleUI>().SystemActive?.Invoke(true);
                _hpBarMaker.DeleteAllHPBar();
                StopAllCoroutines();
            }
        }
    }

    [SerializeField] private UnityEvent OnGameEndEvent;
    [SerializeField] private UnityEvent<Enemy, Vector2> _maskCreateEvent;
    public UnityEvent<Enemy> maskEnableEvent;
    public UnityEvent<Enemy> maskDisableEvent;
    public CameraController CameraController { get; private set; }

    private void Start()
    {
        enemySpawnPos.Clear();

        foreach (var p in enemySpawnTrm)
        {
            enemySpawnPos.Add(p.position);
        }
        enemyGroupPos = enemyGroupCenter.position;

        _hpBarMaker = FindObjectOfType<HpBarMaker>();

        CameraController = FindObjectOfType<CameraController>();
        CameraController.BattleController = this;

        onFieldMonsterList = new Enemy[enemySpawnPos.Count];

        CardReader.SkillCardManagement.useCardEndEvnet.AddListener(HandleEndSkill);

        TurnCounter.PlayerTurnStartEvent += HandleCardDraw;
        TurnCounter.EnemyTurnStartEvent += OnEnemyTurnStart;
        TurnCounter.EnemyTurnEndEvent += OnEnemyTurnEnd;

        Player.BattleController = this;
        _hpBarMaker.SetupHpBar(Player);
        Player.HealthCompo.OnDeathEvent.AddListener(() => IsGameEnd = true);
    }
    private void HandleCardDraw(bool obj)
    {
        CardReader.CardDrawer.DrawCard(3, false);
    }
    private void HandleEndSkill()
    {
        foreach (var e in onFieldMonsterList)
        {
            if (e != null)
            {
                Health h = e.HealthCompo;
                if (h.GetNormalizedHealth() <= 0)
                {
                    h.IsDead = true;
                    e.DeadSequence();
                }
            }
        }
        Health health = Player.HealthCompo;
        if (health.GetNormalizedHealth() <= 0)
        {
            health.IsDead = true;
            health.OnDeathEvent?.Invoke();
        }
    }
    private void OnDestroy()
    {
        CardReader.SkillCardManagement.useCardEndEvnet.RemoveListener(HandleEndSkill);
        TurnCounter.EnemyTurnStartEvent -= OnEnemyTurnStart;
        TurnCounter.EnemyTurnEndEvent -= OnEnemyTurnEnd;
        TurnCounter.PlayerTurnStartEvent -= HandleCardDraw;
    }
    private void OnEnemyTurnStart(bool value)
    {
        StartCoroutine(EnemySquence());
    }
    private void OnEnemyTurnEnd()
    {
        foreach (var e in onFieldMonsterList)
        {
            if (e is null) continue;

            e.TurnEnd();
            maskDisableEvent?.Invoke(e);
        }
    }
    private IEnumerator EnemySquence()
    {
        foreach (var e in onFieldMonsterList)
        {
            if (e is null) continue;
            Player.VFXManager.SetBackgroundColor(Color.gray);

            e.TurnAction();
            yield return new WaitUntil(() => e.turnStatus == TurnStatus.End);

            Player.VFXManager.SetBackgroundColor(Color.white);
            if (_isGameEnd)
                break;

            yield return new WaitForSeconds(1.5f);
        }

        if (!_isGameEnd)
        {
            TurnCounter.ChangeTurn();
        }
    }
    public void SetStage()
    {
        Debug.Log(MapManager.Instanace.SelectStageData.enemyGroup);
        _enemyGroup = MapManager.Instanace.SelectStageData.enemyGroup;

        foreach (var e in _enemyGroup.enemies)
        {
            _enemyQue.Enqueue(e.poolingType);
        }

        for (int i = 0; i < enemySpawnPos.Count; i++)
        {
            SpawnMonster(i);
        }
    }
    private void SpawnMonster(int idx)
    {
        if (_enemyQue.Count > 0)
        {
            Vector3 pos = enemySpawnPos[idx];
            print(pos);
            Enemy selectEnemy = PoolManager.Instance.Pop(_enemyQue.Dequeue()) as Enemy;
            selectEnemy.transform.position = pos;
            selectEnemy.BattleController = this;
            int posChecker = ((idx + 3) % 2) * 2;
            selectEnemy.Spawn(pos);
            _maskCreateEvent?.Invoke(selectEnemy, pos);
            selectEnemy.SpriteRendererCompo.sortingOrder = posChecker;

            selectEnemy.HealthCompo.OnDeathEvent.AddListener(() => DeadMonster(selectEnemy));

            onFieldMonsterList[idx] = selectEnemy;
            selectEnemy.target = Player;

            selectEnemy.OnAttackStart += _battleCutter.Cutting;
            selectEnemy.OnAttackEnd += _battleCutter.Reverting;

            SpawnEnemyList.Add(selectEnemy);
            _hpBarMaker.SetupHpBar(selectEnemy);
        }
    }
    public void DeadMonster(Enemy enemy)
    {
        onFieldMonsterList[Array.IndexOf(onFieldMonsterList, enemy)] = null;

        DeathEnemyList.Add(enemy);
        maskDisableEvent?.Invoke(enemy);
    }
    public bool IsStuck(int to, int who)
    {
        return isStuck.list[to].list[who];
    }
    public void ChangePosition(Transform e1, Transform e2, Action callback = null)
    {
        e1.DOMove(e2.position, 0.5f);
        e2.DOMove(e1.position, 0.5f).OnComplete(() => callback?.Invoke());
    }
    public void ChangeXPosition(Transform e1, Transform e2, Action callback = null)
    {
        e1.DOMoveX(e2.position.x, 0.5f);
        e2.DOMoveX(e1.position.x, 0.5f).OnComplete(() => callback?.Invoke());
    }
    public void SelectPlayerTarget(CardBase cardBase, Entity entity)
    {
        Player.SaveSkillToEnemy(cardBase, entity);
    }
    public void BackgroundColor(Color color)
    {
        Player.VFXManager.SetBackgroundColor(color);
    }
}
