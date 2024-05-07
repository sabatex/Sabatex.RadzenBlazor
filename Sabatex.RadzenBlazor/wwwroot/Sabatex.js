

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
    
//radzenBlazorSSRLayout () {
//        document.querySelector('.rz-sidebar-toggle').addEventListener("click", function () {
//            let sidebar = document.querySelector('.rz-sidebar');
//            let isButtonCollapse = sidebar.classList.contains("side-bar-collapse");
//            console.log("isButtonCollapse = " + isButtonCollapse);
//            let isCollapsed = sidebar.clientWidth == 0;
//            console.log("isCollapsed = " + isCollapsed);

//            if (isButtonCollapse) {
//                sidebar.classList.remove("side-bar-collapse");
//                if (sidebar.clientWidth == 0) {
//                    sidebar.classList.remove("rz-sidebar-responsive");
//                }
//                console.log("SideBar Show");
//            } else {
//                if (isCollapsed && sidebar.classList.contains("rz-sidebar-responsive")) {
//                    sidebar.classList.remove("rz-sidebar-responsive");
//                }
//                else {
//                    sidebar.classList.add("side-bar-collapse");
//                    if (!sidebar.classList.contains("rz-sidebar-responsive")) {
//                        sidebar.classList.add("rz-sidebar-responsive");
//                    }
//                }
//                console.log("SideBar Close");
//            }

//        })



