// <copyright file="SSD1306_Display.cs" company="RolandoMagico">
// MIT License
//
// Copyright (c) 2020 by RolandoMagico
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// </copyright>

namespace Display.SSD1306
{
  using System;
  using System.Device.I2c;
  using Display.Fonts;

  /// <summary>
  /// Driver for a SSD1306 compatible display.
  /// Based on https://github.com/sharmavishnu/nf-companion/tree/master/nf-companion-lib-drivers-display.
  /// But using System.Device.I2c instead of Windows.Devices.I2c.
  /// </summary>
  public class SSD1306_Display
  {
    /// <summary>
    /// The I2C device instance which is used for I2C communication.
    /// </summary>
    private readonly I2cDevice i2cDevice;

    /// <summary>
    /// Display buffer.
    /// </summary>
    private byte[] displayBbuffer;

    /// <summary>
    /// Initializes a new instance of the <see cref="SSD1306_Display"/> class.
    /// </summary>
    /// <param name="busId">The ID of the I2C bus.</param>
    /// <param name="deviceAddress">The I2C address of the device.</param>
    /// <param name="width">Width of OLED display (in pixel).</param>
    /// <param name="height">Height of OLED display (in pixel).</param>
    public SSD1306_Display(int busId, int deviceAddress = 0x3C, int width = 128, int height = 32)
    {
      this.i2cDevice = I2cDevice.Create(new I2cConnectionSettings(busId, deviceAddress, I2cBusSpeed.StandardMode));
      this.Width = width;
      this.Height = height;
      this.Pages = height / 8;
      this.displayBbuffer = new byte[this.Pages * this.Width];
    }

    /// <summary>
    /// Gets the display width.
    /// </summary>
    public int Width { get; private set; }

    /// <summary>
    /// Gets the display height.
    /// </summary>
    public int Height { get; private set; }

    /// <summary>
    /// Gets the displays number of pages.
    /// </summary>
    public int Pages { get; private set; }

    /// <summary>
    /// Gets the currently used font.
    /// </summary>
    public IPixelFont Font { get; private set; } = new PixelFont7X9();

    /// <summary>
    /// Gets the font width.
    /// </summary>
    public int FontWidth => this.Font.Width;

    /// <summary>
    /// Gets the font height.
    /// </summary>
    public int FontHeight => this.Font.Height;

    /// <summary>
    /// Initializes the display.
    /// </summary>
    public void Initialize()
    {
      // NOTE: We do not need constants for these commands...
      // They are not to be reused by either other classes or by SDK user

      // Ensure display is switched off
      this.SendCommand(SSD1306_Command.SetDisplayOff);

      // Set mux ratio...this tells the controller to use how many row numbers in the display (0-63)
      this.SendCommand(SSD1306_Command.SetMultiplexRatio);
      this.SendCommand((byte)(this.Height - 1));

      // Set display offset...we want to start from 0 (the shift by COM)
      this.SendCommand(SSD1306_Command.SetDisplayOffset);
      this.SendCommand((byte)0x00);

      // Set display line address in display RAM ...we want to start from 0
      this.SendCommand(SSD1306_Command.SetDisplayStartLine);

      // Set the segment re-map...(Segment 0 is mapped to column 0)
      // TODO: Change this as per your hardware design
      // Set scan direction. We want it to scan from left to right (COM0 - COM[N-1])
      this.SendCommand(SSD1306_Command.SetSegmentRemap_Adress127IsMappedToSeg0);

      // TODO: Change this as per your hardware design
      // Set COM pins hardware configuration, this is hardware dependent. See page 40 in ssd1306 datasheet
      this.SendCommand(SSD1306_Command.SetCOMOutputScanDirection_RemappedMode);

      this.SendCommand(SSD1306_Command.SetComPinsHardwareConfiguration);
      this.SendCommand(0x02);

      // Set the contrast...just in the middle
      this.SetContrast(0x7F);

      // Set to resume from RAM content display
      this.SendCommand(SSD1306_Command.EntireDisplayOn_FollowRamContent);

      // Set normal display
      this.SendCommand(SSD1306_Command.SetNormalDisplay);

      // Set oscillator frequency
      this.SendCommand(SSD1306_Command.SetDisplayClockDivideRatio_OscillatorFrequency);
      this.SendCommand(0x80);

      // Enable charge pump regulator
      // TODO: Change this as per your hardware design
      // Set memory addressing mode to page address mode, horizontal addressing
      this.SendCommand(SSD1306_Command.ChargePumpSetting);
      this.SendCommand(SSD1306_Command.EnableChargePump);

      this.SendCommand(SSD1306_Command.SetMemoryAddressingMode);
      this.SendCommand((byte)0x00);

      // Finally set the display ON
      this.SendCommand(SSD1306_Command.SetDisplayOn);
    }

    /// <summary>
    /// Prepare display to write data..basically resets the internal buffer.
    /// </summary>
    public void PrepareToWrite()
    {
      if (this.displayBbuffer != null && this.displayBbuffer.Length > 0)
      {
        Array.Clear(this.displayBbuffer, 0, this.displayBbuffer.Length);
      }
    }

    /// <summary>
    /// Write buffer to display.
    /// </summary>
    public void Write()
    {
      this.SendCommand(SSD1306_Command.SetColumnAddress);
      this.SendCommand((byte)0);
      this.SendCommand((byte)(this.Width - 1));

      // set page address
      this.SendCommand(SSD1306_Command.SetPageAddress);
      this.SendCommand((byte)0);
      this.SendCommand((byte)(this.Pages - 1));

      for (ushort i = 0; i < this.displayBbuffer.Length; i = (ushort)(i + 16))
      {
        this.SendCommand(SSD1306_Command.SetDisplayStartLine);

        // SendArray(buffer, i, (ushort)(i + 16));
        this.SendData(this.displayBbuffer, i, 16);
      }
    }

    /// <summary>
    /// Reset and initialize the display.
    /// </summary>
    public void Reset()
    {
      // The particular model, that works on I2C does not have any external reset.
      // Just re-initializing the display--soft reset
      this.Initialize();
      this.SetFont(new PixelFont7X9());
      this.SetContrast();
      this.SetInverseDisplay(false);
      this.StopScrolling();
      this.PrepareToWrite();
      this.Write();
    }

    /// <summary>
    /// Set the font object to use to render text elements.
    /// </summary>
    /// <param name="font">ILCDFont instance.</param>
    public void SetFont(IPixelFont font)
    {
      this.Font = font ?? throw new ArgumentNullException("font");
    }

    /// <summary>
    /// Set contrast.
    /// </summary>
    /// <param name="value">0x00 to 0xFF.</param>
    public void SetContrast(byte value = 0xFF)
    {
      this.SendCommand(SSD1306_Command.SetContrastControlForBank0);
      this.SendCommand(value);
    }

    /// <summary>
    /// Inverse the display (painted pixels become off and vice versa).
    /// </summary>
    /// <param name="inverse">If true,display is inversed.</param>
    public void SetInverseDisplay(bool inverse)
    {
      if (inverse)
      {
        this.SendCommand(SSD1306_Command.SetInverseDisplay);
      }
      else
      {
        this.SendCommand(SSD1306_Command.SetNormalDisplay);
      }
    }

    /// <summary>
    /// Switch on the whole display.
    /// </summary>
    /// <param name="setOn">if true, display switches on.</param>
    public void SetEntireDisplayON(bool setOn)
    {
      if (setOn)
      {
        this.SendCommand(SSD1306_Command.EntireDipslayOn_IngoreRamContent);
      }
      else
      {
        this.SendCommand(SSD1306_Command.EntireDisplayOn_FollowRamContent);
      }
    }

    /// <summary>
    /// Start horizontall scrolling.
    /// </summary>
    /// <param name="left">true = scrolling to left, false = scroll to right.</param>
    public void StartHorizontalScroll(bool left)
    {
      this.StopScrolling();

      if (left)
      {
        this.SendCommand(SSD1306_Command.ContinuousHorizontalScrollSetup_LeftScroll);
      }
      else
      {
        this.SendCommand(SSD1306_Command.ContinuousHorizontalScrollSetup_RightScroll);
      }

      this.SendCommand((byte)0x00);

      // start page index
      this.SendCommand((byte)0);

      // scroll interval in frames
      this.SendCommand((byte)0x00);

      // end page index
      this.SendCommand((byte)(this.Pages - 1));
      this.SendCommand((byte)0x00);
      this.SendCommand(0xFF);

      // start scroll
      this.SendCommand(SSD1306_Command.ActivateScroll);
    }

    /// <summary>
    /// Turn off scrolling.
    /// </summary>
    public void StopScrolling()
    {
      this.SendCommand(SSD1306_Command.DeactivateScroll);
    }

    /// <summary>
    /// Clear the OLED screen.
    /// </summary>
    public void ClearScreen()
    {
      this.PrepareToWrite();
      this.Write();
    }

    /// <summary>
    /// Set the pixel at a given position.
    /// </summary>
    /// <param name="xPos">The x-position.</param>
    /// <param name="yPos">The y-position.</param>
    /// <param name="on">If true, the pixel is turned on, else its turned off.</param>
    public void SetPixel(byte xPos, byte yPos, bool on = true)
    {
      if (xPos < 0 || xPos > this.Width)
      {
        return;
      }

      if (yPos < 0 || yPos > this.Height)
      {
        return;
      }

      int index = ((yPos / 8) * this.Width) + xPos;

      if (on)
      {
        this.displayBbuffer[index] = (byte)(this.displayBbuffer[index] | (byte)(1 << (yPos % 8)));
      }
      else
      {
        this.displayBbuffer[index] = (byte)(this.displayBbuffer[index] & ~(byte)(1 << (yPos % 8)));
      }
    }

    /// <summary>
    /// Draw a line.
    /// </summary>
    /// <param name="startX">Starting x-pos.</param>
    /// <param name="startY">Starting y-pos.</param>
    /// <param name="endX">Ending x-pos.</param>
    /// <param name="endY">Ending y-pos.</param>
    /// <param name="on">If true, the line pixels are turned on, else its turned off.</param>
    public void DrawLine(byte startX, byte startY, byte endX, byte endY, bool on = true)
    {
      if (startX < 0 || startX > this.Width)
      {
        throw new ArgumentException("Invalid start x-pos");
      }

      if (endX < 0 || endX > this.Width || endX < startX)
      {
        throw new ArgumentException("Invalid end x-pos");
      }

      if (startY < 0 || startY > this.Height)
      {
        throw new ArgumentException("Invalid start y-pos");
      }

      if (endY < 0 || endY > this.Height || endY < startY)
      {
        throw new ArgumentException("Invalid end y-pos");
      }

      if (endX == this.Width)
      {
        endX -= 1;
      }

      if (startX == this.Width)
      {
        startX -= 1;
      }

      if (endY == this.Height)
      {
        endY -= 1;
      }

      if (startY == this.Height)
      {
        startY -= 1;
      }

      for (byte xpos = startX; xpos <= endX; xpos++)
      {
        for (byte ypos = startY; ypos <= endY; ypos++)
        {
          this.SetPixel(xpos, ypos, on);
        }
      }
    }

    /// <summary>
    /// Draw a rectangle.
    /// </summary>
    /// <param name="x">The starting x-pos.</param>
    /// <param name="y">The starting y-pos.</param>
    /// <param name="width">The rectangle width.</param>
    /// <param name="height">The rectangle height.</param>
    /// <param name="on">If true, the line pixels are turned on, else its turned off.</param>
    public void DrawRectangle(byte x, byte y, byte width, byte height, bool on = true)
    {
      if (x < 0 || width <= 0 || width > this.Width)
      {
        throw new ArgumentOutOfRangeException("x,width");
      }

      if (y < 0 || height <= 0 || height > this.Height)
      {
        throw new ArgumentOutOfRangeException("y,height");
      }

      if (x == width)
      {
        x -= 1;
      }

      if (y == height)
      {
        y -= 1;
      }

      // Draw top horizontal
      this.DrawLine(x, y, (byte)(x + width - 1), y, on);

      // draw right vertical
      this.DrawLine((byte)(x + width - 1), y, (byte)(x + width - 1), (byte)(y + height - 1), on);

      // draw bottom horizontal
      this.DrawLine(x, (byte)(height - 1), (byte)(x + width - 1), (byte)(height - 1), on);

      // draw left vertical
      this.DrawLine(x, y, x, (byte)(y + height - 1));
    }

    /// <summary>
    /// Draw the given character.
    /// </summary>
    /// <param name="x">The character x position.</param>
    /// <param name="y">The character y position.</param>
    /// <param name="c">The character to print.</param>
    public void DrawChar(byte x, byte y, char c)
    {
      if (((int)c - 32) >= this.Font.CharMap.Length)
      {
        c = '?';
      }

      for (int yPos = 0; yPos < this.Font.Height; yPos++)
      {
        uint pixMapHorizontal = this.Font.CharMap[((c - 32) * this.Font.Height) + yPos];

        // left to right
        uint cmp = 0x80000000u;
        for (int xPos = 0; xPos < this.Font.Width; xPos++)
        {
          if ((pixMapHorizontal & cmp) == cmp)
          {
            this.SetPixel((byte)(xPos + x), (byte)(yPos + y), true);
          }

          cmp = (uint)(cmp >> 1);
        }
      }
    }

    /// <summary>
    /// Draw the given text.
    /// </summary>
    /// <param name="x">Starting x position.</param>
    /// <param name="y">Starting y position.</param>
    /// <param name="text">Text to draw.</param>
    public void DrawText(byte x, byte y, string text)
    {
      char[] chars = text.ToCharArray();
      byte originX = x;
      foreach (char c in chars)
      {
        if (c == '\r')
        {
          continue;
        }

        if (c == '\n')
        {
          y += (byte)(this.Font.Height + 1);
          continue;
        }

        // TODO...next line
        if ((x + this.Font.Width) > this.Width)
        {
          // wrap
          x = originX;
          y += (byte)(this.Font.Height + 1);
        }

        if ((y + this.Font.Height) > this.Height)
        {
          // nothing more to draw
          break;
        }

        this.DrawChar(x, y, c);
        x += (byte)(this.Font.Width + 1);
      }
    }

    /// <summary>
    /// TODO: Dispose the object.
    /// </summary>
    protected void DisposeActuator()
    {
      this.displayBbuffer = null;

      if (this.i2cDevice != null)
      {
        this.i2cDevice.Dispose();
      }
    }

    /// <summary>
    /// Send an array of bytes to the display controller.
    /// </summary>
    /// <param name="data">Data array to send.</param>
    private void Send(byte[] data) => this.i2cDevice.Write(data);

    /// <summary>
    /// Send a command to the display controller.
    /// </summary>
    /// <param name="cmd">Command to send.</param>
    private void SendCommand(byte cmd)
    {
      // Co = 0, D/C = 0
      this.Send(new byte[] { 0x00, cmd });
    }

    /// <summary>
    /// Send a command to the display controller.
    /// </summary>
    /// <param name="command">The Command to send.</param>
    private void SendCommand(SSD1306_Command command) => this.SendCommand((byte)command);

    /// <summary>
    /// Send data as byte array to the display controller.
    /// </summary>
    /// <param name="data">Data array to send.</param>
    /// <param name="start">Index of starting element to send from.</param>
    /// <param name="len">Number of elements to send.</param>
    private void SendData(byte[] data, int start, int len)
    {
      // Co = 0, D/C = 1
      byte[] buf = new byte[len + 1];
      Array.Copy(data, start, buf, 1, len);
      buf[0] = 0x40;
      this.Send(buf);
    }
  }
}
