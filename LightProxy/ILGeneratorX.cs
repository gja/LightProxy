using System;
using System.Reflection;
using System.Reflection.Emit;

namespace LightProxy
{
    public static class ILGeneratorX
    {
        public static void LoadSelf(this ILGenerator generator)
        {
            generator.Emit(OpCodes.Ldarg_0);
        }

        public static void LoadNull(this ILGenerator generator)
        {
            generator.Emit(OpCodes.Ldnull);
        }

        public static void LoadField(this ILGenerator generator, FieldInfo field)
        {
            generator.LoadSelf();
            generator.Emit(OpCodes.Ldfld, field);
        }

        public static void LoadArgument(this ILGenerator generator, int argNo)
        {
            generator.Emit(OpCodes.Ldarg, argNo);
        }

        public static void LoadInteger(this ILGenerator generator, int val)
        {
            generator.Emit(OpCodes.Ldc_I4, val);
        }

        public static void LoadTemporaryVariable(this ILGenerator generator, int pos)
        {
            generator.Emit(OpCodes.Ldloc, pos);
        }

        public static void Box(this ILGenerator generator, Type type)
        {
            generator.Emit(OpCodes.Box, type);
        }

        public static void MaybeBox(this ILGenerator generator, Type type)
        {
            if(type.IsValueType)
                generator.Emit(OpCodes.Box, type);
        }

        public static void MaybeUnBox(this ILGenerator generator, Type type)
        {
            if (type.IsValueType)
                generator.Emit(OpCodes.Unbox_Any, type);
        }
        
        public static void StashTemporaryVariable(this ILGenerator generator, int pos)
        {
            generator.Emit(OpCodes.Stloc, pos);
        }

        public static void Execute(this ILGenerator generator, MethodInfo execute)
        {
            generator.Emit(OpCodes.Call, execute);
        }
        
        public static void CreateArray(this ILGenerator generator, Type type, int count)
        {
            generator.Emit(OpCodes.Ldc_I4, count);
            generator.Emit(OpCodes.Newarr, typeof(Object));
        }

        public static void Return(this ILGenerator generator)
        {
            generator.Emit(OpCodes.Ret);
        }
    }
}
