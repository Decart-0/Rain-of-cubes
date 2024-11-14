using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class Cube : MonoBehaviour
{
    [SerializeField] private Spawner _spawner;
    [SerializeField] private Color _color;

    private Renderer _renderer;
    private Color _defaultColor;

    public int LifeTime { get; private set; }

    public bool IsChangedColor { get; private set; }

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        IsChangedColor = false;
        _defaultColor = _renderer.material.color;
    }

    public void InstallLifeTime()
    {
        int minTime = 2;
        int maxTime = 5;

        LifeTime = Random.Range(minTime, maxTime + 1);
    }

    public void SetDefaultColor()
    {
        IsChangedColor = false;
        _renderer.material.color = _defaultColor;
    }

    private IEnumerator DecreaseLifeTime()
    {
        while (LifeTime > 0)
        {
            yield return new WaitForSeconds(1f);
            LifeTime--;
        }

        _spawner.ReturnObjectPool(gameObject);
    }

    private void OnTriggerEnter(Collider collider)
    {
        string tag = "platform";

        if (collider.tag == tag)
        {                    
            if (IsChangedColor == false)
            {
                StartCoroutine(DecreaseLifeTime());
                ChangeColor();
            }    
        }
    }

    private void ChangeColor()
    {
        _renderer.material.color = _color;
        IsChangedColor = true;
    }
}
