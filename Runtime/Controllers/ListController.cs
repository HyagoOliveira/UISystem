using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Controller for a List with item placement.
    /// Use it along side with ScrollRect.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ScrollRect))]
    public sealed class ListController : MonoBehaviour
    {
        [Tooltip("The item that will be instantiated inside this list.")]
        public GameObject itemPrefab;
        [Tooltip("The place where items will be placed.")]
        public Transform itemContainer;

        private readonly List<GameObject> items = new();

        public T Add<T>() where T : Component
        {
            var instance = Add();
            return instance.GetComponent<T>();
        }

        public GameObject Add() => Add(itemPrefab);

        public GameObject Add(GameObject prefab)
        {
            var item = Instantiate(prefab);
            item.transform.SetParent(itemContainer, worldPositionStays: false);

            items.Add(item);

            return item;
        }

        public GameObject GetItem(int index)
        {
            var hasItem = index >= 0 && index < items.Count;
            if (hasItem) return items[index];
            throw new System.ArgumentOutOfRangeException();
        }

        public void Select(int index) =>
            EventManager.TrySetSelectedGameObject(GetItem(index));

        public void Clear()
        {
            items.Clear();
            foreach (Transform child in itemContainer)
            {
                Destroy(child.gameObject);
            }
        }
    }
}