using System;
using _Train.Scripts.Character;
using _Train.Scripts.Root;
using _Train.Scripts.Train.Root;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Train.Scripts.Train
{
    public class Ventilation : MonoBehaviour, ISpendTemperature, IInteractable, INotifyStateChanged
    {
        public event Action OnChange;
        public Transform RootTransform { get; }
        [SerializeField] private float spendTemperature = 10;
        [SerializeField] private Transform view;
        [SerializeField] private Vector3 axisRotate;
        [SerializeField] private float angleOpen;

        private Quaternion _openRotation;
        private Quaternion _closeRotation;
        private bool _isOpen;

        private void Start()
        {
            _openRotation = Quaternion.Euler(axisRotate * angleOpen);
            _closeRotation = view.localRotation;
        }

        public float SpendPercent()
        {
            return _isOpen ? spendTemperature : 0f;
        }

        public string GetPromt(CharacterContext character)
        {
            return _isOpen ? "Закрыть" : "Открыть";
        }

        public bool CanInteract(CharacterContext character)
        {
            return true;
        }

        public void Interact(CharacterContext character)
        {
            _isOpen = !_isOpen;
            view.localRotation = _isOpen ? _openRotation : _closeRotation;
        }

    }
}