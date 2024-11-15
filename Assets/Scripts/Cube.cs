using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class Cube : MonoBehaviour
{
    public event Action<Cube> OnLifeTimeExpired;

    [SerializeField] private Color _color;

    private Renderer _renderer;
    private Color _defaultColor;

    public int LifeTime { get; private set; }

    public bool IsThereCollision { get; private set; }

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        IsThereCollision = false;
        _defaultColor = _renderer.material.color;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (IsThereCollision == false)
        {
            if (collider.GetComponent<Platform>() != null)
            {            
                StartCoroutine(DecreaseLifeTime());
                ChangeColor();
                IsThereCollision = true;
            }
        }
    }

    public void Init() 
    {
        int minTime = 2;
        int maxTime = 5;

        LifeTime = UnityEngine.Random.Range(minTime, maxTime + 1);
        _renderer.material.color = _defaultColor;
        IsThereCollision = false;
    }

    private void ChangeColor()
    {
        _renderer.material.color = _color;
    }

    private IEnumerator DecreaseLifeTime()
    {
        yield return new WaitForSeconds(LifeTime);
        OnLifeTimeExpired?.Invoke(this);
    }
}