# R-George-B

I wrote a little .Net CLI to control the RGB LEDs on my gaming PC. I use the [OpenRGB.NET](https://github.com/diogotr7/OpenRGB.NET/) client to control (via TCP, as it turns out) [OpenRGB](https://gitlab.com/CalcProgrammer1/OpenRGB/).

The existing RGB software ecosystem is bad. Most RGB control programs only support the same devices that the manufacturer made, and many are achingly slow, bloated and awful on top of that (looking at you MSI Center - the most egregious in a bad field - but special mentions to Asus Armory Crate and Corsair's iCue).

Whilst [SignalRGB](https://signalrgb.com) is good, and only mildly bloated, it pushes a 2D metaphor for most of the control on you. This has potential to be really impressive, sadly, it's let down by hardware. Most motherboards have just 2 or 3 12V ARGB headers, and you can buy RGB splitter boards but they duplicate the signal instead of splitting it. What this means is that, if you buy multiple RGB case fans or strips, you CANNOT layout a meaningful 2D geometry. And with coolers and cases these days often coming with 2 or more RGB fans, you can quickly exceed the headers on your motherboard.

OpenRGB is the best RGB software I've used so far, though its interface looks primitive and the plugin system is difficult to use (although I'm certain a patient user could replicate SignalRGB's functionality using it, I haven't)

Luckily for me, I am a software dev, so I can easily use the neat interface OpenRGB provides, and existing open source clients. It would be nice if all users could get the same experience as me, but that's not going to happen anytime soon.

## Stupid name
Yeah, it is really stupid. Personal use only.