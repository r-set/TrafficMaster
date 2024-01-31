using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Header("Cars List")]
    public List<GameObject> sceneCarsList = new List<GameObject>();

    [Header("Scenes")]
    [SerializeField] TMP_Text _textSceneName;

    private string _currentSceneName;
    private string _nextSceneName;
    private int _currentSceneIndex;
    private int _nextSceneIndex;

    private AudioClip _buttonUISFX;

    public static SceneLoader Instance;
    private UIManager _uiManager;
    private AudioManager _audioManager;

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

        _uiManager = UIManager.Instance;
        _audioManager = AudioManager.Instance;

        _buttonUISFX = _audioManager.GetButtonSFX();
    }

    private void Start()
    {
        UpdateScenceInfo();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void UpdateScenceInfo()
    {
        _currentSceneName = SceneManager.GetActiveScene().name;
        _currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        _nextSceneIndex = (_currentSceneIndex + 1) % SceneManager.sceneCountInBuildSettings;

        if (_currentSceneName != "MainMenu")
        {
            _textSceneName.text = _currentSceneName;

            _uiManager.ShowGamePanel();
        }
        else
        {
            _uiManager.ShowHomePanel();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateScenceInfo();

        if (scene.name == "Day " + (SceneManager.sceneCountInBuildSettings))
        {
            LoadMainMenu();
        }
    }

    public void LoadSceneWithButtonSFX(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        _audioManager.PlayButtonSFX(_buttonUISFX);
    }

    public void LoadNewGame()
    {
        LoadSceneWithButtonSFX("Day 1");
    }

    public void LoadNextLevel()
    {
        UpdateScenceInfo();

        _nextSceneName = "Day " + _nextSceneIndex;

        if (_currentSceneName != "MainMenu")
        {
            _textSceneName.text = _nextSceneName;
        }

        LoadSceneWithButtonSFX(_nextSceneName);
    }

    public void LoadMainMenu()
    {
        LoadSceneWithButtonSFX("MainMenu");
    }

    public void ReloadGame()
    {
        LoadSceneWithButtonSFX(_currentSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void RemoveCarFromList(GameObject car)
    {
        if (car != null && sceneCarsList != null)
        {
            sceneCarsList.Remove(car);
        }

        if (sceneCarsList == null || sceneCarsList.Count <= 0)
        {
            _uiManager.ShowCompletionPopup();
        }
    }
}