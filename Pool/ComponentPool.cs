using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Devdog.General
{
    public class ComponentPool<T> : PoolBase<T> where T : class, IPoolable
    {
        private static List<IPoolable> _interfaceCache = new List<IPoolable>();


        public ComponentPool(T baseObject, int startSize = 32)
            : base(baseObject, startSize)
        {

        }

        public override T Instantiate()
        {
            var obj = UnityEngine.Object.Instantiate<GameObject>(baseObject.gameObject);

            obj.transform.SetParent(rootObject);
            obj.gameObject.SetActive(false); // Start disabled

            var c = obj.GetComponent<T>();
            inactiveObjectsPool.Add(c);
            return c;
        }
        
        public override T Get(bool createWhenNoneLeft = true)
        {
            T item = null;
            if (inactiveObjectsPool.Count == 0)
            {
                if (createWhenNoneLeft)
                {
                    DevdogLogger.Log("New object created, considering increasing the pool size if this is logged often");
                    item = Instantiate();
                }
            }
            else
            {
                item = inactiveObjectsPool[inactiveObjectsPool.Count - 1];
            }

            Assert.IsNotNull(item, "Couldn't get poolable object from pool!");
            item.gameObject.SetActive(true);
            item.gameObject.transform.localScale = Vector3.one;
            item.gameObject.transform.localRotation = Quaternion.identity;

            activeObjectsList.Add(item);
            inactiveObjectsPool.RemoveAt(inactiveObjectsPool.Count - 1);

            return item;
        }

        public override void Destroy(T item)
        {
            item.gameObject.transform.SetParent(rootObject);
            item.gameObject.SetActive(false); // Up for reuse

            item.gameObject.GetComponents<IPoolable>(_interfaceCache);
            foreach (var c in _interfaceCache)
            {
                c.ResetStateForPool();
            }

            inactiveObjectsPool.Add(item);
            activeObjectsList.Remove(item);
        }
    }
}
