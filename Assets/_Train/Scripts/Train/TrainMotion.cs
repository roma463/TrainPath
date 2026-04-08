using System;
using _Train.Scripts.Root;
using TMPro;
using UnityEngine;

namespace _Train.Scripts.Train
{
    public class TrainMotion : MonoBehaviour, IMovableObject, IFixedUpdater
    {
        [SerializeField] private Rails.Rails rails;
        [SerializeField] private float acceleration = 8f;
        [SerializeField] private float braking = 14f;
        [SerializeField] private Rigidbody trainRb;
        [SerializeField] private float maxSpeed = 10f;
        [SerializeField] private TrainMotor motor;
        [SerializeField] private float speedBraking;
        
        [SerializeField, Range(0f, 1f)] private float range;
        
        [Header("Debug")]
        [SerializeField] private TMP_Text viewSpeed;
        
        private Quaternion previousRotation;
        private float _currentSpeed;
        
        public Vector3 LinearVelocity => _leniarVeloty;
        public Vector3 AngularVelocity
        {
            get
            {
                return trainRb.angularVelocity;
            }
        }

        private Vector3 _leniarVeloty;
        private Vector3 _lastPosition;

        private void Awake()
        {
            _lastPosition = trainRb.transform.position;
            
            trainRb = GetComponent<Rigidbody>();
            trainRb.interpolation = RigidbodyInterpolation.Interpolate;
            var startPosition = rails.GetStartPosition();
            
            transform.position = startPosition;
            previousRotation = transform.rotation;
            
        }
        public void SetTargetPower(float speed)
        {
            motor.SetTargetPower(speed);
        }
        
        private void FixedUpdate()
        {

        }

        private void UpdateDebug()
        {
            viewSpeed.text = $"current speed: <color=yellow>{(float)Math.Round(_currentSpeed, 1)}</color>";
            // Debug.Log("Train FixedUpdate");
        }
        
        private void MoveTrain()
        {
            var position = rails.GetPosition(_currentSpeed * Time.fixedDeltaTime);
            var rotate = rails.GetRotationOnCurrentRange();
            
            _leniarVeloty = (position - _lastPosition) / Time.fixedDeltaTime;
            _lastPosition = position;
            
            trainRb.MovePosition(position);
            trainRb.MoveRotation(rotate);

            Quaternion deltaRotation = rotate * Quaternion.Inverse(previousRotation);
            deltaRotation.ToAngleAxis(out float angle, out Vector3 axis);
        }

        private void UpdateSpeed()
        {
            var currentMotorSpeed = motor.NormalPower * maxSpeed; 
            
            if(Mathf.Approximately(_currentSpeed, currentMotorSpeed))
                return;
            
            if (_currentSpeed < currentMotorSpeed)
            {
                _currentSpeed = currentMotorSpeed;
            }
            else
            {
                _currentSpeed = Mathf.MoveTowards(_currentSpeed, currentMotorSpeed, speedBraking * Time.fixedDeltaTime);
            }
        }

        public void FUpdate()
        {
            UpdateSpeed();
            MoveTrain();
            UpdateDebug();
        }
    }
}
