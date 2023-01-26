using System;
using Lean.Pool;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class LevelSpawner : MonoBehaviour
{
    [SerializeField] private int numberOfStartPlatforms = 9;

    [SerializeField] private Platform[] platform;
    [SerializeField] private float velocity;
    [SerializeField] private LevelBorder topBorder;
    [SerializeField] private LevelBorder bottomBorder;

    [Inject] private SignalBus _signalBus;

    private float _xOffset;
    private float _yOffset;
    private float _boxSize;

    private bool _isStartGenerationFinished;

    private void Start()
    {
        _boxSize = platform[0].Col.size.y + 0.1f;
        var screenBorder =
            Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));

        topBorder.transform.position = new Vector3(0, screenBorder.y, 0);
        bottomBorder.transform.position = new Vector3(0, -screenBorder.y, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Platform platf))
        {
            _signalBus.Fire(new Signals.SpawnPlatformSignal());

            LeanPool.Despawn(platf);
        }
    }

    private void OnEnable()
    {
        _signalBus.Subscribe<Signals.FirstTouchSignal>(StartSpawn);
        _signalBus.Subscribe<Signals.SpawnPlatformSignal>(SpawnPlatform);
    }

    private void OnDisable()
    {
        _signalBus.Unsubscribe<Signals.FirstTouchSignal>(StartSpawn);
        _signalBus.Unsubscribe<Signals.SpawnPlatformSignal>(SpawnPlatform);
    }

    private void StartSpawn()
    {
        for (int i = 0; i < numberOfStartPlatforms; i++)
        {
            SpawnPlatform();
            if (i < numberOfStartPlatforms - 1)
            {
                _yOffset += _boxSize;
            }
        }

        _isStartGenerationFinished = true;
    }

    private void SpawnPlatform()
    {
        int index = 0;

        if (_isStartGenerationFinished)
        {
            index = Random.Range(0, platform.Length);
        }

        var platf = LeanPool.Spawn(platform[index], transform);
        var x = (float) Math.Round(Random.Range(-platf.XOffset, platf.XOffset) * 2, MidpointRounding.AwayFromZero) / 2;
        var pos = new Vector3(x, transform.position.y + _yOffset, 0);
        platf.Init(velocity, pos);
    }
}