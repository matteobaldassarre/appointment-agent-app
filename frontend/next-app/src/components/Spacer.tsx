export default function Spacer({ size = 16, horizontal = false }) {
    const style = {
        display: "block",
        width: horizontal ? size : "100%",
        height: horizontal ? "100%" : size,
    };

    return <span style={style} />;
}
