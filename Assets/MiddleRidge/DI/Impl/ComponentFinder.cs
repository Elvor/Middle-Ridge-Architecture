using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MiddleRidge.DI.Attribute;

namespace MiddleRidge.DI.Impl
{
    public class ComponentFinder
    {
        public ICollection<ComponentData> FindComponents(IEnumerable<Component> componentAttributes)
        {
            var types = Assembly.GetExecutingAssembly().GetTypes();
            var result = new List<ComponentData>();
            var componentTypes = componentAttributes.Select(c => c.GetType());
            foreach (var type in types)
            {
                if (type.Namespace != null && !type.Namespace.StartsWith(nameof(MiddleRidge))) continue;

                var component = HandleAsComponent(type, componentAttributes);
                if (component != null)
                {
                    result.Add(component);
                }
                else
                {
                    var res = HandleAsDependencyProvider(type, componentTypes);
                    if (res != null) result.AddRange(res);
                }
            }

            return result;
        }

        private IEnumerable<ComponentData> HandleAsDependencyProvider(Type type, IEnumerable<Type> componentAttributes)
        {
            var dpAttribute =
                (DependencyProvider) System.Attribute.GetCustomAttribute(type, typeof(DependencyProvider));
            if (dpAttribute == null) return null;

            if (!componentAttributes.Contains(dpAttribute.ComponentType)) return null;

            var constructorInfo = type.GetConstructor(Array.Empty<Type>());
            if (constructorInfo == null)
                throw new DependencySearchException("Failed to find default constructor for" +
                                                    $" dependency provider {type.FullName}");

            var providerObj = constructorInfo.Invoke(Array.Empty<object>());

            var result = new LinkedList<ComponentData>();
            foreach (var methodInfo in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic |
                                                       BindingFlags.Instance))
            {
                var attr = (Provides) System.Attribute.GetCustomAttribute(methodInfo, typeof(Provides));
                if (attr == null) continue;

                result.AddLast(new ProvidedComponentData(attr.HandleOrder, attr.Name, methodInfo.ReturnType, methodInfo,
                    providerObj));
            }

            return result;
        }

        private ComponentData HandleAsComponent(Type type, IEnumerable<Component> componentAttributes)
        {
            foreach (var attribute in componentAttributes)
            {
                var component = (Component) System.Attribute.GetCustomAttribute(type, attribute.GetType());
                if (component == null) continue;
                ConstructorInfo injectionPoint = null;
                foreach (var constructorInfo in type.GetConstructors())
                {
                    if (System.Attribute.GetCustomAttribute(constructorInfo, typeof(InjectionConstructor)) == null)
                        continue;
                    if (injectionPoint != null)
                        throw new DependencySearchException(
                            $"Multiple injection points found for {type.FullName}");
                    injectionPoint = constructorInfo;
                }

                if (injectionPoint == null)
                {
                    var defaultConstructor = type.GetConstructor(Array.Empty<Type>());
                    if (defaultConstructor == null)
                        throw new DependencySearchException($"No injection point found for {type.FullName}");

                    injectionPoint = defaultConstructor;
                }

                return new PrototypeComponentData(component.HandleOrder, component.Name, type, injectionPoint);
            }

            return null;
        }
    }


    public sealed class PrototypeComponentData : ComponentData
    {
        private readonly ConstructorInfo injectionPoint;

        public PrototypeComponentData(int handleOrder, string name, Type type, ConstructorInfo injectionPoint) : base(
            name,
            handleOrder, type, injectionPoint)
        {
            this.injectionPoint = injectionPoint;
        }

        public override object GetObject(DependenciesManager dependenciesManager)
        {
            return injectionPoint.Invoke(dependenciesManager.GetDependencies());
        }
    }

    public sealed class ProvidedComponentData : ComponentData
    {
        private readonly MethodInfo injectionPoint;
        private readonly object providerObject;

        public ProvidedComponentData(int handleOrder, string name, Type type, MethodInfo injectionPoint,
            object providerObject) : base(name, handleOrder, type, injectionPoint)
        {
            this.injectionPoint = injectionPoint;
            this.providerObject = providerObject;
        }

        public override object GetObject(DependenciesManager dependenciesManager)
        {
            return injectionPoint.Invoke(providerObject, dependenciesManager.GetDependencies());
        }
    }

    public abstract class ComponentData
    {
        private readonly MethodBase injectionPoint;

        protected ComponentData(string name, int handleOrder, Type type, MethodBase injectionPoint)
        {
            Name = name ?? "";
            HandleOrder = handleOrder;
            Type = type;
            this.injectionPoint = injectionPoint;
        }

        public Type Type { get; }

        public string Name { get; }

        public int HandleOrder { get; }

        public DependenciesManagerFactory GetDependenciesManagerFactory()
        {
            return new DependenciesManagerFactory(injectionPoint.GetParameters().Select((info, index) =>
            {
                var attribute = (InjectName) System.Attribute.GetCustomAttribute(info, typeof(InjectName));
                return new DependencyInfo(info.ParameterType, index, attribute?.Name);
            }).ToList());
        }

        public abstract object GetObject(DependenciesManager dependenciesManager);
    }

    public class DependencyInfo
    {
        public DependencyInfo(Type type, int order, string name)
        {
            Type = type;
            Order = order;
            Name = name ?? "";
        }

        public Type Type { get; }

        public int Order { get; }

        public string Name { get; }
    }

    public class DependenciesManager
    {
        private readonly Dictionary<DependencyInfo, object> dependencyMap = new Dictionary<DependencyInfo, object>();

        public DependenciesManager(ICollection<DependencyInfo> dependencyInfos)
        {
            DependencyInfos = dependencyInfos;
            Unresolved = dependencyInfos.Count;
        }

        public ICollection<DependencyInfo> DependencyInfos { get; }

        public int Unresolved { get; private set; }

        public void SetDependency(DependencyInfo dependencyInfo, ObjectWithInfo dep)
        {
            dependencyMap.TryGetValue(dependencyInfo, out var prev);
            if (prev == null)
            {
                Unresolved--;
                dependencyMap[dependencyInfo] = dep.Obj;
            }
            else if (!ReferenceEquals(prev, dep))
            {
                throw new ModuleContainerBuildException(
                    $"Overriding {prev.GetType().Name} with {dep.Obj.GetType().Name}");
            }
        }

        public object[] GetDependencies()
        {
            var deps = new object[dependencyMap.Count];
            foreach (var pair in dependencyMap) deps[pair.Key.Order] = pair.Value;

            return deps;
        }

        public ICollection<DependencyInfo> GetUnresolved()
        {
            return DependencyInfos.Where(di => !dependencyMap.ContainsKey(di)).ToList();
        }
    }

    public class DependenciesManagerFactory
    {
        private readonly ICollection<DependencyInfo> dependencies;

        public DependenciesManagerFactory(ICollection<DependencyInfo> dependencies)
        {
            this.dependencies = dependencies;
        }

        public DependenciesManager Create()
        {
            return new DependenciesManager(dependencies);
        }
    }
}