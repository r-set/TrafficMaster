using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CarManager : MonoBehaviour
{
    [Header("Car Settings")]
    private float _moveSpeed = 10f;
    private float _startEngine = 1.5f;
    private float _rotationSpeed = 720f;
    private float _minDistance = 0.1f;
    private bool _isMoving = false;

    [Header("Wheel Settings")]
    [SerializeField] private List<Transform> _wheels;
    private float _wheelRotationSpeed = 1080f;

    [Header("Scale Settings")]
    private float _sizeScale = 1.05f;
    private float _timeRepeatScale = 0.2f;
    private bool _isScaling = true;

    [Header("Route, Prefab , VFX")]
    [SerializeField] private LineRenderer _routeLine;
    [SerializeField] private GameObject _carPrefab;
    [SerializeField] private ParticleSystem _carVFX;

    [Header("Sounds")]
    private AudioClip _startEngineSFX;
    private AudioClip _engineLoopSFX;
    private AudioClip _accidentCarSFX;
    private AudioClip _accidentWalkerSFX;

    [Header("Route")]
    private Vector3[] _routePoints;
    private int _currentRouteIndex = 0;
    private bool _routeCompleted = false;
    private const int START_INDEX = 0;
    private const int NEXT_POINT_INDEX = 1;

    private Camera _camera;
    private bool _isAccident = false;

    private AudioManager _audioManager;
    private UIManager _uiManager;
    private SceneLoader _sceneLoader;

    private AudioSource _carSFXSource;

    private void Awake()
    {
        _camera = Camera.main;

        _carSFXSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        _sceneLoader = SceneLoader.Instance;
        _audioManager = AudioManager.Instance;
        _uiManager = UIManager.Instance;

        _startEngineSFX = _audioManager.GetStartEngineSFX();
        _engineLoopSFX = _audioManager.GetEngineLoopSFX();
        _accidentCarSFX = _audioManager.GetAccidentCarSFX();
        _accidentWalkerSFX = _audioManager.GetAccidentCarSFX();

        InitializeRoutePoints();
        _carVFX.Stop();
    }

    private void Update()
    {
        CheckCarStatus();
        CheckRouteStatus();
    }

    private void InitializeRoutePoints()
    {
        _routePoints = new Vector3[_routeLine.positionCount];
        _routeLine.GetPositions(_routePoints);
        _routePoints[START_INDEX] = _carPrefab.transform.position;
    }

    private void CheckCarStatus()
    {
        if (!_routeCompleted && _routePoints != null && _routePoints.Length > NEXT_POINT_INDEX)
        {
            ClickOnCar();
            OnMove();
        }
    }

    private void CheckRouteStatus()
    {
        if (!_routeCompleted)
        {
            UpdateRouteLine();
        }
        else
        {
            RemoveCarAndRoute();
        }
    }

    private void ClickOnCar()
    {
        if (Input.GetMouseButtonDown(0) && !_isMoving && !_isAccident && !_uiManager._isPopupOpen)
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == _carPrefab.gameObject)
                {
                    _audioManager.PlaySFX(_startEngineSFX, _carSFXSource);

                    Invoke("StartMove", _startEngine);

                }
            }
        }
    }
    private void StartMove()
    {
        _isMoving = true;
        
        _audioManager.StopSFX(_carSFXSource);
        _audioManager.PlayLoopSFX(_engineLoopSFX, _carSFXSource);

        _carVFX.Play();

        Move();
    }

    private void OnMove()
    {
        if (_isMoving)
        {
            Move();
        }
    }

    public void Move()
    {
        Vector3 currentPosition = _carPrefab.transform.position;
        Vector3 targetPosition = _routePoints[_currentRouteIndex];

        _carPrefab.transform.position = Vector3.MoveTowards(currentPosition, targetPosition, _moveSpeed * Time.deltaTime);

        Vector3 destDirection = targetPosition - currentPosition;
        destDirection.y = 0;
        float destDistance = destDirection.magnitude;

        if (currentPosition != targetPosition)
        {
            if (destDistance >= _minDistance)
            {
                Quaternion targetRotation = Quaternion.LookRotation(destDirection);
                _carPrefab.transform.rotation = Quaternion.RotateTowards(_carPrefab.transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
            }
        }
        else
        {
            if (Vector3.Distance(currentPosition, targetPosition) < _minDistance)
            {
                _currentRouteIndex++;

                if (_currentRouteIndex >= _routePoints.Length)
                {
                    _routeCompleted = true;
                }
            }
        }

        InvokeRepeating("ScaleCar", 0f, _timeRepeatScale);

        RotateWheels();
    }

    private void ScaleCar()
    {
        if (_isScaling && _isMoving)
        {
            _carPrefab.transform.localScale = new Vector3(1f, _sizeScale, 1f);
        }
        else
        {
            _carPrefab.transform.localScale = new Vector3(1f, 1f, 1f);
        }

        _isScaling = !_isScaling;
    }

    private void RotateWheels()
    {
        for (int i = 0; i < _wheels.Count; i++)
        {
            float rotationDirection = (i < 2) ? 1f : -1f;

            _wheels[i].Rotate(Vector3.up * rotationDirection * _wheelRotationSpeed * Time.deltaTime);
        }
    }

    private void RemoveRoutePoint(int indexToRemove)
    {
        List<Vector3> newRoutePointsList = new List<Vector3>(_routePoints);
        newRoutePointsList.RemoveAt(indexToRemove);
        _routePoints = newRoutePointsList.ToArray();

        _routeLine.positionCount = _routePoints.Length;
        _routeLine.SetPositions(_routePoints);
    }

    private void UpdateRouteLine()
    {
        _routePoints[START_INDEX] = _carPrefab.transform.position;
        _routeLine.SetPosition(START_INDEX, _routePoints[START_INDEX]);

        for (int i = NEXT_POINT_INDEX; i < _routeLine.positionCount - 1; i++)
        {
            if (_routePoints[i] == _routePoints[START_INDEX])
            {
                RemoveRoutePoint(i);
                break;
            }
        }

        if (_currentRouteIndex >= _routePoints.Length)
        {
            _routeCompleted = true;
            RemoveCarAndRoute();

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Car"))
        {
            _isMoving = false;
            _isAccident = true;

            _carVFX.Stop();

            _audioManager.PlaySFX(_accidentCarSFX, _carSFXSource);

            _uiManager.ShowAccidentCarPopup();
        }
        else if (other.gameObject.CompareTag("Walker"))
        {
            _isMoving = false;
            _isAccident = true;

            _carVFX.Stop();

            _audioManager.PlaySFX(_accidentWalkerSFX, _carSFXSource);

            _uiManager.ShowAccidentWalkerPopup();
        }
    }

    private void RemoveCarAndRoute()
    {
        if (_carPrefab != null && _routeLine != null)
        {
            if (_sceneLoader.sceneCarsList != null)
            {
                _sceneLoader.RemoveCarFromList(_carPrefab);
            }

            Destroy(_carPrefab);
            Destroy(_routeLine.gameObject);

            _audioManager.StopSFX(_carSFXSource);
         }
    }
}