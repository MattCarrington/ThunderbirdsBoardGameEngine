export function show(dialog, componentReference) {
    if (!dialog || dialog.open) {
        return;
    }

    dialog.addEventListener("cancel", event => {
        event.preventDefault();

        componentReference.invokeMethodAsync(
            "HandleDialogCancelled");
    });

    dialog.showModal();
}

export function close(dialog) {
    if (dialog?.open) {
        dialog.close();
    }
}