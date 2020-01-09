using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using StreamRipper.Interfaces;
using StreamRipper.Models;
using StreamRipper.Models.Events;
using StreamRipper.Utilities;

namespace StreamRipper
{
    public class StreamRipper : IStreamRipper
    {
        /// <summary>
        /// Metadata updated event handlers
        /// </summary>
        public EventHandler<MetadataChangedEventArg> MetadataEventHandlers { get; set;}

        /// <summary>
        /// Stream updated event handlers
        /// </summary>
        public EventHandler<StreamUpdateEventArg> StreamUpdateEventHandlers { get; set; }
        
        /// <summary>
        /// Stream started event handlers
        /// </summary>
        public EventHandler<StreamStartedEventArg> StreamStartedEventHandlers { get; set; }
        
        /// <summary>
        /// Stream ended event handlers
        /// </summary>
        public EventHandler<StreamEndedEventArg> StreamEndedEventHandlers { get; set; }
        
        /// <summary>
        /// Song change event handlers
        /// </summary>
        public EventHandler<SongChangedEventArg> SongChangedEventHandlers { get; set; }

        /// <summary>
        /// Exception handler
        /// </summary>
        public EventHandler<StreamFailedEventArg> StreamFailedHandlers { get; set; }

        /// <summary>
        /// Url to stream
        /// </summary>
        private readonly string _url;

        /// <summary>
        /// StreamRipping task
        /// </summary>
        private Task _runningTask;

        /// <summary>
        /// Flag to indicate whether task is running or not
        /// </summary>
        private bool _running;

        /// <summary>
        /// Count of songs, ripped so far
        /// </summary>
        private decimal _count = 0;
        
        /// <summary>
        /// SongInfo reference
        /// </summary>
        private SongInfo _songInfo;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="url"></param>
        /// <param name="filters"></param>
        public StreamRipper(Uri url, IEnumerable<Func<SongMetadata, bool>> filters)
        {
            _url = url.AbsoluteUri;

            SongChangedEventHandlers = (_, arg) => { };

            StreamEndedEventHandlers += (_, arg) => { };
            
            StreamStartedEventHandlers += (_, arg) =>
            {
                // Initialize the SongInfo
                _songInfo = new SongInfo
                {
                    // Empty properties
                    SongMetadata = new SongMetadata(),
                    Stream = new MemoryStream()
                };
            };

            StreamUpdateEventHandlers += (_, arg) =>
            {
                // Append to MemoryStream
                _songInfo.Stream.Write(arg.SongRawPartial, 0, arg.SongRawPartial.Length);
            };

            MetadataEventHandlers += (_, arg) =>
            {
                // if count is greater than zero, ignore the first one
                if (_count > 0)
                {
                    if (filters.All(x => x(arg.SongMetadata)))
                    {
                        SongChangedEventHandlers.Invoke(this, new SongChangedEventArg
                        {
                            // Clone SongInfo
                            SongInfo = (SongInfo) _songInfo.Clone()
                        });
                    }

                    // Clean the buffer
                    _songInfo.Dispose();
                }
                
                // Set the metadata
                _songInfo.SongMetadata = arg.SongMetadata;
            };
            
            // Initialize
            _running = false;
        }

        /// <summary>
        /// Start the streaming in async fashion
        /// </summary>
        public void Start()
        {
            _running = true;
            _runningTask = Task.Run(StreamHttpRadio);
        }

        /// <summary>
        /// Stream HTTP Radio
        /// </summary>
        private void StreamHttpRadio()
        {
            try
            {
                var request = (HttpWebRequest) WebRequest.Create(_url);
                request.Headers.Add("icy-metadata", "1");
                request.ReadWriteTimeout = 10 * 1000;
                request.Timeout = 10 * 1000;

                using (var response = (HttpWebResponse) request.GetResponse())
                {
                    // Trigger on stream started
                    StreamStartedEventHandlers.Invoke(this, new StreamStartedEventArg());

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
                            while (_running)
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
                                    StreamEndedEventHandlers.Invoke(this, new StreamEndedEventArg());
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
                                        MetadataEventHandlers.Invoke(this, new MetadataChangedEventArg
                                        {
                                            SongMetadata = MetadataUtility.ParseMetadata(metadata)
                                        });

                                        // Increment the count
                                        _count++;

                                        metadataSb.Clear();
                                        break;
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            // Invoke on stream ended
                            StreamEndedEventHandlers.Invoke(this, new StreamEndedEventArg());
                            StreamFailedHandlers.Invoke(this, new StreamFailedEventArg {Exception = e});
                            Console.WriteLine(e.Message);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                // Invoke on stream ended
                StreamFailedHandlers.Invoke(this, new StreamFailedEventArg {Exception = e});
                Console.WriteLine(e.Message);
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
            StreamUpdateEventHandlers.Invoke(this, new StreamUpdateEventArg
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