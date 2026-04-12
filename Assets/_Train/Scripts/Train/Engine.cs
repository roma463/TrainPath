using _Train.Scripts.Train.Motors;
using UnityEngine;

namespace _Train.Scripts.Train
{
    public class Engine : MonoBehaviour
    {
        [SerializeField] private TrainMotor motor;
        [SerializeField] private Transform moveItem;
        [SerializeField] private float moveSpeed;
        [SerializeField] private float offset;

        [SerializeField] private Vector3 openPosition;
        private Vector3 positionClosed;

        private float _currentT;
    
        private void Start()
        {
            _currentT = Random.Range(0f, 1f);
            positionClosed = moveItem.localPosition;
        }

        private void Update()
        {
            if (motor.IsActive && motor.NormalPower > 0)
            {
                _currentT += Time.deltaTime * (moveSpeed * motor.NormalPower);
                var t = Mathf.PingPong(_currentT, 1);
                moveItem.localPosition = Vector3.Lerp(positionClosed, openPosition, t);
            }
        }
    }
}
