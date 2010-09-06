using System;
using System.Reflection;
using System.Reflection.Emit;

namespace LightProxy.Internal
{
    internal static class ILGeneratorX
    {
        internal static void LoadSelf(this ILGenerator generator)
        {
            generator.Emit(OpCodes.Ldarg_0);
        }

        internal static void LoadNull(this ILGenerator generator)
        {
            generator.Emit(OpCodes.Ldnull);
        }

        internal static void LoadField(this ILGenerator generator, FieldInfo field)
        {
            generator.LoadSelf();
            generator.Emit(OpCodes.Ldfld, field);
        }

        internal static void LoadArgument(this ILGenerator generator, int argNo)
        {
            generator.Emit(OpCodes.Ldarg, argNo);
        }

        internal static void LoadInteger(this ILGenerator generator, int val)
        {
            generator.Emit(OpCodes.Ldc_I4, val);
        }

        internal static void LoadTemporaryVariable(this ILGenerator generator, int pos)
        {
            generator.Emit(OpCodes.Ldloc, pos);
        }

        internal static void Box(this ILGenerator generator, Type type)
        {
            generator.Emit(OpCodes.Box, type);
        }

        internal static void MaybeBox(this ILGenerator generator, Type type)
        {
            if(type.IsValueType)
                generator.Emit(OpCodes.Box, type);
        }

        internal static void MaybeUnBox(this ILGenerator generator, Type type)
        {
            if (type.IsValueType)
                generator.Emit(OpCodes.Unbox_Any, type);
        }
        
        internal static void StashTemporaryVariable(this ILGenerator generator, int pos)
        {
            generator.Emit(OpCodes.Stloc, pos);
        }

        internal static void Execute(this ILGenerator generator, MethodInfo execute)
        {
            generator.Emit(OpCodes.Call, execute);
        }
        
        internal static void CreateArray(this ILGenerator generator, Type type, int count)
        {
            generator.Emit(OpCodes.Ldc_I4, count);
            generator.Emit(OpCodes.Newarr, typeof(Object));
        }

        internal static void Pop(this ILGenerator generator)
        {
            generator.Emit(OpCodes.Pop);
        }

        internal static void Return(this ILGenerator generator, Type type)
        {
            if(type == typeof(void))
                generator.Pop();
            else
                generator.MaybeUnBox(type);
            generator.Emit(OpCodes.Ret);
        }
    }
}