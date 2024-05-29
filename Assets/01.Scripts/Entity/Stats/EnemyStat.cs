using UnityEngine;

[CreateAssetMenu(menuName = "SO/Stat/Enemy")]
public class EnemyStat : CharacterStat
{
    [Header("DropItem")]
    [SerializeField] private ItemDataIngredientSO _dropItem;
    public ItemDataIngredientSO DropItem => _dropItem;

    [Header("Level detail")]
    [SerializeField] private int _level;

    [Range(0, 1f)]
    [SerializeField] private float _percentageModifier;

    public int attackCnt = 1;

    public void Modify(Stat stat)
    {
        for (int i = 1; i < _level; i++)
        {
            //레벨당 증가하게 됨.
            float modifier = stat.GetValue() * _percentageModifier;
            stat.AddModifier(Mathf.RoundToInt(modifier));
        }
    }
}