# ESP32NodeMCU
Playground for nanoframework with ESP32 using an ESP32 NodeMCU Module WLAN WiFi Development Board from AZ delivery.
Borad product page: https://www.az-delivery.de/products/esp32-developmentboard?ls=de

# Getting Started
Open the Package Manager Console in Visual Studio (View > Other Windows > Package Manager Console) and run the following commands:
```
dotnet tool install -g nanoFirmwareFlasher
dotnet tool update -g nanoFirmwareFlasher
nanoff --update --target ESP32_WROOM_32 --serialport COM3 --fwversion 1.6.5-preview.429
```
