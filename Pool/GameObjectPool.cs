using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Devdog.General
{
    public class GameObjectPool : PoolBase<GameObject>
    {

        private static List<IPoolable> _interfaceCache = new List<IPoolable>();

        public GameObjectPool(GameObject prefab, int startSize = 32)
            : base(prefab, startSize)
        {
            
        }

        public override GameObject Instantiate()
        {
            var obj = UnityEngine.Object.Instantiate<GameObject>(baseObject);

            obj.transform.SetParent(rootObject);
            obj.gameObject.SetActive(false); // Start disabled
            
            inactiveObjectsPool.Add(obj);
            return obj;
        }

        public override GameObject Get(bool createWhenNoneLeft = true)
        {
            GameObject obj = null;
            if (inactiveObjectsPool.Count == 0)
            {
                if (createWhenNoneLeft)
                {
                    DevdogLogger.Log("New object created, considering increasing the pool size if this is logged often");
                    obj = Instantiate();
                }
            }
            else
            {
                obj = inactiveObjectsPool[inactiveObjectsPool.Count - 1];
            }

            Assert.IsNotNull(obj, "Couldn't get poolable object from pool!");
            obj.gameObject.SetActive(true);
            obj.gameObject.transform.localScale = Vector3.one;
            obj.gameObject.transform.localRotation = Quaternion.identity;

            activeObjectsList.Add(obj);
            inactiveObjectsPool.RemoveAt(inactiveObjectsPool.Count - 1);

            return obj;
        }

        public override void Destroy(GameObject obj)
        {
            obj.transform.SetParent(rootObject);
            obj.SetActive(false); // Up for reuse

            obj.GetComponents<IPoolable>(_interfaceCache);
            foreach (var component in _interfaceCache)
            {
                component.ResetStateForPool();
            }

            inactiveObjectsPool.Add(obj);
            activeObjectsList.Remove(obj);
        }
    }
}
