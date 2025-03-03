/*
 * Copyright 2023 HashSet Extension for NetSerializer
 * 
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace NetSerializer
{
    sealed class HashSetSerializer : IDynamicTypeSerializer
    {
        public bool Handles(Type type)
        {
            if (!type.IsGenericType)
                return false;

            return type.GetGenericTypeDefinition() == typeof(HashSet<>);
        }

        public IEnumerable<Type> GetSubtypes(Type type)
        {
            // We need uint for size and the element type
            return new[] { typeof(uint), type.GetGenericArguments()[0] };
        }

        public void GenerateWriterMethod(Serializer serializer, Type type, ILGenerator il)
        {
            Type elementType = type.GetGenericArguments()[0];

            // Define labels
            var notNullLabel = il.DefineLabel();

            // Check if value is null
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Brtrue_S, notNullLabel);

            // If value is null, write 0
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldc_I4_0);
            il.Emit(OpCodes.Call, serializer.GetDirectWriter(typeof(uint)));
            il.Emit(OpCodes.Ret);

            // If not null
            il.MarkLabel(notNullLabel);

            // Get Count property
            PropertyInfo countProperty = type.GetProperty("Count");

            // Write count + 1 (to distinguish from null)
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Callvirt, countProperty.GetGetMethod());
            il.Emit(OpCodes.Ldc_I4_1);
            il.Emit(OpCodes.Add);
            il.Emit(OpCodes.Call, serializer.GetDirectWriter(typeof(uint)));

            // Get Enumerator method
            MethodInfo getEnumeratorMethod = type.GetMethod("GetEnumerator");
            Type enumeratorType = getEnumeratorMethod.ReturnType;
            MethodInfo moveNextMethod = enumeratorType.GetMethod("MoveNext");
            PropertyInfo currentProperty = enumeratorType.GetProperty("Current");
            MethodInfo disposeMethod = enumeratorType.GetMethod("Dispose");

            // Define locals
            var enumeratorLocal = il.DeclareLocal(enumeratorType);

            // Get enumerator
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Callvirt, getEnumeratorMethod);
            il.Emit(OpCodes.Stloc, enumeratorLocal);

            // Use try-finally to ensure Dispose is called
            il.BeginExceptionBlock();

            // Loop start label
            var loopCheckLabel = il.DefineLabel();
            var loopBodyLabel = il.DefineLabel();

            // Jump to loop check
            il.Emit(OpCodes.Br_S, loopCheckLabel);

            // Loop body
            il.MarkLabel(loopBodyLabel);

            // Get data for element type
            var data = serializer.GetIndirectData(elementType);

            // If writer needs serializer instance, load it
            if (data.WriterNeedsInstance)
                il.Emit(OpCodes.Ldarg_0);

            // Load stream
            il.Emit(OpCodes.Ldarg_1);

            // Get current element
            il.Emit(OpCodes.Ldloc, enumeratorLocal);
            il.Emit(OpCodes.Callvirt, currentProperty.GetGetMethod());

            // Call writer for element
            il.Emit(OpCodes.Call, data.WriterMethodInfo);

            // Loop check
            il.MarkLabel(loopCheckLabel);

            // Call MoveNext
            il.Emit(OpCodes.Ldloc, enumeratorLocal);
            il.Emit(OpCodes.Callvirt, moveNextMethod);
            il.Emit(OpCodes.Brtrue_S, loopBodyLabel);

            // Finally block
            il.BeginFinallyBlock();

            // Call Dispose
            il.Emit(OpCodes.Ldloc, enumeratorLocal);
            il.Emit(OpCodes.Callvirt, disposeMethod);

            // End try-finally
            il.EndExceptionBlock();

            il.Emit(OpCodes.Ret);
        }

        public void GenerateReaderMethod(Serializer serializer, Type type, ILGenerator il)
        {
            Type elementType = type.GetGenericArguments()[0];

            // Define locals
            var lenLocal = il.DeclareLocal(typeof(uint));
            var setLocal = il.DeclareLocal(type);
            var elementLocal = il.DeclareLocal(elementType);

            // Read length
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldloca_S, lenLocal);
            il.Emit(OpCodes.Call, serializer.GetDirectReader(typeof(uint)));

            // If length == 0, return null
            var notNullLabel = il.DefineLabel();
            il.Emit(OpCodes.Ldloc_S, lenLocal);
            il.Emit(OpCodes.Brtrue_S, notNullLabel);

            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Ldnull);
            il.Emit(OpCodes.Stind_Ref);
            il.Emit(OpCodes.Ret);

            // Create new HashSet instance
            il.MarkLabel(notNullLabel);

            // Create and store the HashSet
            ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
            il.Emit(OpCodes.Newobj, constructor);
            il.Emit(OpCodes.Stloc, setLocal);

            // Store the HashSet to the out parameter early
            // This ensures the object reference is available if needed during element deserialization
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Ldloc, setLocal);
            il.Emit(OpCodes.Stind_Ref);

            // Get Add method
            MethodInfo addMethod = type.GetMethod("Add", new[] { elementType });

            // Decrement length (we added 1 when serializing)
            il.Emit(OpCodes.Ldloc_S, lenLocal);
            il.Emit(OpCodes.Ldc_I4_1);
            il.Emit(OpCodes.Sub);
            il.Emit(OpCodes.Stloc_S, lenLocal);

            // For loop counter
            var counterLocal = il.DeclareLocal(typeof(uint));
            il.Emit(OpCodes.Ldc_I4_0);
            il.Emit(OpCodes.Stloc, counterLocal);

            // Loop labels
            var loopBodyLabel = il.DefineLabel();
            var loopCheckLabel = il.DefineLabel();

            // Jump to loop check
            il.Emit(OpCodes.Br_S, loopCheckLabel);

            // Loop body
            il.MarkLabel(loopBodyLabel);

            // Get data for element type
            var data = serializer.GetIndirectData(elementType);

            // If reader needs serializer instance, load it
            if (data.ReaderNeedsInstance)
                il.Emit(OpCodes.Ldarg_0);

            // Load stream
            il.Emit(OpCodes.Ldarg_1);

            // Load address of element local
            il.Emit(OpCodes.Ldloca_S, elementLocal);

            // Call reader
            il.Emit(OpCodes.Call, data.ReaderMethodInfo);

            // Add element to set
            il.Emit(OpCodes.Ldloc, setLocal);
            il.Emit(OpCodes.Ldloc, elementLocal);
            il.Emit(OpCodes.Callvirt, addMethod);

            // HashSet<T>.Add returns bool, we need to pop it from the stack
            il.Emit(OpCodes.Pop);

            // Increment counter
            il.Emit(OpCodes.Ldloc, counterLocal);
            il.Emit(OpCodes.Ldc_I4_1);
            il.Emit(OpCodes.Add);
            il.Emit(OpCodes.Stloc, counterLocal);

            // Loop check
            il.MarkLabel(loopCheckLabel);
            il.Emit(OpCodes.Ldloc, counterLocal);
            il.Emit(OpCodes.Ldloc, lenLocal);
            il.Emit(OpCodes.Blt_S, loopBodyLabel);

            il.Emit(OpCodes.Ret);
        }
    }
}