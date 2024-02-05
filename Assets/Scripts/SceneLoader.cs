using System.Collections;
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

        if (_nextSceneIndex == 0)
        {
            _nextSceneIndex = 1;
        }

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

        if (_currentSceneIndex > 10)
        {
            LoadMainMenu();

            _uiManager.ShiftClouds();
        }
    }

    public void LoadSceneName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadNewGame()
    {
        LoadSceneName("Day 1");

        _uiManager.SpreadClouds();
    }

    public void LoadNextLevel()
    {
        if (_currentSceneIndex > 10)
        {
            LoadMainMenu();
        }
        else
        {
            NextLevelClouds();
        }
    }

    public void NextLevelClouds()
    {
        StartCoroutine(NextLevelCloudsCoroutine());
    }

    private IEnumerator NextLevelCloudsCoroutine()
    {
        _uiManager.ShiftClouds();

        UpdateScenceInfo();

        _nextSceneName = "Day " + _nextSceneIndex;

        yield return new WaitForSeconds(1f);

        if (_currentSceneName != "MainMenu")
        {
            _textSceneName.text = _nextSceneName;
        }

        LoadSceneName(_nextSceneName);

        _uiManager.SpreadClouds();
    }

    public void LoadMainMenu()
    {
        MainMenuClouds();
    }

    public void MainMenuClouds()
    {
        StartCoroutine(MainMenuCloudsCoroutine());
    }

    private IEnumerator MainMenuCloudsCoroutine()
    {
        _uiManager.ShiftClouds();

        yield return new WaitForSeconds(1f);

         _currentSceneIndex = 0;
        _nextSceneIndex = 1;

        LoadSceneName("MainMenu");
    }

    public void ReloadGame()
    {
        LoadSceneName(_currentSceneName);
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