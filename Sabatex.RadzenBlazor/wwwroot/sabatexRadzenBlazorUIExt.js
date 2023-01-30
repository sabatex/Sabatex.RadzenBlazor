"use strict";
// This is a JavaScript module that is loaded on demand. It can export any number of
// functions, and may import other JavaScript modules if required.
Object.defineProperty(exports, "__esModule", { value: true });
exports.focusElement = exports.showPrompt = void 0;
function showPrompt(message) {
    return prompt(message, 'Type anything here');
}
exports.showPrompt = showPrompt;
function focusElement() {
    var elements = document.getElementsByClassName("rz-state-highlight rz-data-row");
    if (elements.length > 0)
        elements[0].scrollIntoView({ block: "center" });
}
exports.focusElement = focusElement;
//# sourceMappingURL=sabatexRadzenBlazorUIExt.js.map