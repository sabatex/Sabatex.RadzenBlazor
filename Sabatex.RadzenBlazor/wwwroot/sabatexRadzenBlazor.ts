// This is a JavaScript module that is loaded on demand. It can export any number of
// functions, and may import other JavaScript modules if required.

export function showPrompt(message: string) {
    return prompt(message, 'Type anything here');
}

export function focusElement() {
    var elements = document.getElementsByClassName("rz-state-highlight rz-data-row");
    if (elements.length > 0)
        elements[0].scrollIntoView({ block: "center" });

}

export function downloadStrigAsFile(fileName:string,content:string){
    let link = document.createElement("a");
    link.download = fileName;
    let data = new Blob([content],{type:'text/plain'});
    link.href = window.URL.createObjectURL(data);
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}

export function getElementClientHeight(element:Element):number{
    return element.clientHeight;    
}
export function getElementOffSetHeight(element:HTMLBaseElement):number{
    return element.offsetHeight;    
}

export function getAvailHeight(element:HTMLBaseElement):number{
    return  window.screen.availHeight - element.offsetHeight - element.style.marginTop.length- element.style.marginBottom.length;    
}