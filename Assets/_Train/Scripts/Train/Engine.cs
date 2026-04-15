using System;
using _Train.Scripts.Character;
using _Train.Scripts.Train.Motors;
using _Train.Scripts.Train.Root;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Train.Scripts.Train
{
    public class Engine : MonoBehaviour, IBrakingDown, IFixble
    {
        public event Action UpdatedPercent;
        
        [SerializeField] private TMP_Text view;
        [SerializeField] private TrainMotor motor;
        [SerializeField] private Transform moveItem;
        [SerializeField] private float moveSpeed;
        [SerializeField] private float offset;
        [SerializeField] private Vector3 openPosition;
        
        private Vector3 positionClosed;

        private float _currentT;
        private float _currentPercent = 0.5f;

        public float CurrentPower => _currentPercent * 100;
        
        private void Start()
        {
            UpdateView();
            _currentT = Random.Range(0f, 1f);
            positionClosed = moveItem.localPosition;
        }

        private void Update()
        {
            if (motor.IsActive && motor.NormalPower > 0)
            {
                _currentT += Time.deltaTime * ((moveSpeed * _currentPercent) * motor.NormalPower);
                var t = Mathf.PingPong(_currentT, 1);
                moveItem.localPosition = Vector3.Lerp(positionClosed, openPosition, t);
            }
        }

        public void Break(float percent)
        {
            _currentPercent = Mathf.Clamp01(_currentPercent - percent);
            UpdateView();
        }

        public string GetPromt(CharacterContext character)
        {
            return CanInteract(character) ? "Fix it" : "пшел отсюда";
        }

        public bool CanInteract(CharacterContext character)
        {
            return character.GrabSystem.CurrentGrabObject != null && character.GrabSystem.CurrentGrabObject is Key;
        }

        public void Interact(CharacterContext character)
        {
            _currentPercent = 1;
            UpdateView();
            UpdatedPercent?.Invoke();
        }

        private void UpdateView()
        {
            view.text = $"{_currentPercent * 100}%";
        }
        
        public void Abort()
        {
            
        }
    }
}
