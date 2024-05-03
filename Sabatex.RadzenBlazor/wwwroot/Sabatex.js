

export const sabatex = {
    downloadStrigAsFile : function (fileName, content) {
        let link = document.createElement("a");
        link.download = fileName;
        let data = new Blob([content], { type: 'text/plain' });
        link.href = window.URL.createObjectURL(data);
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    },
    showPrompt : function (message) {
        return prompt(message, 'Type anything here');
    },

    focusElement : function () {
        var elements = document.getElementsByClassName("rz-state-highlight rz-data-row");
        if (elements.length > 0)
            elements[0].scrollIntoView({ block: "center" });
    },
    getElementClientHeigh :function (element){
        return element.clientHeight;    
    },
    getElementOffSetHeight : function (element){
        return element.offsetHeight;
    },
    getAvailHeight : function (element){
        return window.screen.availHeight - element.offsetHeight - element.style.marginTop.length - element.style.marginBottom.length;
    },
    localStorageSetItem: function (key, value) {
        localStorage.setItem(key, value);
    },
    localStorageGetItem: function (key) {
        return localStorage.getItem(key);
    },
    getWindowDimensions: function () {
        return { height: window.height, width: window.height, availHeight: window.innerHeight, availWidth: window.innerWidth };
    },

    isDevice:function isDevice() {
        return /android|webos|iphone|ipad|ipod|blackberry|iemobile|opera mini|mobile/i.test(navigator.userAgent);
        }

}

