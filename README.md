# TurboJpegWrapperTiny
Partial fork of TurboJpegWrapper to run on .netv4 framework (no special directory structure or nuget install required).

Forked: https://github.com/qmfrederik/AS.TurboJpegWrapper/tree/master/LibJpegWrapper

Will run as console app (test-mode) or .net framework class library.

Ideal for repackaging.

Runs w/64bit bit TurboJpegWrapper (default).
Will support 64 or 32bit by copying contents of 'dll' folder to your package's ApplicationBase folder (=CurrentDirectory).
