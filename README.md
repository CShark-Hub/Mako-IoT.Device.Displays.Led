# Mako-IoT.Device.Displays.Led
The cool blink library :) Provides effects for RGB LED such as smooth color transition, fade etc. Both synchronous and asynchronous methods.

## Usage
```c#
//initialize RGB pixel
RgbPixel pixel = new RgbPixel(pixelDriver);

//set pixel color
pixel.SetColor(new Color(255, 0, 0));

//blink nicely
var thread = pixel.BlinkSmoothAsync(cancellationToken);

//transition to new color
pixel.Transition(new Color(255, 0, 255), cancellationToken);

//fade out then fade into new color
var thread = pixel.FadeTransitionAsync(newColor, cancellationToken);
```

## _IPixelDriver_ implementation
You need to provide implementation of _IPixelDriver_ which talks to underlying hardware. This will usually be three PWM channels (for each of the base colors R, G and B). Sample implementation for ESP32 is here: [PwmPixelDriver](https://github.com/CShark-Hub/Mako-IoT.Device.Samples/blob/main/WasteBinsCalendar/src/MakoIoT.Samples.WBC.Device.App/HardwareServices/PwmPixelDriver.cs)
