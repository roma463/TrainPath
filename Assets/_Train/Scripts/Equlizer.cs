using System;
using UnityEngine;

public class Equlizer : MonoBehaviour
{
    private static Equlizer _instance;

    [SerializeField] private int numberOfBars = 64;
    [SerializeField] private float sensitivity = 100.0f;

    [SerializeField] private float _offset;
    [SerializeField] private LineEqulizer _prefabCubeElement;
    [SerializeField] private float _speedMoveLines = .5f;
    [SerializeField] private int _countVisibleLine = 3;
    [SerializeField] private bool _createIsRight = true;
    [SerializeField] private bool _isEnable = true;
    [SerializeField] private LineEqulizer[] _listOfject;

    private float[] audioData;

    public static void Activate()
    {
        _instance.StartAnimation();
    }

    public void StartAnimation()
    {
        audioData = new float[numberOfBars];
        
        if (_listOfject.Length > 0)
            return;
        
        _listOfject = new LineEqulizer[numberOfBars];
        
        for (int i = 0; i < _countVisibleLine; i++)
        {
            var directionCreate = _createIsRight ? transform.right : -transform.right;
            _listOfject[i] = Instantiate(_prefabCubeElement, transform.position + (directionCreate * i * _offset), transform.rotation);
            _listOfject[i].transform.parent = transform;
        }
    }

    private void Awake()
    {
        _instance = this;
        Activate();
    }

    private void Update()
    {
        if (!_isEnable)
            return;

        AudioListener.GetSpectrumData(audioData, 0, FFTWindow.Rectangular);

        for (int i = 0; i < _countVisibleLine; i++)
        {
            float barHeight = audioData[i] * sensitivity;

            _listOfject[i].UpdateView(barHeight);
        }
    }

    private void OnDrawGizmos()
    {
        if (!_isEnable)
            return;

        for (int i = 0; i < _countVisibleLine; i++)
        {
            var directionCreate = _createIsRight ? transform.right : -transform.right;
            Gizmos.DrawCube(transform.position + (directionCreate * i * _offset), Vector2.one * .1f);
        }
    }
}
