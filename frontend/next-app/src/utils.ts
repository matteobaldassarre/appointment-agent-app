export function formatDate(dateToFormat: string): string {
    const date = new Date(dateToFormat);

    const formatted = date.toLocaleString(
        "en-US", 
        {
            year: "numeric",
            month: "long",
            day: "numeric",
            hour: "2-digit",
            minute: "2-digit",
            hour12: true
        }
    );

    return formatted;
}

export function formatPhoneNumber(phone: string): string {
    const digits = phone.replace(/\D/g, ""); // Remove any non-digit characters

    const part1 = digits.slice(0, 3);
    const part2 = digits.slice(3, 6);
    const part3 = digits.slice(6);

    return `${part1} ${part2} ${part3}`;
}