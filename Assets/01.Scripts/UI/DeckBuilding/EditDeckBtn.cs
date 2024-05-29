  using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditDeckBtn : MonoBehaviour
{
    public void PressEditDeck()
    {
        GameManager.Instance.ChangeScene(SceneType.deckBuild);
    }
}
