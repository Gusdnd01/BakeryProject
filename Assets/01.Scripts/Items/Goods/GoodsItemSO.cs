using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GoodsType
{
    LoadStone = 0
}

[CreateAssetMenu(menuName = "SO/Items/GoodsItem")]
public class GoodsItemSO : ItemDataSO
{
    public GoodsType goodsType;
}
