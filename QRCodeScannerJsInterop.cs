using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace ReactorBlazorQRCodeScanner
{ 
    public class QRCodeScannerJsInterop : IAsyncDisposable
    {
        private static Action<string>? _onQrCodeScanAction;

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
        

        private static DateTime? _lastScannedValueDateTime;
        private static int _scanInterval = 2000;

        [JSInvokable]
        public static Task<string> QRCodeJsCallBack(string value)
        {
            if(_lastScannedValueDateTime == null)
            {
                _lastScannedValueDateTime = DateTime.Now;
                DoSomethingAboutThisQRCode(value);
            }

            //Si le dernier scan est assez vieux
            var maxDate = DateTime.Now.AddMilliseconds(-_scanInterval);
            if(_lastScannedValueDateTime < maxDate)
            {
                _lastScannedValueDateTime = DateTime.Now;
                DoSomethingAboutThisQRCode(value);                
            }
            else
            {
                //Le dernier scan est trop récent, pour éviter d'avoir une multitude de requete a traiter on ne fait rien.
            }
        
            return Task.FromResult("retour"); //Inutile, mais bon des fois qu'on ait besoin un jour d'obtenir un retour ici...
        }

        public static void DoSomethingAboutThisQRCode(string code)
        {
            //Console.WriteLine($"QRCodeJsCallBack C# receive value: {code}");

            if(!string.IsNullOrEmpty(code))
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