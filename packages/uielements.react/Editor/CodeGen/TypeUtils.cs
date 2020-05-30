using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;
using UnityReactUIElements;
using MethodAttributes = Mono.Cecil.MethodAttributes;

namespace Unity.ReactUIElements.JsStructBinding.CodeGen
{
    internal static class TypeUtils
    {
        public static bool ContainsBindingAttribute(IEnumerable<CustomAttribute> attributes)
        {
            return attributes.Any(IsBindingAttribute);
        }

        public static bool IsBindingAttribute(CustomAttribute attribute)
        {
            return attribute.AttributeType.Name == nameof(JSBindingAttribute) &&
                   attribute.AttributeType.Namespace == "UnityReactUIElements";
        }

        public static bool IsAssemblyBindingAttribute(CustomAttribute attribute)
        {
            return attribute.AttributeType.Name == nameof(JSTypeBindingAttribute) &&
                   attribute.AttributeType.Namespace == "UnityReactUIElements";
        }

        public static bool IsSystemBindingAttribute(CustomAttribute attribute)
        {
            return attribute.AttributeType.Name == nameof(JSBindingSystemAttribute) &&
                   attribute.AttributeType.Namespace == "UnityReactUIElements";
        }

        public static MethodInfo GetMethod(this Type type, string name, Type[] arguments, Type returnType)
        {
            var methods = type.GetMethods()
                .Where(e => e.Name == name && e.ReturnType == returnType);

            if (methods.Count() == 1)
            {
                return methods.Single();
            }

            foreach (var method in methods)
            {
                var parameters = method.GetParameters();

                if (!parameters.Any() && arguments.Length == 0) return method;

                if (parameters.Length != arguments.Length) continue;

                for (var i = 0; i < arguments.Length; i++)
                {
                    if (arguments[i] != parameters[i].ParameterType) continue;
                }

                return method;
            }

            throw new InvalidOperationException($"No method {name} found on {type.Name}");
        }

        public static GenericInstanceMethod SetGenericParameter(this MethodReference definition, TypeReference type)
        {
            var instance = new GenericInstanceMethod(definition);
            instance.GenericArguments.Add(type);

            return instance;
        }

        public static bool IsSameType(TypeReference ref1, TypeReference ref2)
        {
            return ref1.Namespace == ref2.Namespace && ref1.Name == ref2.Name;
        }

        public static void AddEmptyConstructor(TypeDefinition type, ModuleDefinition module, MethodReference baseEmptyConstructor)
        {
            var methodAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;
            var method = new MethodDefinition(".ctor", methodAttributes, module.TypeSystem.Void);
            method.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
            method.Body.Instructions.Add(Instruction.Create(OpCodes.Call, baseEmptyConstructor));
            method.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
            type.Methods.Add(method);
        }
    }
}
