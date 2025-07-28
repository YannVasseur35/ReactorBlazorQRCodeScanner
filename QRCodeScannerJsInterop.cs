using Microsoft.JSInterop;

namespace ReactorBlazorQRCodeScanner
{
    public class QRCodeScannerJsInterop : IAsyncDisposable
    {
        private static DateTime? _lastScannedValueDateTime;
        private static int _scanInterval = 2000;

        private static Func<string, ValueTask>? _onQrCodeScanAction;
        private static Func<string, ValueTask>? _onCameraPermissionFailedAction;

        private readonly Lazy<Task<IJSObjectReference>> moduleTask;

        public QRCodeScannerJsInterop(IJSRuntime jsRuntime)
        {
            moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
                "import", "./_content/ReactorBlazorQRCodeScanner/qrCodeScannerJsInterop.js").AsTask());
        }

        public ValueTask Init(Action<string> onQrCodeScanAction, bool useFrontCamera = false, bool flipHorizontal = false)
        {
            return Init(code => { onQrCodeScanAction?.Invoke(code); return ValueTask.CompletedTask; },
                useFrontCamera, flipHorizontal);
        }

        public ValueTask Init(Action<string> onQrCodeScanAction, Action<string> onCameraPermissionFailedAction
            , bool useFrontCamera = false, bool flipHorizontal = false)
        {
            return Init(code => { onQrCodeScanAction?.Invoke(code); return ValueTask.CompletedTask; },
                value => { onCameraPermissionFailedAction?.Invoke(value); return ValueTask.CompletedTask; },
                useFrontCamera, flipHorizontal);
        }

        public async ValueTask Init(Func<string, ValueTask> onQrCodeScanAction, bool useFrontCamera = false, bool flipHorizontal = false)
        {
            _onQrCodeScanAction = onQrCodeScanAction;

            var module = await moduleTask.Value;
            await module.InvokeVoidAsync("Scanner.Init", new object[2] { useFrontCamera, flipHorizontal });
        }

        public async ValueTask Init(Func<string, ValueTask> onQrCodeScanAction, Func<string, ValueTask> onCameraPermissionFailedAction
            , bool useFrontCamera = false, bool flipHorizontal = false)
        {
            _onQrCodeScanAction = onQrCodeScanAction;
            _onCameraPermissionFailedAction = onCameraPermissionFailedAction;

            var module = await moduleTask.Value;
            await module.InvokeVoidAsync("Scanner.Init", new object[2] { useFrontCamera, flipHorizontal });
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
        public async static Task<string> ManageErrorJsCallBack(string value)
        {
            Console.WriteLine(value);

            if (_onCameraPermissionFailedAction != null)
                await _onCameraPermissionFailedAction(value);

            return "retour"; //Inutile, mais bon des fois qu'on ait besoin un jour d'obtenir un retour ici...
        }


        [JSInvokable]
        public static async Task<string> QRCodeJsCallBack(string value)
        {
            if (_lastScannedValueDateTime == null)
            {
                _lastScannedValueDateTime = DateTime.Now;
                await DoSomethingAboutThisQRCode(value);
            }

            // If the last scan is old enough
            var maxDate = DateTime.Now.AddMilliseconds(-_scanInterval);
            if (_lastScannedValueDateTime < maxDate)
            {
                _lastScannedValueDateTime = DateTime.Now;
                await DoSomethingAboutThisQRCode(value);
            }

            return "retour"; //Inutile, mais bon des fois qu'on ait besoin un jour d'obtenir un retour ici...
        }

        public static ValueTask DoSomethingAboutThisQRCode(string code)
        {
            //Console.WriteLine($"QRCodeJsCallBack C# receive value: {code}");

            if (!string.IsNullOrEmpty(code))
            {
                if (_onQrCodeScanAction != null)
                    return _onQrCodeScanAction(code);
            }

            return ValueTask.CompletedTask;
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