// <copyright file="SSD1306_Command.cs" company="RolandoMagico">
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
  /// <summary>
  /// Enumeration of command for the <see cref="SSD1306"/> driver.
  /// </summary>
  public enum SSD1306_Command : byte
  {
    /// <summary>
    /// Enable Charge Pump.
    /// </summary>
    EnableChargePump = 0x14,

    /// <summary>
    /// Set Memory Adressing Mode.
    /// </summary>
    SetMemoryAddressingMode = 0x20,

    /// <summary>
    /// Set Column Address.
    /// </summary>
    SetColumnAddress = 0x21,

    /// <summary>
    /// Set Page Address.
    /// </summary>
    SetPageAddress = 0x22,

    /// <summary>
    /// Continuous Horizontal Scroll Setup: Right Horizontal Scroll.
    /// </summary>
    ContinuousHorizontalScrollSetup_RightScroll = 0x26,

    /// <summary>
    /// Continuous Horizontal Scroll Setup: Left Horizontal Scroll.
    /// </summary>
    ContinuousHorizontalScrollSetup_LeftScroll = 0x27,

    /// <summary>
    /// Deactivate scroll.
    /// </summary>
    DeactivateScroll = 0x2E,

    /// <summary>
    /// Activate scroll.
    /// </summary>
    ActivateScroll = 0x2F,

    /// <summary>
    /// Set Display Start Line.
    /// </summary>
    SetDisplayStartLine = 0x40,

    /// <summary>
    /// Set contrast control for BANK0.
    /// </summary>
    SetContrastControlForBank0 = 0x81,

    /// <summary>
    /// Charge Pump Setting.
    /// </summary>
    ChargePumpSetting = 0x8D,

    /// <summary>
    /// Set Segment Re-map. Column address 0 is mapped to SEG0.
    /// </summary>
    SetSegmentRemap_Adress0IsMappedToSeg0 = 0xA0,

    /// <summary>
    /// Set Segment Re-map. Column address 127 is mapped to SEG0.
    /// </summary>
    SetSegmentRemap_Adress127IsMappedToSeg0 = 0xA1,

    /// <summary>
    /// Entire Display ON. Output follows RAM content.
    /// </summary>
    EntireDisplayOn_FollowRamContent = 0xA4,

    /// <summary>
    /// Entire Display ON. Output ignores RAM content.
    /// </summary>
    EntireDipslayOn_IngoreRamContent = 0xA5,

    /// <summary>
    /// Set Normal Display.
    /// </summary>
    SetNormalDisplay = 0xA6,

    /// <summary>
    /// Set Inverse Display.
    /// </summary>
    SetInverseDisplay = 0xA7,

    /// <summary>
    /// Set multiplex ratio.
    /// </summary>
    SetMultiplexRatio = 0xA8,

    /// <summary>
    /// Set Display OFF.
    /// </summary>
    SetDisplayOff = 0xAE,

    /// <summary>
    /// Set Display ON.
    /// </summary>
    SetDisplayOn = 0xAF,

    /// <summary>
    /// Set COM Output Scan Directioin.
    /// Scan from COM0 to COM[N-1].
    /// </summary>
    SetCOMOutputScanDirection_NormalMode = 0xC0,

    /// <summary>
    /// Set COM Output Scan Directioin.
    /// Scan from COM[N-1] to COM0.
    /// </summary>
    SetCOMOutputScanDirection_RemappedMode = 0xC8,

    /// <summary>
    /// Set Display Offset.
    /// </summary>
    SetDisplayOffset = 0xD3,

    /// <summary>
    /// Set Display Clock Divide Ratio/ Oscillator Frequency.
    /// </summary>
    SetDisplayClockDivideRatio_OscillatorFrequency = 0xD5,

    /// <summary>
    /// Set COM Pins Hardware configuration.
    /// </summary>
    SetComPinsHardwareConfiguration = 0xDA,
  }
}
