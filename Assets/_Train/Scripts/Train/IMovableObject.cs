
using UnityEngine;

namespace _Train.Scripts.Train
{
    public interface IMovableObject
    {
        public Vector3 LinearVelocity { get; }
        public Vector3 AngularVelocity { get; }
    }
}