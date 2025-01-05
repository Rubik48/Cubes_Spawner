using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    [Header("Pool Settings")]
    [SerializeField] private Cube _cubePrefab;
    [SerializeField] private int _defaultCapacity = 5;
    [SerializeField] private int _maxPoolSize = 5;

    [Header("Spawn Settings")] 
    [SerializeField] private float _spawnInterval = 1f;
    [SerializeField] private float _spawnHeight = 10f;
    [SerializeField] private float _spawnAreaWidth = 10f;

    private IObjectPool<Cube> _cubePool;
    private Cube _cube;
    
    private int _activeCubes = 0;
    private float _divide = 2;

    private void Awake()
    {
        _cubePool = new ObjectPool<Cube>(
            createFunc: () => Instantiate(_cubePrefab),
            actionOnGet: cube => cube.gameObject.SetActive(true),
            actionOnRelease: cube => cube.gameObject.SetActive(false),
            actionOnDestroy: cube => Destroy(cube.gameObject),
            collectionCheck: true,
            defaultCapacity: _defaultCapacity,
            maxSize: _maxPoolSize
        );
    }

    private void Start()
    {
        StartCoroutine(SpawnCubes());
    }

    private void FixedUpdate()
    {
        Debug.Log(_cubePool.CountInactive);
    }

    private void SpawnCube()
    {
        float x = Random.Range(-_spawnAreaWidth / _divide, _spawnAreaWidth / _divide);
        float z = Random.Range(-_spawnAreaWidth / _divide, _spawnAreaWidth / _divide);
        
        _cube = _cubePool.Get();
        
        _activeCubes++;

        _cube.LifeTimeFinished += ReleaseCube;
        
        _cube.transform.position = new Vector3(x, _spawnHeight, z);
    }

    private void ReleaseCube(Cube cube)
    {
        cube.LifeTimeFinished -= ReleaseCube;
        
        _activeCubes--;
        
        _cubePool.Release(cube);
    }

    private IEnumerator SpawnCubes()
    {
        while (true)
        {
            if (_activeCubes < _maxPoolSize)
                SpawnCube();
            
            yield return new WaitForSeconds(_spawnInterval);
        }
    }
}
