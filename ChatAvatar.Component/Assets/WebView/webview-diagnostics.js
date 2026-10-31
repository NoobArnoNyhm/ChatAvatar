function reportRendererError(message) {
  const details = String(message || "Unknown WebView renderer error");
  const status = document.querySelector("#status");
  if (status) status.textContent = `3D renderer error: ${details}`;

  window.chrome?.webview?.postMessage({
    type: "rendererError",
    protocolVersion: 1,
    errorMessage: details,
  });
}

window.addEventListener("error", (event) => {
  const source = event.filename ? ` (${event.filename}:${event.lineno})` : "";
  const failedResource = event.target instanceof HTMLElement
    ? event.target.src || event.target.href
    : "";
  const message = event.message || (failedResource
    ? `Failed to load ${event.target.tagName.toLowerCase()}: ${failedResource}`
    : "Script or WebGL initialization failed");
  reportRendererError(`${message}${source}`);
}, true);

window.addEventListener("unhandledrejection", (event) => {
  reportRendererError(event.reason?.stack || event.reason?.message || event.reason);
});

setTimeout(() => {
  if (!window.__chatAvatarSceneReady) {
    reportRendererError("No 3D frame was rendered within 10 seconds. Check WebGL availability and canvas dimensions.");
  }
}, 10000);