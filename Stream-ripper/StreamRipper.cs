using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using StreamRipper.Interfaces;
using StreamRipper.Models.Events;
using StreamRipper.Utilities;

namespace StreamRipper
{
    public class StreamRipper : IDisposable, IStreamRipper
    {
        /// <summary>
        /// Url to stream
        /// </summary>
        private readonly string _url;

        /// <summary>
        /// Plugin manager instance
        /// </summary>
        private readonly IPluginManager _pluginManager;

        /// <summary>
        /// StreamRipping task
        /// </summary>
        private Task _runningTask;

        /// <summary>
        /// Flag to indicate whether task is running or not
        /// </summary>
        private bool _running;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="url"></param>
        /// <param name="pluginManager"></param>
        public StreamRipper(Uri url, IPluginManager pluginManager)
        {
            _url = url.AbsoluteUri;
            _running = false;
            _pluginManager = pluginManager;
        }

        /// <summary>
        /// Start the streaming
        /// </summary>
        public void StartAsync()
        {
            _running = true;
            _runningTask = Task.Run(() => StreamHttpRadio());
        }

        /// <summary>
        /// Start the streaming
        /// </summary>
        public void Start()
        {
            _running = true;
            _runningTask = new Task(StreamHttpRadio);
            _runningTask.RunSynchronously();
        }

        /// <summary>
        /// Stream HTTP Radio
        /// </summary>
        private void StreamHttpRadio()
        {
            var request = (HttpWebRequest) WebRequest.Create(_url);
            request.Headers.Add("icy-metadata", "1");
            request.ReadWriteTimeout = 10 * 1000;
            request.Timeout = 10 * 1000;

            using (var response = (HttpWebResponse) request.GetResponse())
            {
                // Trigger on stream started
                _pluginManager.OnStreamStarted(new StreamStartedEventArg());

                // Get the position of metadata
                var metaInt = 0;
                if (!string.IsNullOrEmpty(response.GetResponseHeader("icy-metaint")))
                {
                    metaInt = Convert.ToInt32(response.GetResponseHeader("icy-metaint"));
                }

                using (var socketStream = response.GetResponseStream())
                {
                    try
                    {
                        var buffer = new byte[16384];
                        var metadataLength = 0;
                        var streamPosition = 0;
                        var bufferPosition = 0;
                        var readBytes = 0;
                        var metadataSb = new StringBuilder();

                        while (_running)
                        {
                            if (bufferPosition >= readBytes)
                            {
                                if (socketStream != null) readBytes = socketStream.Read(buffer, 0, buffer.Length);
                                bufferPosition = 0;
                            }

                            if (readBytes <= 0)
                            {
                                break;
                            }

                            if (metadataLength == 0)
                            {
                                if (metaInt == 0 || streamPosition + readBytes - bufferPosition <= metaInt)
                                {
                                    streamPosition += readBytes - bufferPosition;
                                    ProcessStreamData(buffer, ref bufferPosition, readBytes - bufferPosition);
                                    continue;
                                }

                                ProcessStreamData(buffer, ref bufferPosition, metaInt - streamPosition);
                                metadataLength = Convert.ToInt32(buffer[bufferPosition++]) * 16;

                                // Check if there's any metadata, otherwise skip to next block
                                if (metadataLength == 0)
                                {
                                    streamPosition = Math.Min(readBytes - bufferPosition, metaInt);
                                    ProcessStreamData(buffer, ref bufferPosition, streamPosition);
                                    continue;
                                }
                            }

                            // Get the metadata and reset the position
                            while (bufferPosition < readBytes)
                            {
                                metadataSb.Append(Convert.ToChar(buffer[bufferPosition++]));
                                metadataLength--;

                                // ReSharper disable once InvertIf
                                if (metadataLength == 0)
                                {
                                    var metadata = metadataSb.ToString();
                                    streamPosition = Math.Min(readBytes - bufferPosition, metaInt);
                                    ProcessStreamData(buffer, ref bufferPosition, streamPosition);

                                    // Trigger song change event
                                    _pluginManager.OnMetadataChanged(new MetadataChangedEventArg
                                    {
                                        SongMetadata = MetadataUtility.ParseMetadata(metadata)
                                    });

                                    metadataSb.Clear();
                                    break;
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        // Invoke on stream ended
                        _pluginManager.OnStreamEnded(new StreamEndedEventArg { Exception = e });
                    }
                }
            }
        }

        /// <summary>
        /// Process the stream
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        private void ProcessStreamData(byte[] buffer, ref int offset, int length)
        {
            if (length < 1)
            {
                return;
            }

            var data = new byte[length];
            Buffer.BlockCopy(buffer, offset, data, 0, length);

            // Trigger update
            _pluginManager.OnStreamUpdate(new StreamUpdateEventArg
            {
                SongRawPartial = data
            });

            offset += length;
        }

        /// <summary>
        /// Dispose the running task
        /// </summary>
        public void Dispose()
        {
            _running = false;
            _runningTask?.Dispose();
        }
    }
}