using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleEnemyPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _stageName;
    [SerializeField] private TextMeshProUGUI _enemiesName;
    [SerializeField] private Image[] _enemyImgArr;

    public void SetBattleEnemy(EnemyGroupSO enemyGroup, string stageName)
    {
        _stageName.text = stageName;
        StringBuilder sb = new StringBuilder();
        sb.Append("(");
        for(int i = 0; i < enemyGroup.enemies.Count; i++)
        {
            _enemyImgArr[i].sprite = enemyGroup.enemies[i].CharStat.characterVisual;
            _enemyImgArr[i].enabled = true;
            sb.Append($"{enemyGroup.enemies[i].CharStat.characterName} ,");
        }
        sb.Append(")");

        _enemiesName.text = sb.ToString();
    }
}
