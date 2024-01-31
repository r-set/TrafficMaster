using DG.Tweening;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private CanvasGroup _gamePanel;
    [SerializeField] private CanvasGroup _homeMenuPanel;
    [SerializeField] private CanvasGroup _popupPanel;
    [SerializeField] private float _fadeTime = 0.5f;
    [SerializeField] private float _durationTime = 1f;

    [Header("Popups")]
    [SerializeField] private GameObject _accidentCarPopup;
    [SerializeField] private GameObject _accidentWalkerPopup;
    [SerializeField] private GameObject _completionPopup;
    [SerializeField] private GameObject _menuPopup;
    [SerializeField] private GameObject _levelPopup;

    private readonly Vector2 _hiddenPosition = new Vector3(0f, 1000f, 0f);
    private readonly Vector2 _shownPosition = new Vector3(0f, 0f, 0f);

    private bool _isPopupAnimating = false;
    [HideInInspector] public bool _isPopupOpen = false;

    public static UIManager Instance;
    private SceneLoader _sceneLoader;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        _sceneLoader = SceneLoader.Instance;
    }

    public void ShowHomePanel()
    {
        _homeMenuPanel.alpha = 1f;
        _gamePanel.alpha = 0f;
    }

    public void ShowGamePanel()
    {
        _gamePanel.alpha = 1f;
        _homeMenuPanel.alpha = 0f;
    }

    private void SetPopupActive(GameObject popup, bool isActive)
    {
        if (_isPopupAnimating)
            return;

        StartCoroutine(AnimatePopup(popup, isActive));
    }

    private System.Collections.IEnumerator AnimatePopup(GameObject popup, bool isActive)
    {
        _isPopupAnimating = true;

        if (isActive)
        {
            ShowPopup(popup);
        }
        else
        {
            HidePopup(popup);
        }

        yield return new WaitForSeconds(_durationTime);

        _isPopupAnimating = false;
    }

    private void ShowPopup(GameObject popup)
    {
        _popupPanel.alpha = 0f;
        _gamePanel.interactable = false;
        _homeMenuPanel.interactable = false;
        _isPopupOpen = true;

        RectTransform popupRect = popup.GetComponent<RectTransform>();
        popupRect.anchoredPosition = _hiddenPosition;
        popup.SetActive(true);
        _popupPanel.DOFade(1, _fadeTime);

        popupRect.DOAnchorPos(_shownPosition, _durationTime).SetEase(Ease.InOutCirc);
    }

    private void HidePopup(GameObject popup)
    {
        _popupPanel.alpha = 1f;
        _gamePanel.interactable = true;
        _homeMenuPanel.interactable = true;
        _isPopupOpen = false;

        RectTransform popupRect = popup.GetComponent<RectTransform>();
        popupRect.DOAnchorPos(_hiddenPosition, _durationTime)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                popup.SetActive(false);
                _popupPanel.DOFade(0, _fadeTime);
            });
    }

    public void ShowCompletionPopup()
    {
        SetPopupActive(_completionPopup, true);
    }

    public void HideCompletionPopup()
    {
        SetPopupActive(_completionPopup, false);

        _sceneLoader.LoadNextLevel();
    }

    public void ShowAccidentCarPopup()
    {
        SetPopupActive(_accidentCarPopup, true);
    }

    public void HideAccidentCarPopup()
    {
        SetPopupActive(_accidentCarPopup, false);

        _sceneLoader.ReloadGame();
    }

    public void ShowAccidentWalkerPopup()
    {
        SetPopupActive(_accidentWalkerPopup, true);
    }

    public void HideAccidentWalkerPopup()
    {
        SetPopupActive(_accidentWalkerPopup, false);

        _sceneLoader.ReloadGame();
    }

    public void ShowMenuPopup()
    {
        SetPopupActive(_menuPopup, true);
    }

    public void HideMenuPopup()
    {
        SetPopupActive(_menuPopup, false);
    }

    public void ShowLevelPopup()
    {
        SetPopupActive(_levelPopup, true);
    }

    public void HideLevelPopup()
    {
        SetPopupActive(_levelPopup, false);
    }
}
