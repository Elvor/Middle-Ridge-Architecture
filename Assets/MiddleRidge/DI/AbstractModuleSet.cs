using MiddleRidge.DI.Impl;
using UnityEngine;
using Component = MiddleRidge.DI.Attribute.Component;

namespace MiddleRidge.DI
{
    public abstract class AbstractModuleSet : MonoBehaviour
    {
        private ModuleContainer moduleContainer;

        protected abstract Component[] GetModuleAttributes();
        
        private void Awake()
        {
            var finder = new ComponentFinder();
            var builder = new ModuleContainerBuilder();
            moduleContainer = builder.Build(finder.FindComponents(GetModuleAttributes()));
            this.moduleContainer.OnCreate();
        }
        
        private void Update() 
        {
            this.moduleContainer.Update(Time.deltaTime);
        }

        private void OnDestroy()
        {
            this.moduleContainer.OnDestroy();
        }

        private void Start()
        {
            moduleContainer.OnStart();
        }
    }
}