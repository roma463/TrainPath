using System.Collections.Generic;
using System.Linq;
using _Train.Scripts.Level.Items.Data;
using _Train.Scripts.Root;
using UnityEngine;

namespace _Train.Scripts
{
    public class PickupObject : MonoBehaviour, IGrabble
    {
        [SerializeField] private Rigidbody rigidBody;
        [SerializeField] private Collider collider;
        [SerializeField] private LevelItem levelItem;
        
        [SerializeField] private List<GameObject> layerChangeableObjects = new List<GameObject>();

        public Rigidbody Rigidbody => rigidBody;
        public virtual string GrabAnimName => "Take";
        public virtual string DropAnimName => "Drop";
        public virtual string ThrowAnimName => "Throw";
        public virtual string InteractDoName => "Take";
        public LevelItem LevelItem => levelItem;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (layerChangeableObjects.Count == 0)
            {
                var meshRenderers = GetComponentsInChildren<MeshRenderer>();
                layerChangeableObjects = meshRenderers.Select(p=>p.gameObject).ToList();
                // UnityEditor.EditorUtility.SetDirty(this);
                UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(this);
            }
        }
#endif

        public virtual bool CanThrow()
        {
            return true;
        }

        public virtual bool CanAttach()
        {
            return true;
        }

        public void RemoveVelocity()
        {
            rigidBody.linearVelocity = Vector3.zero;
            rigidBody.angularVelocity = Vector3.zero;
        }

        protected virtual void AfterGrab()
        {
            
        }

        public virtual bool CanGrab()
        {
            return true;
        }

        public void Grab(Character.Character character)
        {
            RemoveVelocity();
            rigidBody.interpolation = RigidbodyInterpolation.None;
            rigidBody.isKinematic = true;
            collider.enabled = false;
            AfterGrab();

        }

        public void Drop()
        {
            rigidBody.interpolation = RigidbodyInterpolation.Interpolate;
            rigidBody.isKinematic = false;
            collider.enabled = true;
            RemoveVelocity();
            AfterDrop();
        }

        public void Throw(Vector3 direction, float force)
        {
            Drop();
            rigidBody.AddForce(direction * force, ForceMode.Impulse);
        }

        public void ChangeLayer(string newLayer)
        {
            foreach (var obj in layerChangeableObjects)
            {
                obj.layer = LayerMask.NameToLayer(newLayer);
            }
        }
        
        protected virtual void AfterDrop()
        {
            
        }
    }
}
