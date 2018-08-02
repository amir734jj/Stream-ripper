# Stream-ripper
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
