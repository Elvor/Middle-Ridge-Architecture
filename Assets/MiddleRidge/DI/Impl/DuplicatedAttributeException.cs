using System;
using System.Reflection;

namespace MiddleRidge.DI.Impl
{
    public class DuplicatedAttributeException : Exception
    {
        public DuplicatedAttributeException(Type attributeType, FieldInfo info) 
            : base($"Duplicated attribute {attributeType.Name} for field {info.Name} in {info.DeclaringType?.Name}") {}
        
        public DuplicatedAttributeException(Type attributeType, MethodInfo info) 
            : base($"Duplicated attribute {attributeType.Name} for method {info.Name} in {info.DeclaringType?.Name}") {}
    }
}