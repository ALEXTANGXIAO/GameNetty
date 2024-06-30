using System;
using System.Collections;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace ET
{
    public static partial class HttpClientHelper
    {
        public static async ETTask<string> Get(string link)
        {
            try
            {
#if USE_UNITY_WEBREQUEST || UNITY_WEBGL
                string result = await GetRequest(link);
#else
                using HttpClient httpClient = new();
                HttpResponseMessage response =  await httpClient.GetAsync(link);
                string result = await response.Content.ReadAsStringAsync();
#endif
                return result;
            }
            catch (Exception e)
            {
                Log.Warning(e.Message);
                throw new Exception($"http request fail: {link.Substring(0,link.IndexOf('?'))}\n{e}");
            }
        }
        
        public static async Task<string> GetRequest(string url)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
               await webRequest.SendWebRequest();
                if (!string.IsNullOrEmpty(webRequest.error))
                {
                    Log.Error(webRequest.error);
                }
                else
                {
                    return webRequest.downloadHandler.text;
                }
                return string.Empty;
            }
        }
        
        /// <summary>
        /// 异步的扩展方法
        /// </summary>
        public static TaskAwaiter GetAwaiter(this UnityWebRequestAsyncOperation asyncOp)
        {
            var tcs = new TaskCompletionSource<object>();
            asyncOp.completed += obj => { tcs.SetResult(null); };
            return ((Task)tcs.Task).GetAwaiter();
        }
    }
}