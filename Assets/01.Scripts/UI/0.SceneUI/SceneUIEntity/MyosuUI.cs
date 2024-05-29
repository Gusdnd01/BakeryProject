using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyosuUI : SceneUI
{
    [SerializeField] private CrossRoadsAppearText _appearText;

    public void ApearText()
    {
        _appearText.Show();
    }

    public void HideText()
    {
        _appearText.Hide();
    }

    public void GoToBattleScene()
    {
        GameManager.Instance.ChangeScene(SceneType.battle);
    }
}
