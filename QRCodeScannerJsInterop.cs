using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace ReactorBlazorQRCodeScanner
{
    public class QRCodeScannerJsInterop : IAsyncDisposable
    {
        private static DateTime? _lastScannedValueDateTime;
        private static int _scanInterval = 2000;

        private static Action<string>? _onQrCodeScanAction;
        private static Action<string>? _onCameraPermissionFailedAction;

        private readonly Lazy<Task<IJSObjectReference>> moduleTask;

        public QRCodeScannerJsInterop(IJSRuntime jsRuntime)
        {
            moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
                "import", "./_content/ReactorBlazorQRCodeScanner/qrCodeScannerJsInterop.js").AsTask());
        }

        public async ValueTask Init(Action<string> onQrCodeScanAction)
        {
            _onQrCodeScanAction = onQrCodeScanAction;

            var module = await moduleTask.Value;
            await module.InvokeVoidAsync("Scanner.Init");
        }

        public async ValueTask Init(Action<string> onQrCodeScanAction, Action<string> onCameraPermissionFailedAction)
        {
            _onQrCodeScanAction = onQrCodeScanAction;
            _onCameraPermissionFailedAction = onCameraPermissionFailedAction;

            var module = await moduleTask.Value;
            await module.InvokeVoidAsync("Scanner.Init");
        }

        public async ValueTask StopRecording()
        {
            if (moduleTask.IsValueCreated)
            {
                var module = await moduleTask.Value;
                await module.InvokeVoidAsync("Scanner.Stop");
            }
        }



        [JSInvokable]
        public static Task<string> ManageErrorJsCallBack(string value)
        {
            Console.WriteLine(value);

            _onCameraPermissionFailedAction?.Invoke(value);

            return Task.FromResult("retour"); //Inutile, mais bon des fois qu'on ait besoin un jour d'obtenir un retour ici...
        }


        [JSInvokable]
        public static Task<string> QRCodeJsCallBack(string value)
        {
            if (_lastScannedValueDateTime == null)
            {
                _lastScannedValueDateTime = DateTime.Now;
                DoSomethingAboutThisQRCode(value);
            }

            // If the last scan is old enough
            var maxDate = DateTime.Now.AddMilliseconds(-_scanInterval);
            if (_lastScannedValueDateTime < maxDate)
            {
                _lastScannedValueDateTime = DateTime.Now;
                DoSomethingAboutThisQRCode(value);
            }

            return Task.FromResult("retour"); //Inutile, mais bon des fois qu'on ait besoin un jour d'obtenir un retour ici...
        }

        public static void DoSomethingAboutThisQRCode(string code)
        {
            //Console.WriteLine($"QRCodeJsCallBack C# receive value: {code}");

            if (!string.IsNullOrEmpty(code))
            {
                _onQrCodeScanAction?.Invoke(code);
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (moduleTask.IsValueCreated)
            {
                var module = await moduleTask.Value;
                await module.DisposeAsync();
            }
        }
    }
}