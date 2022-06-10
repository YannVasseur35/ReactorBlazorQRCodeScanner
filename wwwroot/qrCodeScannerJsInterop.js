import("./jsQR.js");
var videoObject = null;
var videoStopped = false;

var Scanner = {

    Wait: function (ms) {
        var start = new Date().getTime();
        var end = start;
        while (end < start + ms) {
            end = new Date().getTime();
        }
    },
    QRCodeScanned: function (code) {
        DotNet.invokeMethodAsync("ReactorBlazorQRCodeScanner", "QRCodeJsCallBack", code);
    },
    Init: function () {
        try {
            console.log("init jsQR");
            videoStopped = false;
            videoObject = document.createElement("video");
            var canvasElement = document.getElementById("canvas");
            var canvas = canvasElement.getContext("2d");
            var loadingMessage = document.getElementById("loadingMessage");
            var outputContainer = document.getElementById("output");
            var outputMessage = document.getElementById("outputMessage");
            var outputData = document.getElementById("outputData");

            function drawLine(begin, end, color) {
                canvas.beginPath();
                canvas.moveTo(begin.x, begin.y);
                canvas.lineTo(end.x, end.y);
                canvas.lineWidth = 4;
                canvas.strokeStyle = color;
                canvas.stroke();
            }

            // Use facingMode: environment to attemt to get the front camera on phones
            navigator.mediaDevices
                .getUserMedia({ video: { facingMode: "environment" } })
                .then(function (stream) {
                    videoObject.srcObject = stream;
                    videoObject.setAttribute("playsinline", true); // required to tell iOS safari we don't want fullscreen
                    videoObject.play();
                    requestAnimationFrame(tick);
                });

            function tick() {

                if (videoStopped === true) // if the video has been stopped we don't have to execute the QR reading
                    return;

                if (videoObject.readyState === videoObject.HAVE_ENOUGH_DATA) {

                    loadingMessage.hidden = true;
                    canvasElement.hidden = false;
                    if (outputContainer)
                        outputContainer.hidden = false;

                    canvasElement.height = videoObject.videoHeight;
                    canvasElement.width = videoObject.videoWidth;
                    canvas.drawImage(
                        videoObject,
                        0,
                        0,
                        canvasElement.width,
                        canvasElement.height
                    );
                    var imageData = canvas.getImageData(
                        0,
                        0,
                        canvasElement.width,
                        canvasElement.height
                    );
                    var code = jsQR(imageData.data, imageData.width, imageData.height, {
                        inversionAttempts: "dontInvert",
                    });
                    if (code) {
                        drawLine(
                            code.location.topLeftCorner,
                            code.location.topRightCorner,
                            "#FF3B58"
                        );
                        drawLine(
                            code.location.topRightCorner,
                            code.location.bottomRightCorner,
                            "#FF3B58"
                        );
                        drawLine(
                            code.location.bottomRightCorner,
                            code.location.bottomLeftCorner,
                            "#FF3B58"
                        );
                        drawLine(
                            code.location.bottomLeftCorner,
                            code.location.topLeftCorner,
                            "#FF3B58"
                        );
                        if (outputContainer) {
                            outputMessage.hidden = true;
                            outputData.parentElement.hidden = false;
                            outputData.innerText = code.data;
                        }

                        Scanner.QRCodeScanned(code.data);

                    }
                }
                requestAnimationFrame(tick);
            }

        } catch (error) {
            console.error(error);
        }
    },
    Stop: function () {
        try {
            // if video object is null then we don't have to stop the stream
            if (videoObject === null)
                return;

            if (videoObject.readyState === videoObject.HAVE_ENOUGH_DATA) {
                videoStopped = true; // indicate that the video has been stopped to stop the requestAnimationFrame from ticking

                const mediaStream = videoObject.srcObject;
                const tracks = mediaStream.getTracks();
                tracks.forEach(track => track.stop()) // stop the video stream(s)

                // hide canvas
                var canvasElement = document.getElementById("canvas");
                if (canvasElement != null)
                    canvasElement.hidden = true;

                // clean up videoObject
                videoObject = null;
            }
        } catch (error) {
            console.error(error);
        }

    },

};

export { Scanner };
