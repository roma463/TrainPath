using System;
using _Train.Scripts.Root;
using UnityEngine;

namespace _Train.Scripts.Train.Buttons
{
    public class Lever : MonoBehaviour, IHoverable
    {
        public event Action<float> OnChange;
        
        [SerializeField] private float maxAngle = 180;
        [SerializeField] private Vector3 axisRotate = Vector3.right;
        [SerializeField] private int countStates = 3;
        [SerializeField] private Transform view;
        
        private INPUTE _inpute;
        private bool _isActive;
        private float _currentState;
        
        private void Start()
        {
            _inpute = INPUTE.instance;
        }

        public void OnHoverEnter()
        {
            _isActive = true;
        }

        private void Update()
        {
            if (_isActive)
            {
                var direction = _inpute.ScrollLock.y;
                
                if (direction == 0)
                    return;
                
                _currentState += direction > 0 ? 1 : -1;
                _currentState = Mathf.Clamp(_currentState, 0, countStates);
                view.localRotation = Quaternion.Euler(axisRotate * (_currentState / countStates * maxAngle));
                
                OnChange?.Invoke(_currentState / countStates);
            }
        }

        public void OnHoverExit()
        {
            _isActive = false;
        }
    }
}