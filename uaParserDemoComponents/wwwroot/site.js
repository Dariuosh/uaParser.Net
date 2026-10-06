// Loaded in <head>, before the page is shown, by both demos. Works without Blazor
// interactivity, so it also serves statically rendered pages:
// - the light/dark theme (follows the system until the visitor picks one),
// - buttons with data-copy="text" copy that text to the clipboard.
(() => {
    const key = "uaparser-demo-theme";
    const root = document.documentElement;

    const stored = () => {
        try {
            const value = localStorage.getItem(key);
            return value === "light" || value === "dark" ? value : null;
        } catch {
            return null;
        }
    };

    const apply = () => {
        const theme = stored();
        if (theme) {
            if (root.dataset.theme !== theme) root.dataset.theme = theme;
        } else if (root.dataset.theme) {
            delete root.dataset.theme;
        }
    };

    apply();

    // Blazor's enhanced navigation can replace the attributes of <html>: put the theme back.
    new MutationObserver(() => {
        if (root.dataset.theme !== (stored() ?? undefined)) apply();
    }).observe(root, { attributes: true, attributeFilter: ["data-theme"] });

    const isDark = () =>
        root.dataset.theme
            ? root.dataset.theme === "dark"
            : matchMedia("(prefers-color-scheme: dark)").matches;

    const copy = async (text) => {
        try {
            await navigator.clipboard.writeText(text);
            return true;
        } catch {
            // Fallback for pages that are not a secure context.
            const area = document.createElement("textarea");
            area.value = text;
            area.setAttribute("readonly", "");
            area.style.position = "fixed";
            area.style.opacity = "0";
            document.body.appendChild(area);
            area.select();
            const done = document.execCommand("copy");
            area.remove();
            return done;
        }
    };

    document.addEventListener("click", async (event) => {
        const target = event.target instanceof Element ? event.target : null;

        const toggle = target?.closest("[data-theme-toggle]");
        if (toggle) {
            const next = isDark() ? "light" : "dark";
            try {
                localStorage.setItem(key, next);
            } catch {
                // Private mode or storage disabled: the choice lasts until the page is reloaded.
            }
            root.dataset.theme = next;
            return;
        }

        const button = target?.closest("[data-copy]");
        if (button) {
            if (await copy(button.getAttribute("data-copy") ?? "")) {
                button.classList.add("copied");
                setTimeout(() => button.classList.remove("copied"), 1600);
            }
        }
    });
})();
