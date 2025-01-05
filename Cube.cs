using UnityEngine;
using System;
using System.Collections;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Renderer), typeof(Rigidbody))]
public class Cube : MonoBehaviour
{
    [SerializeField] private float _minTimeLife = 2f;
    [SerializeField] private float _maxTimeLife = 5f;
    
    private Renderer _renderer;
    private Coroutine _countTimeLife;
    
    private float _timeLife;
    private float _timeDelay = 1f;
    private float _seconds = 0f;
    private bool _wasChanged = false;
    
    public event Action<Cube> LifeTimeFinished;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        
        _timeLife = Random.Range(_minTimeLife, _maxTimeLife);
    }
    
    private void OnEnable()
    {
        _wasChanged = false;
        _seconds = 0;
        _timeLife = Random.Range(_minTimeLife, _maxTimeLife);
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        Platform platform = collision.gameObject.GetComponent<Platform>();
        
        if (platform != null && _wasChanged == false)
        {
            _wasChanged = true;
            
            ChangeColor();

            _countTimeLife = StartCoroutine(RunLifeTime());
        }
    }

    private void OnDisable()
    {
        if(_countTimeLife != null)
            StopCoroutine(_countTimeLife);
    }

    private void ChangeColor()
    {
        _renderer.material.color = Random.ColorHSV();
    }
    
    private IEnumerator RunLifeTime()
    {
        while (_seconds <= _timeLife)
        {
            yield return new WaitForSeconds(_timeDelay);

            _seconds++;
        }
        
        LifeTimeFinished?.Invoke(this);
    }
}