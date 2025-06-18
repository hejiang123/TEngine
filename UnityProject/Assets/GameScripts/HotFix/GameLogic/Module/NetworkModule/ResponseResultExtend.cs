using System;
using Google.Protobuf;
using IoGame.Sdk;
using TEngine;

namespace GameLogic
{
    public static class ResponseResultExtend
    {
        public static void Check<T>(this ResponseResult responseResult, Action<T> action) where T : IMessage, new()
        {
            if (responseResult.Success())
            {
                action?.Invoke(responseResult.GetValue<T>());
            }
            else
            {
                Log.Error($"发生错误 {responseResult.Message}");
            }
        }
        
        public static void Check(this ResponseResult responseResult, Action action)
        {
            if (responseResult.Success())
            {
                action?.Invoke();
            }
            else
            {
                Log.Error($"发生错误 {responseResult.Message}");
            }
        }
    }
}