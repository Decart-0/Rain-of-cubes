using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(Collider))]
public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject _cubePrefab;
    [SerializeField] private float _repeatRate;
    [SerializeField] private int _heightSpawn;
    [SerializeField] private int _poolMaxSize;
    [SerializeField] private int _poolCapacity;

    private ObjectPool<GameObject> _pool;
    private Collider _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider>();

        _pool = new ObjectPool<GameObject>
        (
            createFunc: () => Instantiate(_cubePrefab),
            actionOnGet: (obj) => ActionOnGet(obj),
            actionOnRelease: (obj) =>obj.SetActive(false),
            actionOnDestroy: (obj) => Destroy(obj),
            collectionCheck: true,
            defaultCapacity: _poolCapacity,
            maxSize: _poolMaxSize
        );
    }

    private void Start()
    {
        InvokeRepeating(nameof(GetCube), 0.0f, _repeatRate);
    }

    public void ReturnObjectPool(GameObject obj) 
    {
        _pool.Release(obj);
    }

    private void ActionOnGet(GameObject obj)
    {
        obj.transform.position = GetRandomSpawnPoint();
        obj.SetActive(true);
        SetupCube(obj);
        SetupRigidbody(obj);
    }

    private void SetupCube(GameObject obj)
    {
        Cube cube = obj.GetComponent<Cube>();

        if (cube == null)
            cube = obj.AddComponent<Cube>();

        cube.InstallLifeTime();
        cube.SetDefaultColor();
    }

    private void SetupRigidbody(GameObject obj)
    {
        Rigidbody rigidbody = obj.GetComponent<Rigidbody>();

        if (rigidbody == null)
            rigidbody = obj.AddComponent<Rigidbody>();

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
}