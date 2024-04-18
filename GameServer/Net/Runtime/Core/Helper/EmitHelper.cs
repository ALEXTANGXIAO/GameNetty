﻿using System;
using System.Reflection.Emit;

namespace GameServer
{
    /// <summary>
    /// Emit帮助类。
    /// </summary>
    public static class EmitHelper
    {
        /// <summary>
        /// 创建默认构造函数。
        /// </summary>
        /// <typeparam name="T">构造的实例类型。</typeparam>
        /// <returns>构造实例。</returns>
        public static Func<T> CreateDefaultConstructor<T>()
        {
            var type = typeof(T);
            var dynamicMethod = new DynamicMethod($"CreateInstance_{type.Name}", type, Type.EmptyTypes, true);
            var il = dynamicMethod.GetILGenerator();
            il.Emit(OpCodes.Newobj, type.GetConstructor(Type.EmptyTypes));
            il.Emit(OpCodes.Ret);
            return (Func<T>)dynamicMethod.CreateDelegate(typeof(Func<T>));
        }
    }
}