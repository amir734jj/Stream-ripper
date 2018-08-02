# Stream-ripper

Stream Ripper library, convert an online radio to your music library!

Example:

```csharp
streamRipper = new StreamRipper("https://rj1.rjstream.com/",
    PlugginManagerBuilder
        .New()
        .SetOnSongChanged(x =>
        {
            songInfo = x.SongInfo;
            // ReSharper disable once AccessToModifiedClosure
            // ReSharper disable once PossibleNullReferenceException
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
