<!-- [![Contributors][contributors-shield]][contributors-url]
[![Forks][forks-shield]][forks-url]
[![Stargazers][stars-shield]][stars-url]
[![Issues][issues-shield]][issues-url]
[![MIT License][license-shield]][license-url] -->

[![LinkedIn][linkedin-shield]][linkedin-url]

<!-- PROJECT -->

# Blazor QR Code Scanner

An easy implementation of [jsQR](https://github.com/cozmo/jsQR), as a Blazor Component.

[GITHUB](https://github.com/YannVasseur35/ReactorBlazorQRCodeScanner)

<!-- ABOUT THE PROJECT -->

### About The Project

I just needed a component that help me scan QR Code. As I could not found one in a Blazor Component, I did it myself, using the wonderfull [jsQR](https://github.com/cozmo/jsQR) javascript library. (all greets to him/her/them)

### Requirement

Should work with .net Core 3.1. Works with .net 6.0 and 7.0

### Getting Started

Add nuget package : [ReactorBlazorQRCodeScanner](https://www.nuget.org/packages/ReactorBlazorQRCodeScanner/)

- Package Manager
  ```sh
  Install-Package ReactorBlazorQRCodeScanner
  ```
- dotnet cli
  ```sh
  dotnet add package ReactorBlazorQRCodeScanner
  ```

### Usage

Open a razor page or a component

1. add reference to the lib and to JsRuntime
   ```dotnet
   @inject IJSRuntime JS
   @using ReactorBlazorQRCodeScanner
   ```
2. add this somewhere in your code
   ```html
   <QRCodeScanner />
   ```
3. In the code section, add this

   ```dotnet
    @code {

      private QRCodeScannerJsInterop? _qrCodeScannerJsInterop;
      private Action<string>? _onQrCodeScanAction; 

      protected override async Task OnInitializedAsync()
      {
         _onQrCodeScanAction = (code) => OnQrCodeScan(code);  

          _qrCodeScannerJsInterop = new QRCodeScannerJsInterop(JS);
          await _qrCodeScannerJsInterop.Init(_onQrCodeScanAction);
      }

      private static void OnQrCodeScan(string code)
      {
          Console.WriteLine($"OnQrCodeScan {code}");
      }
   }
   ```

4. run your project
   ```sh
   dotnet watch run
   ```

### Code explaination

```dotnet
private QRCodeScannerJsInterop? _qrCodeScannerJsInterop;
```

\_qrCodeScannerJsInterop is the way to communicate with the ReactorBlazorQRCodeScanner Component "QRCodeScanner"

```dotnet
private Action<string> _onQrCodeScanAction = (code) => OnQrCodeScan(code);
```

\_onQrCodeScanAction is an action that call OnQrCodeScan when invoked. (invokation is done in the ReactorBlazorQRCodeScanner lib when it detects a QR Code throw the cam)

```dotnet
protected override async Task OnInitializedAsync()
      {
          _qrCodeScannerJsInterop = new QRCodeScannerJsInterop(JS);
          await _qrCodeScannerJsInterop.Init(_onQrCodeScanAction);
      }
```

On your page/componenent initialization, you need to build the QRCodeScannerJsInterop object and the run the Init method that takes one parameter : an Action (delegate).

When this one is Invoked, it fires the methods "OnQrCodeScan" where you can treat your QRCode data.

```dotnet
protected async Task StopQRScan()
      {
          await _qrCodeScannerJsInterop.StopRecording();
      }
```

With "StopRecording" you can cancel the QR recording / video stream so that the browser will no longer use the webcam and the webcam icon in the browser (red dot) will dissapear.

### Options

- You can show/hide the output line that indicates the result of the QRCode when scanned. Default is true (visible)

```dotnet
<QRCodeScanner ShowOutput="false"/>
```

- You can set a custom loading message with the parameter "LoadingMessage", the default message is "Loading video stream (please make sure you have a camera enabled)".

```dotnet
<QRCodeScanner LoadingMessage="My custom loading message"/>
```

- You can set a custom output message, when no QR code is scanned, with the parameter "OutputMessage", the default message is "No QR code detected.".

```dotnet
<QRCodeScanner OutputMessage="My custom no QR code found message"/>
```

- You can set a width in pixel (W/H ratio repected) :

```dotnet
<QRCodeScanner Width="100"/>
```

or in % of the screen width size (W/H ratio repected) :

```dotnet
<QRCodeScanner Width="90%"/>
```

Width default will be the size of your camera video stream. 

### TroubleShooting

You need to have a camera on your device.

It can works on a laptop with a webcam, or it could be a phone's camera

The webbrowser will ask you permission to use the camera.

Until you accept it (or refuse it), a message will be displayed :

```text
Loading video stream (please make sure you have a camera enabled)
```

 <!-- CONTRIBUTING -->

## Contributing

Contributions are what make the open source community such an amazing place to learn, inspire, and create. Any contributions you make are **greatly appreciated**.

If you have a suggestion that would make this better, please fork the repo and create a pull request. You can also simply open an issue with the tag "enhancement".
Don't forget to give the project a star! Thanks again!

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

<!-- LICENSE -->

## License

Distributed under the MIT License. See `LICENSE.txt` for more information.

<!-- CONTACT -->

## Contact

Yann Vasseur - [@YannVasseur](https://twitter.com/YannVasseur) - contact@reactor.fr

Project Link: [https://github.com/YannVasseur35/ReactorBlazorQRCodeScanner](https://github.com/YannVasseur35/ReactorBlazorQRCodeScanner)

<!-- MARKDOWN LINKS & IMAGES -->
<!-- https://www.markdownguide.org/basic-syntax/#reference-style-links -->

<!--
[contributors-shield]: https://img.shields.io/github/contributors/othneildrew/Best-README-Template.svg?style=for-the-badge
[contributors-url]: https://github.com/othneildrew/Best-README-Template/graphs/contributors
[forks-shield]: https://img.shields.io/github/forks/othneildrew/Best-README-Template.svg?style=for-the-badge
[forks-url]: https://github.com/othneildrew/Best-README-Template/network/members
[stars-shield]: https://img.shields.io/github/stars/othneildrew/Best-README-Template.svg?style=for-the-badge
[stars-url]: https://github.com/othneildrew/Best-README-Template/stargazers
[issues-shield]: https://img.shields.io/github/issues/othneildrew/Best-README-Template.svg?style=for-the-badge
[issues-url]: https://github.com/othneildrew/Best-README-Template/issues
[license-shield]: https://img.shields.io/github/license/othneildrew/Best-README-Template.svg?style=for-the-badge
[license-url]: https://github.com/othneildrew/Best-README-Template/blob/master/LICENSE.txt
[linkedin-shield]: https://img.shields.io/badge/-LinkedIn-black.svg?style=for-the-badge&logo=linkedin&colorB=555
[linkedin-url]: https://linkedin.com/in/othneildrew
[product-screenshot]: images/screenshot.png

-->

[linkedin-shield]: https://img.shields.io/badge/-LinkedIn-black.svg?style=for-the-badge&logo=linkedin&colorB=555
[linkedin-url]: https://www.linkedin.com/in/yannvasseur/
[product-screenshot]: images/screenshot.png
