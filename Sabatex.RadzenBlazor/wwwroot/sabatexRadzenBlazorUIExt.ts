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
