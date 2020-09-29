using Refinitiv.DataPlatform.Delivery.Stream;
using System;
using System.Threading.Tasks;
using WebSocketSharp;

namespace _3._0._03_Core_WebSocket
{
    // Websocket-sharp (https://github.com/sta/websocket-sharp) 
    internal class WebSocketSharpImpl : IWebSocket
    {
        private WebSocket _webSocket;        // Main WebSocket container
        private bool _disposed = false;

        #region IDisposable Support
        /// <summary>
        /// Ensure the WebSocket channel is closed.
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                ((IDisposable)_webSocket)?.Dispose();
            }
        }

        /// <summary>
        /// Ensure the WebSocket channel is closed.
        /// </summary>
        ~WebSocketSharpImpl()
        {
            Dispose();
        }
        #endregion

        #region IWebSocket Implementation
        public void Register(RDPWebSocket.Params wsParams)
        {
            _webSocket = new WebSocket(wsParams.Url, wsParams.Protocol);

            // Define callbacks
            _webSocket.OnOpen += (sender, e) => wsParams.OpenCb(this);
            _webSocket.OnError += (sender, e) => wsParams.ErrorCb(this, $"WebSocket error occurred: {e.Message}");
            _webSocket.OnClose += (sender, e) => wsParams.CloseCb(this, $"WebSocket connection closed. Code: {e.Code} Reason: [{e.Reason}]");
            _webSocket.OnMessage += (sender, e) => wsParams.MessageCb(this, e.Data);

            // Override WebSocket output
            _webSocket.Log.Output = (data, s) =>
            {
                switch (data.Level)
                {
                    case LogLevel.Trace:
                        Console.WriteLine("Trace - WebSocketSharp: {msg}", data.Message);
                        break;
                    case LogLevel.Debug:
                        Console.WriteLine("Debug - WebSocketSharp: {msg}", data.Message);
                        break;
                    case LogLevel.Info:
                        Console.WriteLine("Info - WebSocketSharp: {msg}", data.Message);
                        break;
                    case LogLevel.Warn:
                        Console.WriteLine("Warn - WebSocketSharp: {msg}", data.Message);
                        break;
                    case LogLevel.Error:
                    case LogLevel.Fatal:
                        if (_webSocket.ReadyState != WebSocketState.Closing)
                        {
                            Console.WriteLine("Error/Fatal - WebSocketSharp: {msg}", data.Message);
                        }

                        break;
                    default:
                        break;
                }
            };
        }
        public RDPWebSocket.State State
        {
            get
            {
                switch (_webSocket.ReadyState)
                {
                    case WebSocketState.Open:
                        return RDPWebSocket.State.Open;
                    case WebSocketState.Connecting:
                        return RDPWebSocket.State.Connecting;
                    case WebSocketState.Closing:
                        return RDPWebSocket.State.Closing;
                    default:
                        return RDPWebSocket.State.Closed;
                }
            }
        }
        #endregion

        #region Class Processing
        public Task ConnectAsync()
        {
            return Task.Run(() =>
            {
                _webSocket.Connect();
                TaskCompletionSource<bool> t = new TaskCompletionSource<bool>();
                t.TrySetResult(true);
                return t.Task;
            });
        }

        public Task CloseAsync()
        {
            return Task.Run(() =>
            {
                _webSocket.Close();
                TaskCompletionSource<bool> t = new TaskCompletionSource<bool>();
                t.TrySetResult(true);
                return t.Task;
            });
        }

        public Task SendAsync(string msg)
        {
            return Task.Run(() => 
            {
                _webSocket.Send(msg);
                TaskCompletionSource<bool> t = new TaskCompletionSource<bool>();
                t.TrySetResult(true);
                return t.Task;
            });
        }
        #endregion
    }
}
