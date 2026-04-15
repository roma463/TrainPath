using UnityEngine;

namespace _Train.Scripts.Train.Obstacle
{
    public class TrainObstacleTrigger : MonoBehaviour
    {
        [SerializeField] private global::Train train;
        [SerializeField] private TrainMotion trainMotion;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Obstacle obstacle))
            {
                Debug.Log(obstacle.name);
                train.TakeDamage(10);
                trainMotion.Stop();
            }
        }
    }
}
