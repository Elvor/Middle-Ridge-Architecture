using System;
using System.Collections.Generic;

namespace MiddleRidge.DI.Impl
{
    public class ModuleContainerBuilder
    {
        public ModuleContainer Build(ICollection<ComponentData> components)
        {
            var graph = new DependencyGraph(components);
            var moduleContainer = new ModuleContainer(graph.Build());
            return moduleContainer;
        }

        private class DependencyGraph
        {
            // private Dictionary<Type, Dictionary<string, List<Node>>> dependencyIndex;
            private readonly Dictionary<Type, Dictionary<string, HashSet<Node>>> componentIndex =
                new Dictionary<Type, Dictionary<string, HashSet<Node>>>();

            private readonly IList<Node> nodes;

            public DependencyGraph(ICollection<ComponentData> components)
            {
                nodes = FillIndex(components);
            }

            public ICollection<ObjectWithInfo> Build()
            {
                var result = new List<ObjectWithInfo>(nodes.Count);
                foreach (var node in nodes)
                {
                    result.Add(node.IsFinalized() ? node.Obj : ResolveDependencies(node));
                }

                return result;
            }

            private ObjectWithInfo ResolveDependencies(Node startNode)
            {
                var stack = new LinkedList<Node>();
                //for prevention of circular dependency
                var set = new HashSet<Node> {startNode};
                stack.AddLast(startNode);

                while (stack.Count > 0)
                {
                    //
                    var node = stack.Last.Value;
                    if (node.DependenciesManager.Unresolved == 0)
                    {
                        stack.RemoveLast();
                        set.Remove(node);
                        if (!node.IsFinalized())
                        {
                            node.CreateObject();
                        }
                        continue;
                    }
                    
                    foreach (var dependencyInfo in node.DependenciesManager.GetUnresolved())
                    {
                        var depNode = TryToFindInIndex(dependencyInfo);
                        if (set.Contains(depNode))
                            throw new ModuleContainerBuildException(
                                $"Circular dependency {startNode.ComponentData.Type.Name}:{startNode.ComponentData.Name}");
                        if (depNode.IsFinalized())
                        {
                            node.DependenciesManager.SetDependency(dependencyInfo, depNode.Obj);
                        }
                        else
                        {
                            stack.AddLast(depNode);
                            set.Add(depNode);
                        }
                    }
                }

                return startNode.Obj;
            }

            private Node TryToFindInIndex(DependencyInfo dependencyInfo)
            {
                componentIndex.TryGetValue(dependencyInfo.Type, out var byName);
                if (byName == null)
                    throw new ModuleContainerBuildException(
                        $"No dependency in index for {dependencyInfo.Type.Name}:{dependencyInfo.Name}");
                byName.TryGetValue(dependencyInfo.Name, out var existing);
                if (existing == null)
                    throw new ModuleContainerBuildException(
                        $"No dependency in index for {dependencyInfo.Type.Name}:{dependencyInfo.Name}");
                if (existing.Count > 1)
                    throw new ModuleContainerBuildException(
                        $"Multiple dependencies found for {dependencyInfo.Type.Name}:{dependencyInfo.Name}");

                using var n = existing.GetEnumerator();
                n.MoveNext();
                return n.Current;
            }

            private IList<Node> FillIndex(ICollection<ComponentData> components)
            {
                var result = new List<Node>(components.Count);
                foreach (var component in components)
                {
                    var node = new Node(component);
                    result.Add(node);
                    var currType = component.Type;
                    while (currType != null && currType != typeof(object))
                    {
                        AddToIndex(currType, node);
                        foreach (var iface in currType.GetInterfaces()) AddToIndex(iface, node);
                        currType = currType.BaseType;
                    }
                }

                return result;
            }

            private void AddToIndex(Type type, Node node)
            {
                componentIndex.TryGetValue(type, out var byName);
                if (byName == null)
                {
                    byName = new Dictionary<string, HashSet<Node>>();
                    componentIndex[type] = byName;
                }

                byName.TryGetValue(node.ComponentData.Name, out var set);
                if (set == null)
                {
                    set = new HashSet<Node>();
                    byName[node.ComponentData.Name] = set;
                }

                set.Add(node);
            }
        }

        private class Node
        {
            public Node(ComponentData componentData)
            {
                ComponentData = componentData;
                DependenciesManager = componentData.GetDependenciesManagerFactory().Create();
            }

            public DependenciesManager DependenciesManager { get; }

            public ComponentData ComponentData { get; }

            public ObjectWithInfo Obj { get; private set; }

            public ObjectWithInfo CreateObject()
            {
                if (IsFinalized())
                    throw new ModuleContainerBuildException(
                        $"Node {ComponentData.Type.Name}:{ComponentData.Name} is finalized");
                Obj = new ObjectWithInfo(ComponentData.GetObject(DependenciesManager),
                    ComponentData.HandleOrder);
                return Obj;
            }

            public bool IsFinalized()
            {
                return Obj != null;
            }
        }
    }

    public class ObjectWithInfo
    {
        public ObjectWithInfo(object obj, int handleOrder)
        {
            Obj = obj;
            HandleOrder = handleOrder;
        }

        public object Obj { get; }
        public int HandleOrder { get; }
    }
}