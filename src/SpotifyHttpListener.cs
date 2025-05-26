using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace DotSpotifyWebWrapper
{
    internal class SpotifyHttpListener : IDisposable
    {
        public string ListeningOnUrl { get; }

        #region Private fields
        private readonly HttpListener _listener;
        private readonly Thread _listenerThread;
        private Action<(HttpListenerRequest request, HttpListenerResponse response)>? _currentCallback;
        private bool _disposedValue;
        #endregion

        public SpotifyHttpListener(HttpListener listener, int port)
        {
            if (port < 1 || port > ushort.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(port));

            _listener = listener;

            ListeningOnUrl = $"http://localhost:{port}/";
            _listener.Prefixes.Add(ListeningOnUrl);
            _listenerThread = new(ListenForIncomingCalls) { IsBackground = true };
            _listenerThread.Start();
        }

        #region Public methods
        public void CloseListener()
        {
            _listener.Close();
        }

        public void RegisterCallbackForNextRequest(Action<(HttpListenerRequest request, HttpListenerResponse response)> callback)
        {
            _currentCallback = callback;

        }
        #endregion

        private void ListenForIncomingCalls()
        {
            _listener.Start();
            while (true)
            {
                HttpListenerContext? context;
                try
                {
                    context = _listener.GetContext();
                }
                catch (Exception e) when (e is HttpListenerException || e is InvalidOperationException || e is ObjectDisposedException)
                {
                    break;
                }

                var request = context!.Request;
                var response = context!.Response;

                if (_currentCallback != null)
                {
                    Task.Run(() =>
                    {
                        _currentCallback.Invoke((request, response));
                        _currentCallback = null;
                    });
                }
                else
                {
                    response.Close();
                }
            }
        }

        #region Disposal
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                    _listener.Close();
                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
