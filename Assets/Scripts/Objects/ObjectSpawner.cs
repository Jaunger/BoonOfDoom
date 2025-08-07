using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [Header("Object Prefab")]
    [SerializeField] GameObject _gameObject;
    [SerializeField] GameObject instantiateGameObject;

    private void Awake()
    {
    }

    private void Start()
    {
        WorldObjectManager.instance.SpawnObject(this);
        gameObject.SetActive(false);
    }



    public void AttemptToSpawnObject()
    {
        if (_gameObject != null)
        {
            instantiateGameObject = Instantiate(_gameObject, transform.position, Quaternion.identity);
        }
    }
}
