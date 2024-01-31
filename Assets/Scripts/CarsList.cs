using System.Collections.Generic;
using UnityEngine;

public class CarsList : MonoBehaviour
{
    [SerializeField] private List<GameObject> _carsList;

    private SceneLoader _sceneLoader;

    private void Start()
    {
        _sceneLoader = SceneLoader.Instance;

        InitializeCarsList();
    }

    private void InitializeCarsList()
    {
        _sceneLoader.sceneCarsList = _carsList;
    }
}
