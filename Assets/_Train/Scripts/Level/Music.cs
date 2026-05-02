using UnityEngine;

namespace _Train.Scripts.Level
{
    public class Music : MonoBehaviour
    {
        [SerializeField] private GameObject screnObject;
        [SerializeField] private AudioSource sound;
        [SerializeField] private Transform rotateObject;
        
        [SerializeField] private Vector3 rotateVector;
        [SerializeField] private float speedRotation;
        [SerializeField] private float speedForSpeed;
        
        private INPUTE input;
        
        private float _normolizedSpeed;
        private bool _isPlay;
        private bool _isStop;
        private float _currentSpeed;
        private float _targetSpeed;

        private void Start()
        {
            input = new INPUTE();
            input.Enable();
            input.OnJumpPerformed += OnChangePlayingMode;
        }

        private void OnDestroy()
        {
            input.Disable();
            input.OnJumpPerformed += OnChangePlayingMode;
        }

        private void OnChangePlayingMode()
        {
            _isPlay = !_isPlay;
                
            _targetSpeed = _isPlay ? speedRotation : 0;
        }
        
        private void Update()
        {
            if (!Mathf.Approximately(_currentSpeed, _targetSpeed))
            {
                _currentSpeed = Mathf.MoveTowards(_currentSpeed, _targetSpeed, Time.deltaTime * speedForSpeed);
                _normolizedSpeed = _currentSpeed / speedRotation;
            }

            if (!Mathf.Approximately(_currentSpeed, 0f))
            {
                rotateObject.Rotate(transform.up, _currentSpeed * Time.deltaTime, Space.World);
            }

            CheckChangePause();
            sound.pitch = _normolizedSpeed;
        }

        private void CheckChangePause()
        {
            if (_currentSpeed != 0 && _isStop)
            {
                _isStop = false;
                ChangePause();
            }
            else if (_currentSpeed == 0 && !_isStop)
            {
                _isStop = true;
                ChangePause();
            }
        }

        private void ChangePause()
        {
            if (_isStop)
                sound.Pause();
            else
                sound.UnPause();
        }
    }
}
