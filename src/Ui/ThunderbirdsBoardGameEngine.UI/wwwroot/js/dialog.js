export function show(dialog, componentReference) {
    if (!dialog || dialog.open) {
        return;
    }

    dialog.addEventListener("cancel", () => {
        componentReference.invokeMethodAsync("HandleDialogCancelled");
    }, { once: true });

    dialog.showModal();
}

export function close(dialog) {
    if (dialog?.open) {
        dialog.close();
    }
}