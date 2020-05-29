using System;
using System.Runtime.InteropServices;
using ChakraHost.Hosting;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using UnityEngine;

namespace UnityReactUIElements
{
    public static class JsEntityCommandBuffer
    {
        public static void CreateOnGlobal(JavaScriptValue global)
        {
            var createAndExecute = JavaScriptValue.CreateFunction("createAndExecute", CreateAndExecute);
            global.SetProperty(JavaScriptPropertyId.FromString("executeBuffer"), createAndExecute, true);
        }

        private static JavaScriptValue CreateAndExecute(JavaScriptValue callee, bool isconstructcall, JavaScriptValue[] arguments, ushort argumentcount, IntPtr callbackdata)
        {
            var callback = arguments[1];
            var system =
                World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            var buffer = system.CreateCommandBuffer();

            var handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            var ptr = GCHandle.ToIntPtr(handle);

            var jsBuffer = JavaScriptValue.CreateExternalObject(ptr, Finalizer);
            var jsBufferPrototype = JavaScriptValue.CreateObject();
            jsBufferPrototype.SetProperty(
                JavaScriptPropertyId.FromString("createEntity"),
                JavaScriptValue.CreateFunction("createEntity", CreateEntity), true);
            jsBufferPrototype.SetProperty(
                JavaScriptPropertyId.FromString("setComponent"),
                JavaScriptValue.CreateFunction("setComponent", SetComponent), true);
            jsBufferPrototype.SetProperty(
                JavaScriptPropertyId.FromString("destroyEntity"),
                JavaScriptValue.CreateFunction("destroyEntity", DestroyEntity), true);
            jsBuffer.Prototype = jsBufferPrototype;

            try
            {
                callback.CallFunction(JavaScriptValue.Undefined, jsBuffer);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }


            handle.Free();

            return JavaScriptValue.Undefined;
        }

        private static unsafe JavaScriptValue CreateEntity(JavaScriptValue callee, bool isconstructcall, JavaScriptValue[] arguments, ushort argumentcount, IntPtr callbackdata)
        {
            var factory = JSTypeFactories.GetFactory("Entity");
            var commandBuffer = arguments[0].ObjectFromJavaScriptValue<EntityCommandBuffer>();

            var entity = commandBuffer.CreateEntity();
            var ptr = UnsafeUtility.Malloc(UnsafeUtility.SizeOf<Entity>(), UnsafeUtility.AlignOf<Entity>(), Allocator.Persistent);
            UnsafeUtility.CopyStructureToPtr<Entity>(ref entity, ptr);

            return factory.CreateJsObjectForNative(ptr, true);
        }

        private static unsafe JavaScriptValue SetComponent(JavaScriptValue callee, bool isconstructcall, JavaScriptValue[] arguments, ushort argumentcount, IntPtr callbackdata)
        {
            var commandBuffer = arguments[0].ObjectFromJavaScriptValue<EntityCommandBuffer>();
            var componentName = arguments[1].ToString();
            var entityData = (void*)arguments[2].ExternalData;
            var componentData = (void*)arguments[3].ExternalData;

            UnsafeUtility.CopyPtrToStructure<Entity>(entityData, out var entity);

            var factory = JSTypeFactories.GetFactory(componentName);

            commandBuffer.AddComponent(entity, factory.ReadComponentType);
            factory.EntityCommandBufferSetComponent(commandBuffer, entity, componentData);

            return JavaScriptValue.Undefined;
        }
        private static unsafe JavaScriptValue DestroyEntity(JavaScriptValue callee, bool isconstructcall, JavaScriptValue[] arguments, ushort argumentcount, IntPtr callbackdata)
        {
            var commandBuffer = arguments[0].ObjectFromJavaScriptValue<EntityCommandBuffer>();
            var entityData = (void*)arguments[1].ExternalData;

            UnsafeUtility.CopyPtrToStructure<Entity>(entityData, out var entity);

            commandBuffer.DestroyEntity(entity);

            return JavaScriptValue.Undefined;
        }


        private static void Finalizer(IntPtr data)
        {
            // nothing to do here. Will unpin it right after buffer execution
        }
    }
}