window.checkImageIsSquare = (inputId, dotNetObjRef) => {
    const input = document.getElementById(inputId);
    if (!input || !input.files || input.files.length === 0) {
        dotNetObjRef.invokeMethodAsync('ReceiveImageCheckResult', false);
        return;
    }

    const file = input.files[0];
    const img = new Image();

    img.onload = function () {
        console.log('Width:', img.naturalWidth, 'Height:', img.naturalHeight);
        const isSquare = img.naturalWidth === img.naturalHeight;
        dotNetObjRef.invokeMethodAsync('ReceiveImageCheckResult', isSquare);
        URL.revokeObjectURL(img.src);
    };

    img.onerror = function () {
        dotNetObjRef.invokeMethodAsync('ReceiveImageCheckResult', false);
    };

    img.src = URL.createObjectURL(file);
};
    