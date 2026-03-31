using UnityEngine;

namespace _Train.Scripts.Train
{
    public interface IConnectableObject
    {
        public Transform ConnectTransform { get; }
    }
}