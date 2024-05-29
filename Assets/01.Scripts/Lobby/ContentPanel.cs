using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ContentPanel : MonoBehaviour
{
    private GameObject _selectPanel;
    [SerializeField] private Image _blackPanel;

    public void ActiveContentPanel(GameObject selectPanel)
    {
        _selectPanel = selectPanel;
        transform.localScale = new Vector3(0, 1, 1);
        selectPanel.SetActive(true);
        transform.DOScale(new Vector3(1, 1, 1), 0.3f).SetEase(Ease.OutBack);
        _blackPanel.DOFade(0.8f, 0.3f);
    }

    public void UnActiveContentPanel()
    {
        _blackPanel.DOFade(0f, 0.3f);
        transform.DOScale(new Vector3(0, 1, 1), 0.3f).SetEase(Ease.InBack).
                  OnComplete(()=>
                  {
                      _selectPanel.SetActive(false);
                      Debug.Log(gameObject.transform.parent.gameObject);
                      gameObject.transform.parent.gameObject.SetActive(false);
                  });
    }
}
