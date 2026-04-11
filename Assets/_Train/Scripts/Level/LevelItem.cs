using System;
using _Train.Scripts.Root;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;

namespace _Train.Scripts.Level.Items.Data
{
    public class LevelItem : MonoBehaviour
    {
        public ItemData ItemData { get; private set; }
        public bool IsLoaded { get; private set; }
        
        [SerializeField] private int id = -1;

        public void SetIndex(int index)
        {
            id = index;
        }
        
        public void Start()
        {
            if (ItemData == null && id != -1)
            {
                ItemData = GameEntryPoint.Instance.ItemsConfig.GetItemDataByIndex(id);
                IsLoaded = true;
            }
        }
    }
}
