using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EpisodeDialogueDefine;
using TMPro;
using System.Text;
using DG.Tweening;
using UnityEngine.UI;

public class EpisodeDialogueDrawer : MonoBehaviour
{
    [SerializeField] private GameObject _nameBase;
    [SerializeField] private TextMeshProUGUI _nameTextTmp;
    private string _nameText;
    private string _nameTextPro
    {
        get
        {
            return _nameText;
        }
        set
        {
            _nameText = value;
            _nameTextTmp.text = _nameText;
        }
    }

    [SerializeField] private TextMeshProUGUI _syntexTextTmp;
    private string _syntexText;
    private StringBuilder _syntexBuilder = new StringBuilder();
    private int _idx;

    private float _typingTime = 0.05f;
    private float _currentTime;

    [SerializeField] private Image[] _backGround;
    [SerializeField] private List<Sprite> _backGroundSpriteList = new List<Sprite>();
    private BackGroundType _bgType = BackGroundType.Castle;

    private EpisodeManager _episodeManager;
    private SoundSelecter _episodeSounder;

    private void Awake()
    {
        _episodeManager = EpisodeManager.Instanace;
        _episodeSounder = transform.parent.Find("EpisodeSounder").GetComponent<SoundSelecter>();
    }

    private void FixedUpdate()
    {
        TypingText();
        SkipText();
    }

    private void SkipText()
    {
        if(_episodeManager.isTextInTyping && Input.GetMouseButtonDown(0))
        {
            _episodeManager.isTextInTyping = false;
            _syntexTextTmp.text = _syntexText;
            
        }
    }

    private void TypingText()
    {
        if (_episodeManager.isTextInTyping)
        {
            if (_typingTime >= _currentTime)
            {
                _syntexBuilder.Append(_syntexText[_idx]);
                _syntexTextTmp.text = _syntexBuilder.ToString();
                _episodeSounder.HandleOutputSFX(SFXType.texter);

                _currentTime = 0;
                _idx++;
                if (_idx >= _syntexText.Length) _episodeManager.isTextInTyping = false;
            }
            _currentTime += Time.deltaTime;
        }
    }

    private void ResetFunctions()
    {
        _syntexTextTmp.text = string.Empty;
        _syntexBuilder.Clear();
        _idx = 0;

        _episodeManager.isTextInTyping = true;
    }

    public void HandleStandardElementDraw(string name, string syntex, BackGroundType bgType)
    {
        _nameBase.SetActive(name != string.Empty);

        _nameTextPro = name;
        _syntexText = syntex;
        ResetFunctions();

        if (_bgType == bgType) return;
        UpdateBackGround(bgType);
    }

    private void UpdateBackGround(BackGroundType bgType)
    {
        if(bgType == BackGroundType.None)
        {
            _backGround[0].color = new Color(1, 1, 1, 0);
            _backGround[1].color = new Color(1, 1, 1, 0);
            return;
        }

        _backGround[0].sprite = _backGround[1].sprite;
        _backGround[1].color = new Color(1, 1, 1, 0);
        _backGround[1].sprite = _backGroundSpriteList[(int)bgType];
        _backGround[1].DOFade(1, 0.3f);
        _bgType = bgType;
    }
}
