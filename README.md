# Stream-ripper

Stream Ripper library, convert an online radio to your music library!

[Nuget](https://www.nuget.org/packages/StreamRipper/)

Example:

```csharp
var streamRipper = StreamRipper.New(new Uri("https://rj1.rjstream.com/"));

// The recommended way is to have an async event handlers
streamRipper.SongChangedEventHandlers += async (_, arg) =>
{
    // Create filename from SongInfo
    var filename = $"{arg.SongInfo.SongMetadata.Artist}-{arg.SongInfo.SongMetadata.Title}";

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
