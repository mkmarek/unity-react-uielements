using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using UnityReactUIElements;

namespace Unity.ReactUIElements.JsStructBinding.CodeGen
{
    internal static class ReactComponentQuerySystemProcessor
    {
        public static void Process(
            string namespaceName,
            AssemblyDefinition definition,
            IEnumerable<TypeReference> supportedTypes)
        {
            definition.MainModule.Types.Add(CreateJobDefinition(namespaceName, definition.MainModule, supportedTypes));
        }

        private static TypeDefinition CreateJobDefinition(
            string namespaceName,
            ModuleDefinition mainModule,
            IEnumerable<TypeReference> supportedTypes)
        {
            var baseType = mainModule.ImportReference(typeof(ReactComponentQuerySystem));
            var baseTypeCtor = mainModule.ImportReference(typeof(object).GetConstructor(Type.EmptyTypes));

            var job = new TypeDefinition(
                namespaceName,
                "GeneratedReactComponentQuerySystem",
                TypeAttributes.Class,
                baseType);

            job.Methods.Add(CreateScheduleJobForComponentMethod(supportedTypes, mainModule));

            TypeUtils.AddEmptyConstructor(job, mainModule, baseTypeCtor);

            return job;
        }

        private static MethodDefinition CreateScheduleJobForComponentMethod(IEnumerable<TypeReference> supportedTypes, ModuleDefinition mainModule)
        {
            unsafe
            {
                var jobHandleType = mainModule.ImportReference(typeof(JobHandle));
                var queryDataType = mainModule.ImportReference(typeof(ReactComponentQuerySystem.QueryData));
                var queryDataPtrField = mainModule.ImportReference(
                    typeof(ReactComponentQuerySystem.QueryData).GetField(nameof(ReactComponentQuerySystem.QueryData
                        .DataPtr)));
                var queryDataSlotSizeField = mainModule.ImportReference(
                    typeof(ReactComponentQuerySystem.QueryData).GetField(nameof(ReactComponentQuerySystem.QueryData
                        .SlotSize)));
                var queryDataOffsetMapField = mainModule.ImportReference(
                    typeof(ReactComponentQuerySystem.QueryData).GetField(nameof(ReactComponentQuerySystem.QueryData
                        .OffsetMap)));
                var queryDataQueryField = mainModule.ImportReference(
                    typeof(ReactComponentQuerySystem.QueryData).GetField(nameof(ReactComponentQuerySystem.QueryData
                        .Query)));
                var jobTypeGenericDefinition = mainModule.ImportReference(typeof(CopyComponentDataToSlots<>));
                var getDictionaryItemMethod =
                    mainModule.ImportReference(
                        typeof(Dictionary<string, int>).GetMethod("get_Item", new[] {typeof(string)}));
                var scheduleMethod =
                    mainModule.ImportReference(
                        typeof(JobChunkExtensions).GetMethod(nameof(JobChunkExtensions.Schedule)));
                var getArchetypeChunkComponentTypeMethod = mainModule.ImportReference(typeof(ComponentSystemBase)
                    .GetMethod(nameof(ComponentSystemBase.GetArchetypeChunkComponentType)));

                var stringEqualityMethod =
                    mainModule.ImportReference(typeof(string).GetMethod("op_Equality",
                        new[] {typeof(string), typeof(string)}));

                var method = new MethodDefinition(
                    "ScheduleJobForComponent",
                    MethodAttributes.Virtual,
                    jobHandleType);

                method.Parameters.Add(new ParameterDefinition(queryDataType));
                method.Parameters.Add(new ParameterDefinition(mainModule.TypeSystem.String));
                method.Parameters.Add(new ParameterDefinition(jobHandleType));

                var nameComparisonResult = new VariableDefinition(mainModule.TypeSystem.Boolean);

                method.Body.Variables.Add(nameComparisonResult);

                var ilProcessor = method.Body.GetILProcessor();

                foreach (var type in supportedTypes)
                {
                    var dotnetType = Type.GetType(type.FullName + ", " + type.Module.Assembly.FullName);

                    if (!typeof(IComponentData).IsAssignableFrom(dotnetType)) continue;

                    var jobType = new GenericInstanceType(jobTypeGenericDefinition);
                    jobType.GenericArguments.Add(type);

                    var jobComponentTypeField = mainModule.ImportReference(jobTypeGenericDefinition.Resolve().Fields
                        .SingleOrDefault(e => e.Name == "ComponentType"));
                    jobComponentTypeField.DeclaringType = jobType;

                    var jobTypePtrField = new FieldReference("DataPtr", mainModule.TypeSystem.Void.MakePointerType(), jobType);
                    var jobTypeStepSizeField = new FieldReference("StepSize", mainModule.TypeSystem.Int32, jobType);
                    var jobTypeSlotOffsetField = new FieldReference("SlotOffset", mainModule.TypeSystem.Int32, jobType);

                    var job = new VariableDefinition(jobType);
                    method.Body.Variables.Add(job);

                    var genericScheduleMethod = scheduleMethod.SetGenericParameter(jobType);
                    var getArchetypeChunkComponentTypeGenericMethod =
                        getArchetypeChunkComponentTypeMethod.SetGenericParameter(type);

                    var nopAfterReturn = Instruction.Create(OpCodes.Nop);

                    // if (componentType == "<component_name>")
                    ilProcessor.Emit(OpCodes.Ldarg_2);
                    ilProcessor.Emit(OpCodes.Ldstr, type.Name);
                    ilProcessor.Emit(OpCodes.Call, stringEqualityMethod);
                    ilProcessor.Emit(OpCodes.Stloc_S, nameComparisonResult);
                    ilProcessor.Emit(OpCodes.Ldloc_S, nameComparisonResult);
                    ilProcessor.Emit(OpCodes.Brfalse, nopAfterReturn);
                    ilProcessor.Emit(OpCodes.Nop);

                    // var job = new CopyComponentDataToSlots<component_name>()
                    ilProcessor.Emit(OpCodes.Ldloca_S, job);
                    ilProcessor.Emit(OpCodes.Initobj, jobType);

                    // job.DataPtr = queryRef.DataPtr,
                    ilProcessor.Emit(OpCodes.Ldloca_S, job);
                    ilProcessor.Emit(OpCodes.Ldarg_1);
                    ilProcessor.Emit(OpCodes.Ldfld, queryDataPtrField);
                    ilProcessor.Emit(OpCodes.Stfld, jobTypePtrField);

                    // job.SlotSize = queryRef.StepSize,
                    ilProcessor.Emit(OpCodes.Ldloca_S, job);
                    ilProcessor.Emit(OpCodes.Ldarg_1);
                    ilProcessor.Emit(OpCodes.Ldfld, queryDataSlotSizeField);
                    ilProcessor.Emit(OpCodes.Stfld, jobTypeStepSizeField);

                    // job.SlotOffset = queryRef.OffsetMap[componentType],
                    ilProcessor.Emit(OpCodes.Ldloca_S, job);
                    ilProcessor.Emit(OpCodes.Ldarg_1);
                    ilProcessor.Emit(OpCodes.Ldfld, queryDataOffsetMapField);
                    ilProcessor.Emit(OpCodes.Ldarg_2);
                    ilProcessor.Emit(OpCodes.Callvirt, getDictionaryItemMethod);
                    ilProcessor.Emit(OpCodes.Stfld, jobTypeSlotOffsetField);

                    //job.ComponentType = GetArchetypeChunkComponentType<component_type>(true);
                    ilProcessor.Emit(OpCodes.Ldloca_S, job);
                    ilProcessor.Emit(OpCodes.Ldarg_0);
                    ilProcessor.Emit(OpCodes.Ldc_I4_1);
                    ilProcessor.Emit(OpCodes.Call, getArchetypeChunkComponentTypeGenericMethod);
                    ilProcessor.Emit(OpCodes.Stfld, jobComponentTypeField);

                    // return job.Schedule(queryRef.Query, inputDeps);
                    ilProcessor.Emit(OpCodes.Ldloc_S, job);
                    ilProcessor.Emit(OpCodes.Ldarg_1);
                    ilProcessor.Emit(OpCodes.Ldfld, queryDataQueryField);
                    ilProcessor.Emit(OpCodes.Ldarg_3);
                    ilProcessor.Emit(OpCodes.Call, genericScheduleMethod);
                    ilProcessor.Emit(OpCodes.Ret);

                    ilProcessor.Append(nopAfterReturn);
                }

                ilProcessor.Emit(OpCodes.Ldarg_3);
                ilProcessor.Emit(OpCodes.Ret);

                return method;
            }
        }
    }
}