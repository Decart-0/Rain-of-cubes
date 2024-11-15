using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(Collider))]
public class Spawner : MonoBehaviour
{
    [SerializeField] private Cube _cubePrefab;
    [SerializeField] private float _repeatRate;
    [SerializeField] private int _heightSpawn;
    [SerializeField] private int _poolMaxSize;
    [SerializeField] private int _poolCapacity;
    [SerializeField] private bool _isSpawning = true;

    private ObjectPool<Cube> _pool;
    private Collider _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider>();

        _pool = new ObjectPool<Cube>
        (
            createFunc: () => Instantiate(_cubePrefab),
            actionOnGet: (cube) => ExecuteOnGet(cube),
            actionOnRelease: (cube) => cube.gameObject.SetActive(false),
            actionOnDestroy: (cube) => Destroy(cube),
            collectionCheck: true,
            defaultCapacity: _poolCapacity,
            maxSize: _poolMaxSize
        );
    }

    private void Start()
    {
        StartCoroutine(SpawnFrequency());
    }

    private void ReturnObjectPool(Cube cube) 
    {
        cube.OnLifeTimeExpired -= ReturnObjectPool;
        _pool.Release(cube);
    }

    private void ExecuteOnGet(Cube cube)
    {
        cube.OnLifeTimeExpired += ReturnObjectPool;
        cube.transform.position = GetRandomSpawnPoint();
        cube.gameObject.SetActive(true);     
        SetupCube(cube);
        SetupRigidbody(cube);
    }

    private void SetupCube(Cube cube)
    {
        cube.Init();
    }

    private void SetupRigidbody(Cube cube)
    {
        Rigidbody rigidbody = cube.GetComponent<Rigidbody>();

        if (rigidbody == null)
            rigidbody = cube.gameObject.AddComponent<Rigidbody>();

        rigidbody.velocity = Vector3.zero;
    }

    private void GetCube()
    {
        _pool.Get();
    }

    private Vector3 GetRandomSpawnPoint()
    {
        Vector3 colliderSize = _collider.bounds.size;
        float half = 0.5f;
        float maxDistance = Mathf.Min(colliderSize.x, colliderSize.z) * half;
        float randomAngle = Random.Range(0f, 2 * Mathf.PI);
        float randomDistance = Random.Range(0f, maxDistance);
        float x = transform.position.x + Mathf.Cos(randomAngle) * randomDistance;
        float z = transform.position.z + Mathf.Sin(randomAngle) * randomDistance;
        float y = transform.position.y + _heightSpawn;

        return new Vector3(x, y, z);
    }

    private IEnumerator SpawnFrequency() 
    {       
        while (_isSpawning) 
        {
            GetCube();
            yield return new WaitForSeconds(_repeatRate);
        }
    }
}