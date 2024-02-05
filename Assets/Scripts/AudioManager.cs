using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [Header("Sounds")]
    [SerializeField] private AudioClip _gameMusic;

    [Header("CarsSFX")]
    [SerializeField] private AudioClip _startEngineSFX;
    [SerializeField] private AudioClip _engineLoopSFX;
    [SerializeField] private AudioClip _accidentCarSFX;
    [SerializeField] private AudioClip _accidentWalkerSFX;

    [Header("MenuFX")]
    [SerializeField] private AudioClip _buttonSFX;
    [SerializeField] private AudioClip _sliderSFX;

    [Header("Slider Controls")]
    [SerializeField] private Slider _soundsSlider;
    [SerializeField] private Slider _musicSlider;

    [Header("AudioSource")]
    private AudioSource _musicSource;
    private AudioSource _soundsSource;

    private bool changingMusicVolume = false;
    private bool changingSoundsVolume = false;

    public static AudioManager Instance;

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

        _musicSource = gameObject.AddComponent<AudioSource>();
        _soundsSource = gameObject.AddComponent<AudioSource>();
    }

    private void Start()
    {
        _musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        _soundsSlider.value = PlayerPrefs.GetFloat("SoundsVolume", 0.5f);

        _musicSlider.onValueChanged.AddListener(ChangeMusicVolume);
        _soundsSlider.onValueChanged.AddListener(ChangeSoundsVolume);

        ChangeMusicVolume(_musicSlider.value);
        ChangeSoundsVolume(_soundsSlider.value);
    }

    private void ChangeMusicVolume(float volume)
    {
        if (!changingMusicVolume)
        {
            changingMusicVolume = true;

            PlayerPrefs.SetFloat("MusicVolume", volume);
            PlayerPrefs.Save();

            _musicSource.volume = volume;

            if (volume > 0)
            {
                _musicSource.clip = _gameMusic;
                _musicSource.loop = true;
                _musicSource.Play();
            }
            else
            {
                _musicSource.Stop();
            }

            changingMusicVolume = false;
        }
    }

    private void ChangeSoundsVolume(float volume)
    {
        if (!changingSoundsVolume)
        {
            changingSoundsVolume = true;

            PlayerPrefs.SetFloat("SoundsVolume", volume);
            PlayerPrefs.Save();

            _soundsSource.volume = volume;

            changingSoundsVolume = false;

        }
    }

    public AudioClip GetStartEngineSFX()
    {
        return _startEngineSFX;
    }

    public AudioClip GetEngineLoopSFX()
    {
        return _engineLoopSFX;
    }

    public AudioClip GetAccidentCarSFX()
    {
        return _accidentCarSFX;
    }

    public void PlaySFX(AudioClip clip, AudioSource source)
    {
        if (_soundsSlider.value > 0)
        {
            source.clip = clip;
            source.loop = false;

            source.Play();
        }
    }

    public void PlayLoopSFX(AudioClip clip, AudioSource source)
    {
        if (_soundsSlider.value > 0)
        {
            source.clip = clip;
            source.loop = true;

            source.Play();
        }
    }

    public void StopSFX(AudioSource source)
    {
        source.Stop();
    }

    public AudioClip GetButtonSFX()
    {
        return _buttonSFX;
    }

    public void PlayButtonSFX(AudioClip clip)
    {
        if (_soundsSlider.value > 0)
        {
            _soundsSource.clip = clip;
            _soundsSource.loop = false;

            _soundsSource.Play();
        }
    }
}