using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardShameElement : MonoBehaviour
{
    [SerializeField] private Image _shameIconImage;
    [SerializeField] private TextMeshProUGUI _shameTypeText;
    [SerializeField] private TextMeshProUGUI _shameText;

    public void SetShame((Sprite, string, int, int, string) shameGroup)
    {
        _shameIconImage.sprite = shameGroup.Item1;
        _shameTypeText.text = shameGroup.Item2;

        if(shameGroup.Item5 == string.Empty)
        {
            string comparison = $"{shameGroup.Item4} -> {shameGroup.Item3}";

            int difference = shameGroup.Item3 - shameGroup.Item4;
            string richTexting;

            if(difference > 0)
            {
                richTexting = $"<size=25><color=#37B1FF>  (+{difference})</size></color>";
            }
            else if(difference < 0)
            {
                richTexting = $"<size=25><color=#FF6464>  ({difference})</size></color>";
            }
            else
            {
                richTexting = $"<size=25>  (+{difference})</size>";
            }

            _shameText.text = comparison + richTexting ;
        }
        else
        {
            _shameText.text = shameGroup.Item5;
        }
    }
}
