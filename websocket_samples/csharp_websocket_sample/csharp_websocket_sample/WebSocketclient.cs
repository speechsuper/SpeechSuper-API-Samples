using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace csharp_websocket_sample
{
    class WebSocketclient
    {
        private readonly SemaphoreSlim sendResource = new SemaphoreSlim(1, 1);

        private readonly ClientWebSocket ws;

        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public WebSocketclient()
        {
            ws = new ClientWebSocket();
        }

        public Action<string> OnMessage { get; set; }
        public Action OnClose { get; set; }
        public Action<Exception> OnError { get; set; }

        public async Task ConnectAsync(string url, CancellationToken cancellationToken = default(CancellationToken))
        {
            await ws.ConnectAsync(new Uri(url), cancellationToken).ConfigureAwait(false);
            Receive();

        }

        private async void Receive()
        {
            while(true)
            {
                byte[] bytes = new byte[4096];
                
                try
                {
                    var result = await ws.ReceiveAsync(new ArraySegment<byte>(bytes), cancellationTokenSource.Token).ConfigureAwait(false);
                    if(result != null && (result.CloseStatus.HasValue || result.MessageType == WebSocketMessageType.Close))
                    {
                        if (OnClose != null)
                        {
                            OnClose();
                        }
                        break;
                    } else
                    {
                        using (var stream = new MemoryStream())
                        {
                            stream.Write(bytes, 0, result.Count);
                            while (!result.EndOfMessage)
                            {
                                result = await ws.ReceiveAsync(new ArraySegment<byte>(bytes), CancellationToken.None);
                                stream.Write(bytes, 0, result.Count);
                            }

                            stream.Seek(0, SeekOrigin.Begin);
                            using (var reader = new StreamReader(stream, Encoding.UTF8))
                            {
                                string message = reader.ReadToEnd();
                                if(OnMessage != null)
                                {
                                    OnMessage(message);
                                }
                            }
                        }
                    }
                } catch(OperationCanceledException)
                {
                    if(OnClose != null)
                    {
                        OnClose();
                    }
                    break;
                } catch (Exception e)
                {
                    if(OnError != null)
                    {
                        OnError(e);
                    }
                    break;
                }
            }
        }

        public async Task SendAsync(byte[] buffer, WebSocketMessageType messageType, CancellationToken _cancellationToken = default(CancellationToken))
        {
            await sendResource.WaitAsync(_cancellationToken).ConfigureAwait(false);
            try
            {
                var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cancellationToken, cancellationTokenSource.Token);
                await ws.SendAsync(new ArraySegment<byte>(buffer), messageType, true, linkedTokenSource.Token).ConfigureAwait(false);
            }finally
            {
                sendResource.Release();
            }
        }

        public void Cancel()
        {
            cancellationTokenSource.Cancel();
            if (ws != null)
            {
                if(ws.State == WebSocketState.Open)
                {
                    ws.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                    ws.Dispose();
                }
            }
        }

    }
}
