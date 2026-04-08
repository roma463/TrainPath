using System.Collections.Generic;
using UnityEngine;

namespace _Train.Scripts.Root
{
    public class FramesController : MonoBehaviour
    {
        [SerializeField] private GameObject[] updaterObjects;

        private List<IFixedUpdater> _updaters = new List<IFixedUpdater>(10);
    
        private void Start()
        {
            foreach (var updater in updaterObjects)
            {
                if (updater.TryGetComponent(out IFixedUpdater updaterObject))
                {
                    _updaters.Add(updaterObject);
                }
            }
        }

        private void FixedUpdate()
        {
            foreach (var updater in _updaters)
            {
                updater.FUpdate();
            }
        }
    }
}
