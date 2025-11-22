export function formatDate(dateToFormat: string): string {
    const date = new Date(dateToFormat);

    const formatted = date.toLocaleString("en-US", {
        year: "numeric",
        month: "long",
        day: "numeric",
        hour: "2-digit",
        minute: "2-digit",
        hour12: true
    });

    return formatted.replace(",", "@");
}