using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityWebSocket;

namespace ET
{
    public class WChannel: AChannel
    {
        private readonly WService Service;
        
        private IWebSocket webSocket;

        private IWebSocket wsTemp;

        private Queue<MemoryBuffer> waitSend = new();
        
        public WChannel(long id, IPEndPoint ipEndPoint, WService service)
        {
            this.Service = service;
            this.Id = id;
            
            wsTemp = new WebSocket($"ws://{ipEndPoint}");

            this.RemoteAddress = ipEndPoint;

            // Subscribe to the WS events
            wsTemp.OnOpen += OnOpen;
            wsTemp.OnClose += OnClosed;
            wsTemp.OnError += OnError;
            wsTemp.OnMessage += OnRead;

            // Start connecting to the server
            wsTemp.ConnectAsync();
        }
        
        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }
            
            this.Id = 0;

            this.webSocket?.CloseAsync();
            this.webSocket = null;
        }

        public void Send(MemoryBuffer memoryBuffer)
        {
            if (this.webSocket == null)
            {
                this.waitSend.Enqueue(memoryBuffer);
                return;
            }

            SendOne(memoryBuffer);
        }

        private void SendOne(MemoryBuffer memoryBuffer)
        {
            this.webSocket.SendAsync(memoryBuffer.GetBuffer(), (int)memoryBuffer.Position, (int)memoryBuffer.Length);
        }

        private void OnOpen(object sender, OpenEventArgs e)
        {
            /*if (ws == null)
            {
                this.OnError(ErrorCore.ERR_WebsocketConnectError);
                return;
            }*/

            if (this.IsDisposed)
            {
                return;
            }

            this.webSocket = wsTemp;
                
            while (this.waitSend.Count > 0)
            {
                MemoryBuffer memoryBuffer = this.waitSend.Dequeue();
                this.SendOne(memoryBuffer);
            }
        }

        /// <summary>
        /// Called when we received a text message from the server
        /// </summary>
        private void OnRead(object sender, MessageEventArgs e)
        {
            if (this.IsDisposed)
            {
                return;
            }
            
            MemoryBuffer memoryBuffer = this.Service.Fetch();
            memoryBuffer.Write(e.RawData);
            memoryBuffer.Seek(0, SeekOrigin.Begin);
            this.Service.ReadCallback(this.Id, memoryBuffer);
        }

        /// <summary>
        /// Called when the web socket closed
        /// </summary>
        private void OnClosed(object sender, CloseEventArgs e)
        {
            if (this.IsDisposed)
            {
                return;
            }
            
            Log.Error($"wchannel closed: StatusCode: {e.StatusCode}, Reason: {e.Reason}");
            this.OnError(0);
        }

        /// <summary>
        /// Called when an error occured on client side
        /// </summary>
        private void OnError(object sender, UnityWebSocket.ErrorEventArgs e)
        {
            if (this.IsDisposed)
            {
                return;
            }
            
            Log.Error($"WChannel error: {this.Id} {e.Message}");
            
            this.OnError(ErrorCore.ERR_WebsocketError);
        }
        
        private void OnError(int error)
        {
            Log.Info($"WChannel error: {this.Id} {error}");
            
            long channelId = this.Id;
			
            this.Service.Remove(channelId);
			
            this.Service.ErrorCallback(channelId, error);
        }
    }
}