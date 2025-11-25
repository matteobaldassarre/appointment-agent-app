import { BarVisualizer, useVoiceAssistant } from "@livekit/components-react";

export default function AgentSpeechVisualizer() {
    const { state, audioTrack } = useVoiceAssistant();

    return (
        <BarVisualizer 
            state={state} 
            track={audioTrack} 
            style={{height: "20rem"}}
        />
    );
}