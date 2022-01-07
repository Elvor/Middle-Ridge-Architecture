using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MiddleRidge.DI.Attribute;
using MiddleRidge.DI.Lifecycle;
using UniRx;
using UnityEngine;

namespace MiddleRidge.DI.Impl
{
    public class ModuleContainer : IUpdatable
    {
        private readonly ISet<IUpdatable> disabledUpdateableObjects = new HashSet<IUpdatable>();
        private readonly ISet<IUpdatable> enabledUpdateableObjects = new HashSet<IUpdatable>();

        private readonly SortedList<int, IList<ILifeCycleManaged>> lifeCycleManaged =
            new SortedList<int, IList<ILifeCycleManaged>>();

        private readonly IDictionary<object, int> moduleObjByOrder = new Dictionary<object, int>();

        private readonly IList<object> moduleObjects = new List<object>();
        private readonly IList<IDisposable> stateChangeSubs = new List<IDisposable>();
        private readonly ISet<IUpdatable> switchableToDisable = new HashSet<IUpdatable>();
        private readonly ISet<IUpdatable> switchableToEnable = new HashSet<IUpdatable>();
        private readonly IList<IUpdatable> updateableTemporaryStorage = new List<IUpdatable>();

        public ModuleContainer(ICollection<ObjectWithInfo> objects)
        {
            foreach (var objectWithInfo in objects)
            {
                HandleModuleObject(objectWithInfo);
                if (objectWithInfo.Obj is MonoBehaviour view) HandleUnityView(view);
                if (objectWithInfo.Obj is ILifeCycleManaged managed)
                    HandleLifecycleManaged(managed, objectWithInfo.HandleOrder);
            }

            PostprocessUpdateable();
        }

        public void OnCreate()
        {
            foreach (var pair in lifeCycleManaged)
            foreach (var managed in pair.Value)
                managed.OnCreate();
        }

        public void OnStart()
        {
            foreach (var pair in lifeCycleManaged)
            foreach (var managed in pair.Value)
                managed.OnStart();
        }

        public void OnDestroy()
        {
            foreach (var dep in lifeCycleManaged.SelectMany(pair => pair.Value)) dep.OnDestroy();
            foreach (var sub in stateChangeSubs) sub.Dispose();
            stateChangeSubs.Clear();
        }

        public void Update(float deltaTime)
        {
            foreach (var dep in enabledUpdateableObjects) dep.Update(deltaTime);
            enabledUpdateableObjects.ExceptWith(switchableToDisable);
            enabledUpdateableObjects.UnionWith(switchableToEnable);
            disabledUpdateableObjects.ExceptWith(switchableToEnable);
            disabledUpdateableObjects.UnionWith(switchableToDisable);
            switchableToDisable.Clear();
            switchableToEnable.Clear();
        }

        private void HandleUnityView(MonoBehaviour unityView)
        {
            if (!HasBindViewsAttribute(unityView))
            {
                return;
            }
            var fields = unityView.GetType()
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var field in fields)
            {
                var attrs = System.Attribute.GetCustomAttributes(field, typeof(BindSubView));
                if (attrs.Length == 0) continue;

                if (attrs.Length > 1) throw new DuplicatedAttributeException(typeof(BindSubView), field);

                var path = ((BindSubView) attrs[0]).SubViewPath;
                field.SetValue(unityView, unityView.transform.Find(path).GetComponent(field.FieldType));
            }
        }
        
        private bool HasBindViewsAttribute(MonoBehaviour obj) {
            var attrs = System.Attribute.GetCustomAttributes(obj.GetType());
            return attrs.Any(a => a is BindViews);
        }

        private void HandleModuleObject(ObjectWithInfo objectWithInfo)
        {
            var order = objectWithInfo.HandleOrder;
            if (moduleObjByOrder.TryGetValue(objectWithInfo.Obj, out var o))
            {
                if (order != o)
                    throw new Exception(
                        $"different handle orders {o} and {order} for same object {objectWithInfo.Obj.GetType().FullName}");
                return;
            }

            moduleObjects.Add(objectWithInfo.Obj);
            moduleObjByOrder.Add(objectWithInfo.Obj, order);
        }

        private void HandleLifecycleManaged(ILifeCycleManaged managed, int handleOrder)
        {
            lifeCycleManaged.TryGetValue(handleOrder, out var list);
            if (list == null)
            {
                list = new List<ILifeCycleManaged>();
                lifeCycleManaged.Add(handleOrder, list);
            }

            list.Add(managed);

            HandleUpdateable(managed);
        }

        private void HandleUpdateable(ILifeCycleManaged managed)
        {
            if (managed is IUpdatable updateable)
            {
                if (updateable is ISwitchable switchable)
                {
                    stateChangeSubs.Add(switchable.OnChangeState().Subscribe(isActive =>
                    {
                        if (isActive)
                        {
                            switchableToDisable.Remove(updateable);
                            switchableToEnable.Add(updateable);
                        }
                        else
                        {
                            switchableToEnable.Remove(updateable);
                            switchableToDisable.Add(updateable);
                        }
                    }));
                    updateableTemporaryStorage.Add(updateable);
                }
                else
                {
                    enabledUpdateableObjects.Add(updateable);
                }
            }
        }

        public void PostprocessUpdateable()
        {
            foreach (var updateable in updateableTemporaryStorage)
            {
                var switchable = updateable as ISwitchable;
                if (switchable == null)
                {
                    Debug.LogWarning($"{updateable.GetType().Name} Is not switchable");
                    continue;
                }

                var targetList = switchable.IsActive ? enabledUpdateableObjects : disabledUpdateableObjects;
                targetList.Add(updateable);
            }
        }
    }
}