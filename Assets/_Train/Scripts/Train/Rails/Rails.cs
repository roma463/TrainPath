using Mirror;
using UnityEngine;
using UnityEngine.Splines;

namespace _Train.Scripts.Train.Rails
{
    public class Rails : MonoBehaviour
    {
        [SerializeField] private SplineContainer splineContainer;

        private float _currentRange;
        private float _lenght;

        private float _syncedValue;
    
        private float _currentValue;     
        private float _targetValue;      
        private float _previousValue;    
    
        private double _receiveTime;
        private double _previousReceiveTime;
        
        private void Reset()
        {
            splineContainer = GetComponent<SplineContainer>();
        }
        
        private void Start()
        {
            _lenght = splineContainer.CalculateLength();
        }

        public Vector3 GetStartPosition()
        {
            return splineContainer.EvaluatePosition(0);
        }

        public Vector3 GetPosition(float normalizeSpeed)
        {
            if (normalizeSpeed != 0)
                _currentRange += (_currentRange / _lenght + normalizeSpeed) / _lenght;
            
            _currentRange = Mathf.Clamp01(_currentRange);
            
            return splineContainer.EvaluatePosition(_currentRange);
        }

        public Quaternion GetRotationOnCurrentRange()
        {
            Vector3 tangent = splineContainer.EvaluateTangent(_currentRange);
            Vector3 upVector = splineContainer.EvaluateUpVector(_currentRange);
            
            return Quaternion.LookRotation(tangent, upVector);
        }
    }
}
