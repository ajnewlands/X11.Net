# X11.Net
The project implements an interface to the Xlib / X11 / X windowing system libraries from .NET core/C#. It began prosaically enough; I needed to do make some simple Xlib calls from a C# program and developed from there.

It covers most of the core Xlib as well as some elements of Xmu and Xcb.

It deliberately cleaves close to the C API rather than trying to cater to more modern approaches (with the exception of some niceties, such as C# enum's to aid intellisense in suggesting sensible values for the assorted 'long' fields in the C API).

This is because Xlib is by now some 30 years old and there is a large body of documentation describing this interface; not only the man pages by also the Xlib programming manual by Christophe Tronche (ref https://tronche.com/gui/x/xlib/) and, of course, the very large amount of source code for various X applications on the web.

A relatively straightforward usage example is available from https://github.com/ajnewlands/cswm.



Lastly, I would like to acknowledge those who have contributed to this library, namely Rodney Morris and Marcus Wichelmann.


