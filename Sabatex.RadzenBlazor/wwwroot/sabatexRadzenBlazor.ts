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