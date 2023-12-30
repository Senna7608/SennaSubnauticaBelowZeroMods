using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BZHelper
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public static class UnityHelper
    {
        public static bool IsRoot(this Transform transform)
        {
            return transform.parent == null ? true : false;
        }

        public static bool IsRoot(this GameObject gameObject)
        {
            return gameObject.transform.parent == null ? true : false;
        }

        public static void PrefabCleaner(this GameObject rootGo, List<Type> componentList, List<string> childList, bool isWhiteList = false)
        {
            if (componentList == null)
            {
                throw new ArgumentException("*** Component list cannot be null!");
            }

            foreach (Component component in rootGo.GetComponentsInChildren<Component>(true))
            {
                Type componentType = component.GetType();

                if (componentType == typeof(Transform))
                    continue;

                if (componentType == typeof(RectTransform))
                    continue;

                bool containsComponent = componentList.Contains(componentType);

                if (isWhiteList)
                {
                    if (containsComponent)
                    {
                        continue;
                    }
                    else
                    {
                        UnityEngine.Object.DestroyImmediate(component);
                    }
                }
                else
                {
                    if (containsComponent)
                    {
                        UnityEngine.Object.DestroyImmediate(component);
                    }
                    else
                    {
                        continue;
                    }
                }
            }

            BZLogger.Debug("PrefabCleaner: components cleared");

            if (childList == null)
                return;

            List<Transform> markedForDestroy = new List<Transform>();

            foreach (Transform transform in rootGo.GetComponentsInChildren<Transform>(true))
            {
                BZLogger.Debug($"Current Transform name: {transform?.name}");

                if (transform == rootGo.transform)
                {
                    continue;
                }

                bool containsTransform = childList.Contains(transform?.name);

                if (isWhiteList)
                {
                    if (containsTransform)
                    {
                        continue;
                    }
                    else
                    {
                        markedForDestroy.Add(transform);
                        BZLogger.Debug($"Whitelist: {transform.name} added to markedForDestroy list.");
                    }
                }
                else
                {
                    if (containsTransform)
                    {
                        markedForDestroy.Add(transform);
                        BZLogger.Debug($"Blacklist: {transform.name} added to markedForDestroy list.");
                    }
                    else
                    {
                        continue;
                    }
                }
            }

            foreach (Transform tr in markedForDestroy)
            {
                UnityEngine.Object.DestroyImmediate(tr?.gameObject);
            }
        }

        public static void CleanObject(this GameObject gameObject)
        {
            foreach (Component component in gameObject.GetComponents<Component>())
            {
                Type componentType = component.GetType();

                if (componentType == typeof(Transform))
                    continue;
                if (componentType == typeof(Renderer))
                    continue;
                if (componentType == typeof(Mesh))
                    continue;
                if (componentType == typeof(Shader))
                    continue;

                UnityEngine.Object.Destroy(component);
            }
        }

        public static bool TryGetComponentInChildren<T>(this GameObject gameObject, out T component) where T : Component
        {
            foreach (Component _component in gameObject.GetComponentsInChildren<Component>(true))
            {
                if (_component.GetType() == typeof(T))
                {
                    component = _component as T;
                    return true;
                }
            }

            component = null;
            return false;
        }
        
        public static string GetUeObjectShortType(this UnityEngine.Object ueObject)
        {
            return ueObject.GetType().ToString().Split('.').GetLast();
        }

        public static string GetPath(this Transform current)
        {
            if (current.parent == null)
                return current.name;
            return current.parent.GetPath() + "/" + current.name;
        }
               
        /*
        public static GameObject GetCreatureRoot(GameObject go)
        {
            EntityTag entityTag = go.GetComponent<EntityTag>();
            
            if (entityTag == null)
            {
                entityTag = go.GetComponentInParent<EntityTag>();
            }
            if (entityTag != null)
            {
                //if (entityTag.gameObject.GetComponent<LiveMixin>() != null)
                //{
                    return entityTag.gameObject;
                //}
            }

            return null;
        }
        */

        public static TechTag GetTechTag(GameObject go)
        {
            TechTag techTag = go.GetComponent<TechTag>();

            if (techTag == null)
            {
                techTag = go.GetComponentInParent<TechTag>();
            }
            if (techTag != null)
            {
                return techTag;
            }

            return null;
        }

        public static GameObject GetEntityRoot(this GameObject go)
        {
            TechTag techTag = go.GetComponent<TechTag>();

            if (techTag == null)
            {
                techTag = go.GetComponentInParent<TechTag>();
            }
            if (techTag != null)
            {
                return techTag.gameObject;
            }

            return null;
        }        

        public static bool TryGetValue<T>(this List<T> list, T getvalue, out T result) where T : List<T>
        {
            foreach (T item in list)
            {
                if (item.Equals(getvalue))
                {
                    result = item;
                    return true;
                }
            }

            result = null;
            return false;
        }

        public static GameObject FindChild(this GameObject parent, string name)
        {
            for (int i = 0; i < parent.transform.childCount; i++)
            {
                Transform child = parent.transform.GetChild(i);

                if (child.name == name)
                {
                    return child.gameObject;
                }
            }
            return null;
        }

        public static GameObject FindDeepChild(this Transform parent, string childName)
        {
            Queue<Transform> queue = new Queue<Transform>();

            queue.Enqueue(parent);

            while (queue.Count > 0)
            {
                var c = queue.Dequeue();

                if (c.name == childName)
                {
                    return c.gameObject;
                }

                foreach (Transform t in c)
                {
                    queue.Enqueue(t);
                }
            }
            return null;
        }

        public static GameObject FindDeepChild(this GameObject parent, string childName)
        {
            Queue<Transform> queue = new Queue<Transform>();

            queue.Enqueue(parent.transform);

            while (queue.Count > 0)
            {
                var c = queue.Dequeue();

                if (c.name == childName)
                {
                    return c.gameObject;
                }

                foreach (Transform t in c)
                {
                    queue.Enqueue(t);
                }
            }
            return null;
        }

        public static T GetObjectClone<T>(this T uEobject) where T : UnityEngine.Object
        {
            return UnityEngine.Object.Instantiate(uEobject);
        }

        public static T GetComponentClone<T>(this T uEcomponent, Transform newParent) where T : Component
        {
            T clone = UnityEngine.Object.Instantiate(uEcomponent);
            clone.transform.SetParent(newParent, false);
            Utils.ZeroTransform(clone.transform);
            return clone;
        }

        public static GameObject GetPrefabClone(this GameObject prefab)
        {
            bool isActive = prefab.activeSelf;

            if (isActive)
            {
                prefab.SetActive(false);
            }

            GameObject clone = UnityEngine.Object.Instantiate(prefab);
            Utils.ZeroTransform(clone.transform);
            return clone;
        }

        public static GameObject GetPrefabClone(this GameObject prefab, Transform newParent)
        {
            bool isActive = prefab.activeSelf;

            if (isActive)
            {
                prefab.SetActive(false);
            }

            GameObject clone = UnityEngine.Object.Instantiate(prefab);
            clone.transform.SetParent(newParent, false);
            Utils.ZeroTransform(clone.transform);
            prefab.SetActive(isActive);
            return clone;
        }

        public static GameObject GetPrefabClone(this GameObject prefab, Transform newParent, bool setActive)
        {
            bool isActive = prefab.activeSelf;

            if (isActive)
            {
                prefab.SetActive(false);
            }

            GameObject clone = UnityEngine.Object.Instantiate(prefab);
            clone.SetActive(setActive);
            clone.transform.SetParent(newParent, false);
            Utils.ZeroTransform(clone.transform);
            prefab.SetActive(isActive);
            return clone;
        }

        public static GameObject GetPrefabClone(this GameObject prefab, Transform newParent, bool setActive, string newName)
        {
            bool isActive = prefab.activeSelf;

            if (isActive)
            {
                prefab.SetActive(false);
            }

            GameObject clone = UnityEngine.Object.Instantiate(prefab);
            clone.SetActive(setActive);
            clone.transform.SetParent(newParent, false);
            clone.name = newName;
            Utils.ZeroTransform(clone.transform);
            prefab.SetActive(isActive);
            return clone;
        }

        public static IEnumerator GetModelCloneFromPrefabAsync(TechType techType, string model, Transform newParent, bool setActive, string newName, IOut<GameObject> result)
        {
            CoroutineTask<GameObject> task = CraftData.GetPrefabForTechTypeAsync(techType);
            yield return task;

            GameObject clone = null;

            try
            {
                clone = UnityEngine.Object.Instantiate(FindDeepChild(task.GetResult(), model));
            }
            catch
            {
                result.Set(null);
                yield break;
            }

            clone.SetActive(setActive);
            clone.transform.SetParent(newParent, false);
            clone.name = newName;
            Utils.ZeroTransform(clone.transform);
            result.Set(clone);
            yield break;
        }

        public static GameObject CreateGameObject(string name)
        {
            GameObject newObject = new GameObject(name);
            Utils.ZeroTransform(newObject.transform);
            return newObject;
        }

        public static GameObject CreateGameObject(string name, Transform parent)
        {
            GameObject newObject = new GameObject(name);
            newObject.transform.SetParent(parent, false);
            Utils.ZeroTransform(newObject.transform);
            return newObject;
        }

        public static GameObject CreateGameObject(string name, Transform parent, bool active)
        {
            GameObject newObject = new GameObject(name);
            newObject.transform.SetParent(parent, false);
            Utils.ZeroTransform(newObject.transform);
            newObject.SetActive(active);
            return newObject;
        }

        public static GameObject CreateGameObject(PrimitiveType primitiveType, string name, Transform parent)
        {
            GameObject newObject = GameObject.CreatePrimitive(primitiveType);
            newObject.name = name;
            newObject.transform.SetParent(parent, false);
            return newObject;
        }

        public static GameObject CreateGameObject(string name, Transform parent, Vector3 localPos)
        {
            GameObject newObject = new GameObject(name);
            newObject.transform.SetParent(parent, false);
            Utils.ZeroTransform(newObject.transform);
            newObject.transform.localPosition = localPos;
            return newObject;
        }

        public static GameObject CreateGameObject(string name, Transform parent, Vector3 localPos, PrimitiveType primitiveType)
        {
            GameObject newObject = GameObject.CreatePrimitive(primitiveType);
            newObject.name = name;
            newObject.transform.SetParent(parent, false);
            newObject.transform.localPosition = localPos;
            return newObject;
        }

        public static GameObject CreateGameObject(string name, Transform parent, Vector3 localPos, Vector3 localRot)
        {
            GameObject newObject = new GameObject(name);
            newObject.transform.SetParent(parent, false);
            newObject.transform.localPosition = localPos;
            newObject.transform.localRotation = Quaternion.Euler(localRot);
            return newObject;
        }

        public static GameObject CreateGameObject(string name, Transform parent, Vector3 localPos, Vector3 localRot, Vector3 localScale)
        {
            GameObject newObject = new GameObject(name);
            newObject.transform.SetParent(parent, false);
            newObject.transform.localPosition = localPos;
            newObject.transform.localRotation = Quaternion.Euler(localRot);
            newObject.transform.localScale = localScale;
            return newObject;
        }

        public static GameObject CreateGameObject(string name, Transform parent, Vector3 localPos, Vector3 localRot, PrimitiveType primitiveType)
        {
            GameObject newObject = GameObject.CreatePrimitive(primitiveType);
            newObject.name = name;
            newObject.transform.SetParent(parent, false);
            newObject.transform.localPosition = localPos;
            newObject.transform.localRotation = Quaternion.Euler(localRot);
            return newObject;
        }

        public static GameObject CreateGameObject(string name, Transform parent, Vector3 localPos, Vector3 localRot, Vector3 localScale, int layer)
        {
            GameObject newObject = new GameObject(name);
            newObject.transform.SetParent(parent, false);
            newObject.transform.localPosition = localPos;
            newObject.transform.localRotation = Quaternion.Euler(localRot);
            newObject.transform.localScale = localScale;
            newObject.layer = layer;
            return newObject;
        }






















        public static GameObject GetRootGameObject(string sceneName, string startsWith)
        {
            Scene scene;

            GameObject[] rootObjects;

            try
            {
                scene = SceneManager.GetSceneByName(sceneName);
            }
            catch
            {
                return null;
            }

            rootObjects = scene.GetRootGameObjects();

            foreach (GameObject gameObject in rootObjects)
            {
                if (gameObject.name.StartsWith(startsWith))
                {
                    return gameObject;
                }
            }
            return null;
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
