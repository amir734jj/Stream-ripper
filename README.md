# Stream-ripper

Stream Ripper library, convert an online radio to your music library!

[Nuget](https://www.nuget.org/packages/StreamRipper/)

Example:

```csharp
SongInfo songInfo = null;

StreamRipper streamRipper = null;

streamRipper = new StreamRipper(new Uri("https://rj1.rjstream.com/"),
    PlugginManagerBuilder
        .New()
        .SetOnSongChanged(x =>
        {
            songInfo = x.SongInfo;

            // Stop the stream!
            streamRipper.Dispose();
        })
        .Build());
```

Events:
- `OnMetadataChanged`: will be invoked when metadata changes
- `OnStreamUpdate`: will be invoked when new `byte[]` gets downloaded
- `OnStreamStarted`: will be invoked when stream starts
- `OnStreamEnded`: will be invoked when stream ends
- `OnSongChanged`: will be invoked when new SongInfo is ready

Notes:
  - All event handler will be automatically wrapped as async event handler by `PlugginManager`
