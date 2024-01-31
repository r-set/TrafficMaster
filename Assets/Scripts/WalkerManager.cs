using System.Collections;
using UnityEngine;

public class WalkerManager : MonoBehaviour
{
    [Header("Walk Settings")]
    [SerializeField]  private Vector3[] _waypointsCoordinates;
    [SerializeField]  private float _waitTime = 2f;
    [SerializeField]  private float _speedWalker = 2f;

    private Animator _animator;

    private int _currentWaypointIndex = 0;
    private bool _isWaiting = false;
    private bool _isMoving = false;
    private bool _isFalling = false;

    private UIManager _uiManager;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        if (_waypointsCoordinates.Length == 0)
        {
            enabled = false;
            return;
        }

        _uiManager = UIManager.Instance;

        StartCoroutine(MoveToWaypoints());
    }

    private IEnumerator MoveToWaypoints()
    {
        while (true)
        {
            if (!_isWaiting && !_uiManager._isPopupOpen)
            {
                yield return StartCoroutine(MoveTowardsWaypoint());

                _isWaiting = true;
                _isMoving = false;
                _animator.SetBool("isMoving", _isMoving);

                yield return new WaitForSeconds(_waitTime);
                _isWaiting = false;

                FlipMove();

                _currentWaypointIndex = (_currentWaypointIndex + 1) % _waypointsCoordinates.Length;
            }

            yield return null;
        }
    }

    private IEnumerator MoveTowardsWaypoint()
    {
        Vector3 targetPosition = _waypointsCoordinates[_currentWaypointIndex];
        while (transform.position != targetPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, _speedWalker * Time.deltaTime);

            _isMoving = true;
            _animator.SetBool("isMoving", _isMoving);

            yield return null;
        }
    }

    private void FlipMove()
    {
        transform.Rotate(0f, 180f, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Car"))
        {
            _speedWalker = 0f;

            _isFalling = true;
            _animator.SetBool("isFalling", _isFalling);

            Invoke("AccidentalFall", 2f);
        }
    }

    private void AccidentalFall()
    {
         _animator.enabled = false;
    }
}