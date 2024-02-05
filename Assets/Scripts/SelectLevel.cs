using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SelectLevel : MonoBehaviour
{
    [SerializeField] private int _level;
    [SerializeField] private Button _buttonLevel;
    [SerializeField] private TMP_Text _ButtontTextLevel;

    private void Start()
    {
        _ButtontTextLevel.text = _level.ToString();
        _buttonLevel.onClick.AddListener(LoadLevel);
    }

    private void LoadLevel()
    {
        UIManager.Instance.HideLevelPopup();
        UIManager.Instance.SpreadClouds();
        SceneLoader.Instance.LoadSceneName("Day " + _level);
    }
}
