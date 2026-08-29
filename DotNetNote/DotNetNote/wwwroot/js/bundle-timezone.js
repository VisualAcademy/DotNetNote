export function getLocalOffsetMinutes() {
    return -new Date().getTimezoneOffset();
}

export function getBrowserCulture() {
    return navigator.language || "en-US";
}
