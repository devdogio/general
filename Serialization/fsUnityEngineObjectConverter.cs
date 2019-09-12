using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.FullSerializer;
using UnityEngine;

namespace Devdog.General
{
    public class fsUnityEngineObjectConverter : fsConverter
    {

        private bool IsAssetWrapper(Type type)
        {
            if (typeof(IAsset).IsAssignableFrom(type))
            {
                return true;
            }

            return false;
        }

        public override object CreateInstance(fsData data, Type storageType)
        {
            return null;
        }

        public override bool CanProcess(Type type)
        {
            if (IsAssetWrapper(type))
            {
                return true;
            }

//            if (ReflectionUtility.IsBuiltInUnityObjectType(type))
//            {
//                return true;
//            }

            if (JsonSerializer.currentRootType.IsAssignableFrom(type))
            {
//                DevdogLogger.LogWarning("Given type " + type + " is assignable to " + JsonSerializer.currentRootType + " (the current object). This would create an infinite serialization loop. Wrap the variable in Asset<> to avoid this. (serialization ignored)");
                return false;
            }

            if (typeof (UnityEngine.Object).IsAssignableFrom(type))
            {
                return true;
            }

            return false;
        }

        public override bool RequestCycleSupport(Type storageType)
        {
            return false;
        }

        public override bool RequestInheritanceSupport(Type storageType)
        {
            return false;
        }

        protected virtual UnityEngine.Object GetUnityEngineObject(object instance)
        {
            UnityEngine.Object unityObject = null;
            var iAsset = instance as IAsset;
            if (iAsset != null)
            {
                unityObject = iAsset.objectVal;
            }

            if (iAsset == null)
            {
                unityObject = instance as UnityEngine.Object;
            }

            return unityObject;
        }

        public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
        {
            var db = Serializer.Context.Get<List<UnityEngine.Object>>();

            var unityObject = GetUnityEngineObject(instance);
            int index = db.IndexOf(unityObject);
            if (index == -1)
            {
                index = db.Count; // Insert at the end of the list
                db.Add(unityObject);
            }

            serialized = new fsData((long)index);
            return fsResult.Success;
        }

        public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
        {
            try
            {
                var db = Serializer.Context.Get<List<UnityEngine.Object>>();
                if (data.IsNull == false)
                {
                    int index = (int)data.AsInt64;
                    if (index == -1 || index >= db.Count)
                    {
                        DevdogLogger.LogError("Couldn't deserialize UnityEngine.Object : " + instance + " - not found in database. (index: " + index + ")");
                        return fsResult.Fail("Index out of range " + index);
                    }

                    if (IsAssetWrapper(storageType))
                    {
                        var def = typeof(Asset<>);
                        var t = def.MakeGenericType(storageType.GetGenericArguments()[0]);

                        var inst = (IAsset)Activator.CreateInstance(t);
                        inst.objectVal = db[index];
                        instance = inst;
                    }
                    else if (typeof(UnityEngine.Object).IsAssignableFrom(storageType))
                    {
                        instance = db[index];
                    }
                }
                else
                {
                    instance = null;
                }
            }
            catch (Exception e)
            {
                DevdogLogger.LogError(e.Message + "\n" + e.StackTrace);
                return fsResult.Fail(e.Message);
            }

            return fsResult.Success;
        }
    }
}
