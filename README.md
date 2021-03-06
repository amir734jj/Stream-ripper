# Stream-ripper

Stream Ripper library, convert an online radio (IceCast stream) to your music library!

Basically, extract mp3 with metadata from an IceCast stream URL. See this NuGet library [in action](https://stream-subscription-ui.herokuapp.com/#/about)

[![NuGet Status](https://img.shields.io/nuget/v/StreamRipper.svg)](https://www.nuget.org/packages/StreamRipper/)


```csharp
var streamRipper = StreamRipperFactory.New(new StreamRipperOptions
{
    Url = new Uri("http://stream.radiojavan.com/radiojavan"),
    Logger = serviceProvider.GetService<ILogger<IStreamRipper>>(),
    MaxBufferSize = 10 * 1000000    // stop when buffer size passes 10 megabytes
});

// The recommended way is to have an async event handlers
streamRipper.SongChangedEventHandlers += async (_, arg) =>
{
    // Create filename from SongInfo
    var filename = $"{arg.SongInfo.SongMetadata}";

    // Save the stream to file
    await arg.SongInfo.Stream.ToFileStream($@"\Music\ripped\{filename}.mp3");
};

// Async start
streamRipper.Start();

// To Stop
// streamRipper.Dispose();
```

Events:
- `OnMetadataChanged`: will be invoked when metadata changes
- `OnStreamUpdate`: will be invoked when new `byte[]` gets downloaded
- `OnStreamStarted`: will be invoked when stream starts
- `OnStreamEnded`: will be invoked when stream ends
- `OnSongChanged`: will be invoked when new SongInfo is ready

Notes:
  - All event handler will be automatically wrapped as async event handler by `PlugginManager`
