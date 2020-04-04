using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StreamRipper.Interfaces;
using StreamRipper.Models.Events;
using StreamRipper.Models.State;
using StreamRipper.Utilities;
using static StreamRipper.Logic.EventHandlerImpl;

namespace StreamRipper
{
    public class StreamRipperImpl : IStreamRipper
    {
        /// <summary>
        /// Flag to indicate whether task is running or not
        /// </summary>
        private CancellationTokenSource _cancellationToken;

        private readonly Uri _url;

        private readonly ILogger _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="url"></param>
        /// <param name="logger"></param>
        public StreamRipperImpl(Uri url, ILogger logger)
        {
            _url = url;

            _logger = logger;

            // Initialize
            _cancellationToken = new CancellationTokenSource();
        }

        public EventHandler<MetadataChangedEventArg> MetadataChangedHandlers { get; set; }

        public EventHandler<StreamUpdateEventArg> StreamUpdateEventHandlers { get; set; }

        public EventHandler<StreamStartedEventArg> StreamStartedEventHandlers { get; set; }

        public EventHandler<StreamEndedEventArg> StreamEndedEventHandlers { get; set; }

        public EventHandler<SongChangedEventArg> SongChangedEventHandlers { get; set; }

        public EventHandler<StreamFailedEventArg> StreamFailedHandlers { get; set; }

        /// <summary>
        /// Start the streaming in async fashion
        /// </summary>
        public void Start()
        {
            _cancellationToken = new CancellationTokenSource();

            var token = _cancellationToken.Token;

            Task.Factory
                .StartNew(state => StreamHttpRadio((EventState) state, token), new EventState(_url.AbsoluteUri, _logger)
                {
                    EventHandlers = new EventHandlers
                    {
                        SongChangedEventHandlers = TypedHandler(SongChangedEventHandler) + SongChangedEventHandlers,
                        StreamEndedEventHandlers = TypedHandler(StreamEndedEventHandler) + StreamEndedEventHandlers,
                        StreamStartedEventHandlers = TypedHandler(StreamStartedEventHandler) + StreamStartedEventHandlers,
                        StreamUpdateEventHandlers = TypedHandler(StreamUpdateEventHandler) + StreamUpdateEventHandlers,
                        MetadataChangedHandlers = TypedHandler(MetadataChangedHandler) + MetadataChangedHandlers
                    }
                }, token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        /// <summary>
        /// Stream HTTP Radio
        /// </summary>
        private static void StreamHttpRadio(EventState state, CancellationToken token)
        {
            try
            {
                var request = (HttpWebRequest) WebRequest.Create(state.Url);
                request.Headers.Add("icy-metadata", "1");
                request.ReadWriteTimeout = 10 * 1000;
                request.Timeout = 10 * 1000;

                using (var response = (HttpWebResponse) request.GetResponse())
                {
                    // Trigger on stream started
                    state.EventHandlers.StreamStartedEventHandlers.Invoke(state, new StreamStartedEventArg());

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

                            // Loop forever
                            while (!token.IsCancellationRequested)
                            {
                                if (bufferPosition >= readBytes)
                                {
                                    if (socketStream != null)
                                    {
                                        readBytes = socketStream.Read(buffer, 0, buffer.Length);
                                    }

                                    bufferPosition = 0;
                                }

                                if (readBytes <= 0)
                                {
                                    // Stream ended
                                    state.EventHandlers.StreamEndedEventHandlers.Invoke(state,
                                        new StreamEndedEventArg());
                                    break;
                                }

                                if (metadataLength == 0)
                                {
                                    if (metaInt == 0 || streamPosition + readBytes - bufferPosition <= metaInt)
                                    {
                                        streamPosition += readBytes - bufferPosition;
                                        ProcessStreamData(state, buffer, ref bufferPosition,
                                            readBytes - bufferPosition);
                                        continue;
                                    }

                                    ProcessStreamData(state, buffer, ref bufferPosition, metaInt - streamPosition);
                                    metadataLength = Convert.ToInt32(buffer[bufferPosition++]) * 16;

                                    // Check if there's any metadata, otherwise skip to next block
                                    if (metadataLength == 0)
                                    {
                                        streamPosition = Math.Min(readBytes - bufferPosition, metaInt);
                                        ProcessStreamData(state, buffer, ref bufferPosition, streamPosition);
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
                                        ProcessStreamData(state, buffer, ref bufferPosition, streamPosition);

                                        // Trigger song change event
                                        state.EventHandlers.MetadataChangedHandlers.Invoke(state,
                                            new MetadataChangedEventArg
                                            {
                                                SongMetadata = MetadataUtility.ParseMetadata(metadata)
                                            });

                                        // Increment the count
                                        state.Count++;

                                        metadataSb.Clear();
                                        break;
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            // Invoke on stream ended
                            state.EventHandlers.StreamEndedEventHandlers.Invoke(state, new StreamEndedEventArg());
                            state.EventHandlers.StreamFailedHandlers.Invoke(state,
                                new StreamFailedEventArg {Exception = e, Message = "Stream loop threw an exception"});
                        }
                    }
                }
            }
            catch (Exception e)
            {
                // Invoke on stream ended
                state.EventHandlers.StreamFailedHandlers.Invoke(state,
                    new StreamFailedEventArg {Exception = e, Message = "Stream threw an exception"});
            }
        }

        /// <summary>
        /// Process the stream
        /// </summary>
        /// <param name="state"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        private static void ProcessStreamData(EventState state, byte[] buffer, ref int offset, int length)
        {
            if (length < 1)
            {
                return;
            }

            var data = new byte[length];
            Buffer.BlockCopy(buffer, offset, data, 0, length);

            // Trigger update
            state.EventHandlers.StreamUpdateEventHandlers.Invoke(state, new StreamUpdateEventArg
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
            _cancellationToken.Cancel();
        }
    }
}