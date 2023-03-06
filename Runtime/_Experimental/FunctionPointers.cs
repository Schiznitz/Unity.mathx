#region Header
// **    Copyright (C) 2023 Nicolas Reinhard, @LTMX. All rights reserved.
// **    Github Profile: https://github.com/LTMX
// **    Repository : https://github.com/LTMX/Unity.Mathematics-Extensions
#endregion

using System;
using System.Runtime.CompilerServices;
using AOT;
using Unity.Burst;
using static Unity.Burst.BurstCompiler;
using static Unity.Mathematics.Operations;


namespace Unity.Mathematics
{
    
    public static class Operations {
        // [BurstCompile]
        // [MonoPInvokeCallback(typeof(p2f))]
        // public static float mul(this float a, float b) => a * b;
        //
        // [BurstCompile]
        // [MonoPInvokeCallback(typeof(p2f))]
        // public static float add(this float a, float b) => a + b;

        // A common interface for both MultiplyFloat and AddFloat methods
        
        
        // operation interfaces
        /// Process 2 float values
        public delegate float p2f1(float a, float b);
        /// Process 2 float values
        public delegate float2 p2f2(float2 a, float2 b);
        /// Process 2 float3 values
        public delegate float3 p2f3(float3 a, float3 b);
        /// Process 2 float4 values
        public delegate float4 p2f4(float4 a, float4 b);
        
        public delegate T p2ft<T>(T a, T b);
        public delegate T delfpt<T>(T a, T b, FunctionPointer<p2f1> func);
        
        // Delegates
        delegate float delf1(float a, float b, FunctionPointer<p2f1> function);
        delegate float2 delf2(float2 a, float2 b, FunctionPointer<p2f1> function);
        delegate float3 delf3(float3 a, float3 b, FunctionPointer<p2f1> function);
        delegate float4 delf4(float4 a, float4 b, FunctionPointer<p2f1> function);
        
        // [BurstCompile] [MonoPInvokeCallback(typeof(delfpt<float>))]
        // static T OP<T>(T f1, T f2, FunctionPointer<p2f1> func) => funct<T>().Invoke(f1,f2, func);
        // static FunctionPointer<delfpt<T>> funct<T>() => CompileFunctionPointer<delfpt<T>>(OP);
        
        // Operation Interfaces
        [BurstCompile] [MonoPInvokeCallback(typeof(float))]
        static float OP(float f1, float f2, FunctionPointer<p2f1> func) => func.Invoke(f1, f2);
        [BurstCompile] [MonoPInvokeCallback(typeof(delfpt<float2>))]
        static float2 OP(float2 f1, float2 f2, FunctionPointer<p2f1> func) => new(func.Invoke(f1.x, f2.x), func.Invoke(f1.y, f2.y));
        [BurstCompile] [MonoPInvokeCallback(typeof(delfpt<float3>))]
        static float3 OP(float3 f1, float3 f2, FunctionPointer<p2f1> func) => new(func.Invoke(f1.x, f2.x), func.Invoke(f1.y, f2.y), func.Invoke(f1.z, f2.z));
        [BurstCompile] [MonoPInvokeCallback(typeof(delfpt<float4>))]
        static float4 OP(float4 f1, float4 f2, FunctionPointer<p2f1> func) => new(func.Invoke(f1.x, f2.x), func.Invoke(f1.y, f2.y), func.Invoke(f1.z, f2.z), func.Invoke(f1.w, f2.w));
        
        

        // Compiled function pointers
        // static FunctionPointer<T> funct<T>() => Comp(OP);
        static FunctionPointer<delf1> funcf1 = Comp<delf1>(OP);
        static FunctionPointer<delf2> funcf2 = Comp<delf2>(OP);
        static FunctionPointer<delf3> funcf3 = Comp<delf3>(OP);
        static FunctionPointer<delf4> funcf4 = Comp<delf4>(OP);
        // static FunctionPointer<p2f2> funcF2OP = Comp<p2f2>(mul);
            
        static FunctionPointer<p2f1> FP_mul = Comp<p2f1>(math.mul);
        static FunctionPointer<p2f1> FP_sum = Comp<p2f1>(mathx.sum);
        static FunctionPointer<p2f1> FP_max = Comp<p2f1>(math.max);
        static FunctionPointer<p2f1> FP_min = Comp<p2f1>(math.min);
        // Specify type in advance with method overload to simplify code
        // static FunctionPointer<p2f1> Compile(p2f1 func) => Comp(func);
        // static FunctionPointer<p2f2> Compile(p2f2 func) => Comp(func);
        // static FunctionPointer<p2f3> Compile(p2f3 func) => Comp(func);
        // static FunctionPointer<p2f4> Compile(p2f4 func) => Comp(func);
        
        static FunctionPointer<p2ft<T>> Compile<T>(p2ft<T> func) => Comp(func);


        // public static float2 test = OP(1.0f, 2.0f, Compile(mathx.mul));
        // public static float2 test2 = Process((float2)1.0f, 2.0f, mathx.mul);`
        // public static float3 test3 = Process((float3)1.0f, 2.0f, mathx.mul);
        // public static float4 test4 = Process((float4)1.0f, 2.0f, mathx.mul);
        // public static float4 test5 = Process(1.0f, 2.0f, mathx.mul);
        
        // public static T Process<T>(T f1, T f2, p2ft<T> func) where T : struct => funcf2.Invoke(f1, f2, Compile(func));
        // public static float2 Process(float2 f1, float2 f2, p2f1 func) => funcf2.Invoke(f1, f2, Compile(func));
        // public static float3 Process(float3 f1, float3 f2, p2f1 func) => funcf3.Invoke(f1, f2, Compile(func));
        // public static float4 Process(float4 f1, float4 f2, p2f1 func) => funcf4.Invoke(f1, f2, Compile(func));
        // //
        // public static T Process<T>(T f1, T f2, delfpt<T> func) where T : struct => funct<T>().Invoke(f1, f2, func);
        // public static T Process<T>(T f1, T f2, p2f1 func) where T : struct => funcf1.Invoke(f1, f2, Compile(func));


        private static FunctionPointer<T> Comp<T>(T delegatMethod) where T : Delegate => CompileFunctionPointer(delegatMethod);
        
        // Testing dynamic invoking compiled funciton with burst
        public delegate object dynainvokedelegate(object[] args);
        
        public static object DynamicInvokeNormal(this Delegate method, params object[] args) => method.DynamicInvoke(args);
        public static FunctionPointer<dynainvokedelegate> DynamicInvokeCompiled(this Delegate method) => Comp<dynainvokedelegate>(method.DynamicInvoke);
        
        public static FunctionPointer<T> DynamicCompile<T>(T method) where T : Delegate => CompileFunctionPointer(method);
        public static object DynamicCompiledInvoke<T>(T method, object[] args) where T : Delegate => DynamicInvokeCompiled(method).Invoke(args);
    }
    public static partial class mathx
    {
        // [MonoPInvokeCallbackAttribute] attribute to the functions. You need to add this so that IL2CPP works with these functions.
        // Todo: move these inside 
        [BurstCompile, MonoPInvokeCallback(typeof(p2f1))] public static float mul(float a, float b) => a * b;
        [BurstCompile, MonoPInvokeCallback(typeof(p2f1))] public static float sum(float a, float b) => a + b;

        public delegate T process2vectors<T>(T a, T b) where T : struct;
        public delegate float process2floats(float a, float b);
        public delegate float process2floatsAndParam(float a, float b, float t);
        public delegate float processfloat(float a);
        // generic function pointer compiler
        // public static FunctionPointer<Delegate> ToPointer(Delegate function) => CompileFunctionPointer(function);
        // public static FunctionPointer<process2floats> ToPointer(process2floats function) => CompileFunctionPointer(function); // Infered Type
        public static FunctionPointer<T> ToPointer<T>(T function) where T : Delegate => CompileFunctionPointer<T>(function); // Infered Type

        public static float4 ParallelOperation(process2floats func, float4 a, float4 b) => ToPointer(func).ParallelOperation(a, b);
        public static float3 ParallelOperation(process2floats func, float3 a, float3 b) => ToPointer(func).ParallelOperation(a, b);
        public static float2 ParallelOperation(process2floats func, float2 a, float2 b) => ToPointer(func).ParallelOperation(a, b);
        public static float ParallelOperation(process2floats func, float a, float b) => ToPointer(func).ParallelOperation(a, b);

        // With method Compilation :
        public static float4 ParallelOperation(this FunctionPointer<process2floats> func, float4 a, float4 b) => new(func.Invoke(a.x, b.x), func.Invoke(a.y, b.y), func.Invoke(a.z, b.z), func.Invoke(a.w, b.w));
        public static float3 ParallelOperation(this FunctionPointer<process2floats> func, float3 a, float3 b) => new(func.Invoke(a.x, b.x), func.Invoke(a.y, b.y), func.Invoke(a.z, b.z));
        public static float2 ParallelOperation(this FunctionPointer<process2floats> func, float2 a, float2 b) => new(func.Invoke(a.x, b.x), func.Invoke(a.y, b.y));
        public static float ParallelOperation(this FunctionPointer<process2floats> func, float a, float b) => func.Invoke(a, b);
        
        
        public static float4 ParallelOperationAndParam(this process2floatsAndParam func, float4 a, float4 b, float t) => ToPointer(func).ParallelOperationAndParam(a, b, t);
        public static float3 ParallelOperationAndParam(this process2floatsAndParam func, float3 a, float3 b, float t) => ToPointer(func).ParallelOperationAndParam(a, b, t);
        public static float2 ParallelOperationAndParam(this process2floatsAndParam func, float2 a, float2 b, float t) => ToPointer(func).ParallelOperationAndParam(a, b, t);
        public static float ParallelOperationAndParam(this process2floatsAndParam func, float a, float b, float t) => ToPointer(func).ParallelOperationAndParam(a, b, t);
        
        // With method Compilation :
        public static float4 ParallelOperationAndParam(this FunctionPointer<process2floatsAndParam> func, float4 a, float4 b, float t) => new(func.Invoke(a.x, b.x, t), func.Invoke(a.y, b.y, t), func.Invoke(a.z, b.z, t), func.Invoke(a.w, b.w, t));
        public static float3 ParallelOperationAndParam(this FunctionPointer<process2floatsAndParam> func, float3 a, float3 b, float t) => new(func.Invoke(a.x, b.x, t), func.Invoke(a.y, b.y, t), func.Invoke(a.z, b.z, t));
        public static float2 ParallelOperationAndParam(this FunctionPointer<process2floatsAndParam> func, float2 a, float2 b, float t) => new(func.Invoke(a.x, b.x, t), func.Invoke(a.y, b.y, t));
        public static float ParallelOperationAndParam(this FunctionPointer<process2floatsAndParam> func, float a, float b, float t) => func.Invoke(a, b, t);
        
        // Now generic parallel operation using these method overloads
        // public static T ParallelOperation<T>(process2vectors<T> func, T a, T b) where T : struct
        // {
        //     if (typeof(T) == typeof(float4)) return (T)(object)ParallelOperation(func as process2floats, (float4)(object)a, (float4)(object)b);
        //     if (typeof(T) == typeof(float3)) return (T)(object)ParallelOperation(func as process2floats, (float3)(object)a, (float3)(object)b);
        //     if (typeof(T) == typeof(float2)) return (T)(object)ParallelOperation(func as process2floats, (float2)(object)a, (float2)(object)b);
        //     if (typeof(T) == typeof(float)) return (T)(object)ParallelOperation(func as process2floats, (float)(object)a, (float)(object)b);
        //     throw new NotImplementedException();
        // }
        
        public static float SumOperations(process2floats func, float4 a, float4 b) => func.Invoke(a.x, b.x) + func.Invoke(a.y, b.y) + func.Invoke(a.z, b.z) + func.Invoke(a.w, b.w);
        public static float SumOperations(process2floats func, float3 a, float3 b) => func.Invoke(a.x, b.x) + func.Invoke(a.y, b.y) + func.Invoke(a.z, b.z);
        public static float SumOperations(process2floats func, float2 a, float2 b) => func.Invoke(a.x, b.x) + func.Invoke(a.y, b.y);
        public static float SumOperations(process2floats func, float a, float b) => func.Invoke(a, b);

        public static float SequentialOperation(process2floats func, float4 a) => func.Invoke(func.Invoke(func.Invoke(a.x, a.y), a.z), a.w);

        // Simple operations :

        // public delegate float process2float2s<T>(float2 a, float2 b);
        // public delegate float process2float3s<T>(float3 a, float3 b);
        // public delegate float process2float4s<T>(float4 a, float4 b);

        public static float Seq<T>(this T value, FunctionPointer<process2floats> operation) where T : unmanaged
        {
            float result = 0;
            int length = Unsafe.SizeOf<T>() / sizeof(float);
            for (int i = 0; i < length; i++)
            {
                float component = Unsafe.Add(ref Unsafe.As<T, float>(ref Unsafe.AsRef(in value)), i);
                result = operation.Invoke(result, component);
            }
            return result;
        }




    }
}