using System.Collections;
using DG.Tweening;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private CanvasGroup _gamePanel;
    [SerializeField] private CanvasGroup _homeMenuPanel;
    [SerializeField] private CanvasGroup _popupPanel;
    [SerializeField] private CanvasGroup _cloudPanel;

    [Header("Popups")]
    [SerializeField] private GameObject _accidentCarPopup;
    [SerializeField] private GameObject _accidentWalkerPopup;
    [SerializeField] private GameObject _completionPopup;
    [SerializeField] private GameObject _menuPopup;
    [SerializeField] private GameObject _levelPopup;
    private float _fadeTime = 0.25f;
    private float _durationTime = 0.5f;
    private float _initialScale = 0f;

    [Header("Clouds")]
    [SerializeField] private RectTransform _leftCloud;
    [SerializeField] private RectTransform _rightCloud;
    private float _moveSpeedCloud = 5f;

    private Vector2 _initialLeftCloudPosition;
    private Vector2 _initialRightCloudPosition;

    private Vector2 _convergePosition = new Vector2(880f, 0f);
    private Vector2 _spreadPosition = new Vector2(-1800f, 0f);

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

        RecordInitialPositions();
        RepositionClouds();
    }

    public void ShowHomePanel()
    {
        _homeMenuPanel.alpha = 1f;
        _homeMenuPanel.blocksRaycasts = true;
        _gamePanel.alpha = 0f;
        _gamePanel.blocksRaycasts = false;
    }

    public void ShowGamePanel()
    {
        _gamePanel.alpha = 1f;
        _gamePanel.blocksRaycasts = true;
        _homeMenuPanel.alpha = 0f;
        _homeMenuPanel.blocksRaycasts = false;
    }

    private void SetPopupActive(GameObject popup, bool isActive)
    {
        if (_isPopupAnimating)
            return;

        StartCoroutine(AnimatePopup(popup, isActive));
    }

    private IEnumerator AnimatePopup(GameObject popup, bool isActive)
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
        _cloudPanel.interactable = false;
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
        _cloudPanel.interactable = false;
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

    private void SetPopupInfoActive(GameObject popup, bool isActive)
    {
        if (_isPopupAnimating)
            return;

        StartCoroutine(AnimateInfoPopup(popup, isActive));
    }

    private IEnumerator AnimateInfoPopup(GameObject popup, bool isActive)
    {
        _isPopupAnimating = true;

        if (isActive)
        {
            ShowInfoPopup(popup);
        }
        else
        {
            HideInfoPopup(popup);
        }

        yield return new WaitForSeconds(_durationTime);

        _isPopupAnimating = false;
    }

    private void ShowInfoPopup(GameObject popup)
    {
        _popupPanel.alpha = 0f;
        _gamePanel.interactable = false;
        _homeMenuPanel.interactable = false;
        _cloudPanel.interactable = false;
        _isPopupOpen = true;

        RectTransform popupRect = popup.GetComponent<RectTransform>();
        popupRect.anchoredPosition = Vector2.zero;
        popup.SetActive(true);

        _popupPanel.DOFade(1, _fadeTime);
        popupRect.localScale = Vector3.one * _initialScale;
        popupRect.DOScale(Vector3.one, _durationTime).SetEase(Ease.InQuart);
    }

    private void HideInfoPopup(GameObject popup)
    {
        _popupPanel.alpha = 1f;
        _gamePanel.interactable = true;
        _homeMenuPanel.interactable = true;
        _cloudPanel.interactable = false;
        _isPopupOpen = false;

        RectTransform popupRect = popup.GetComponent<RectTransform>();
        popupRect.DOScale(Vector3.one * _initialScale, _durationTime)
            .SetEase(Ease.InOutQuart)
            .OnComplete(() =>
            {
                popup.SetActive(false);
                _popupPanel.DOFade(0, _fadeTime);
            });
    }

    public void ShowCompletionPopup()
    {
        SetPopupInfoActive(_completionPopup, true);
    }

    public void HideCompletionPopup()
    {
        SetPopupInfoActive(_completionPopup, false);

        _sceneLoader.LoadNextLevel();
    }

    public void ShowAccidentCarPopup()
    {
        SetPopupInfoActive(_accidentCarPopup, true);
    }

    public void HideAccidentCarPopup()
    {
        SetPopupInfoActive(_accidentCarPopup, false);

        _sceneLoader.ReloadGame();
    }

    public void ShowAccidentWalkerPopup()
    {
        SetPopupInfoActive(_accidentWalkerPopup, true);
    }

    public void HideAccidentWalkerPopup()
    {
        SetPopupInfoActive(_accidentWalkerPopup, false);

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

    public void RecordInitialPositions()
    {
        _cloudPanel.alpha = 1f;
        _initialLeftCloudPosition = _leftCloud.anchoredPosition;
        _initialRightCloudPosition = _rightCloud.anchoredPosition;
    }

    public void ShiftClouds()
    {
        _cloudPanel.alpha = 1f;
        _leftCloud.DOAnchorPos(_initialLeftCloudPosition + new Vector2(_moveSpeedCloud, 0f), 1f);
        _rightCloud.DOAnchorPos(_initialRightCloudPosition - new Vector2(_moveSpeedCloud, 0f), 1f);
    }

    public void SpreadClouds()
    {
        _cloudPanel.alpha = 1f;
        _leftCloud.DOAnchorPos(_spreadPosition, 1f);
        _rightCloud.DOAnchorPos(_spreadPosition * -1, 1f);
    }

    public void RepositionClouds()
    {
        _leftCloud.anchoredPosition = _convergePosition;
        _rightCloud.anchoredPosition = _convergePosition * -1;
    }

    public void ExpandAndCollapseClouds()
    {
        StartCoroutine(ExpandAndCollapseCoroutine());
    }

    private IEnumerator ExpandAndCollapseCoroutine()
    {
        ShiftClouds();

        yield return new WaitForSeconds(1f);

        SpreadClouds();
    }
}
