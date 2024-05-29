using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/EnemyGroup")]
public class EnemyGroupSO : ScriptableObject
{
    public List<Entity> enemies = new List<Entity>();

}
