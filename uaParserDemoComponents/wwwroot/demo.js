// JavaScript the demos call from .NET (see Interop/DemoJs.cs).

export function userAgent() {
    return navigator.userAgent;
}

export function maxTouchPoints() {
    return navigator.maxTouchPoints ?? 0;
}

// User-Agent Client Hints (Chromium-based browsers only), as JSON; null elsewhere.
export async function clientHints() {
    const data = navigator.userAgentData;
    if (!data) return null;

    let high = {};
    try {
        high = await data.getHighEntropyValues([
            "architecture", "bitness", "model", "platformVersion", "fullVersionList", "formFactors", "wow64",
        ]);
    } catch {
        // The browser may refuse high-entropy values; the low-entropy ones are still useful.
    }

    const list = (brands) => (brands ?? []).map((b) => ({ brand: b.brand, version: b.version }));

    return JSON.stringify({
        brands: list(data.brands),
        mobile: !!data.mobile,
        platform: data.platform ?? "",
        platformVersion: high.platformVersion ?? null,
        architecture: high.architecture ?? null,
        bitness: high.bitness ?? null,
        model: high.model ?? null,
        wow64: high.wow64 ?? null,
        fullVersionList: high.fullVersionList ? list(high.fullVersionList) : null,
        formFactors: high.formFactors ?? null,
    });
}

// The WebGL renderer string, as ua-parser-js reads it for the GPU; null without WebGL.
export function webglRenderer() {
    try {
        const canvas = document.createElement("canvas");
        const gl = canvas.getContext("webgl") || canvas.getContext("experimental-webgl");
        if (!gl) return null;
        const info = gl.getExtension("WEBGL_debug_renderer_info");
        const renderer = gl.getParameter(info ? info.UNMASKED_RENDERER_WEBGL : gl.RENDERER);
        gl.getExtension("WEBGL_lose_context")?.loseContext();
        return typeof renderer === "string" ? renderer : null;
    } catch {
        return null;
    }
}

export async function downloadStream(fileName, contentType, streamReference) {
    const buffer = await streamReference.arrayBuffer();
    const url = URL.createObjectURL(new Blob([buffer], { type: contentType }));
    const link = document.createElement("a");
    link.href = url;
    link.download = fileName;
    document.body.appendChild(link);
    link.click();
    link.remove();
    setTimeout(() => URL.revokeObjectURL(url), 2000);
}

export function scrollToId(id) {
    const element = document.getElementById(id);
    if (!element) return;
    const smooth = !matchMedia("(prefers-reduced-motion: reduce)").matches;
    element.scrollIntoView({ behavior: smooth ? "smooth" : "auto", block: "start" });
}
