using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasePool<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField] private T Prefab;
    [SerializeField] private Transform PoolStorageTransform;
    [SerializeField] int PreparingCount = 50;

    private List<T> pooledObjects = new List<T>();

    private void Awake()
    {
        InitPool();
    }

    private void InitPool()
    {
        pooledObjects = new List<T>();
        for (int i = 0; i < PreparingCount; i++)
        {
            T newObject = Instantiate<T>(Prefab, PoolStorageTransform);
            newObject.gameObject.SetActive(false);
            pooledObjects.Add(newObject);
        }
    }

    public T GetPooledObject()
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (pooledObjects[i].gameObject == null)
                pooledObjects.RemoveAt(i);

            if (!pooledObjects[i].gameObject.activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }

        T newObject = Instantiate<T>(Prefab, PoolStorageTransform);
        newObject.gameObject.SetActive(false);
        pooledObjects.Add(newObject);
        return newObject;
    }

    public void ReturnObject(T obj)
    {
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(PoolStorageTransform);
    }
}