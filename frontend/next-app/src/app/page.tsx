'use client';

import { ControlBar, RoomAudioRenderer, RoomContext } from "@livekit/components-react";
import { Room } from "livekit-client";
import '@livekit/components-styles';
import React from "react";

async function getLiveKitToken(): Promise<string> {
    const tokenServerEndpoint = `${process.env.NEXT_PUBLIC_TOKEN_SERVER_BASE_URL}/api/getToken`;
    const response = await fetch(tokenServerEndpoint);

    return await response.text();
}

export default function Home() {
    const [room] = React.useState(() =>
        new Room({
            // Enable automatic audio/video quality optimization
            dynacast: true,
        })
    );

    // Connect to room
    React.useEffect(() => {
        let mounted = true;

        const connect = async () => {
            const serverUrl = process.env.NEXT_PUBLIC_LIVEKIT_URL;
            const token = await getLiveKitToken();

            if (mounted) {
                await room.connect(serverUrl as string, token);
            }
        };
        connect();

        return () => {
            mounted = false;
            room.disconnect();
        };
    }, [room]);

    return (
        <RoomContext.Provider value={room}>
            <div
                data-lk-theme="default"
                style={{
                    height: "100dvh",
                    display: "grid",
                    placeContent: "center",
                }}
            >
                {/* The RoomAudioRenderer takes care of room-wide audio for you. */}
                <RoomAudioRenderer />
                {/* Controls for the user to start/stop audio, video, and screen share tracks */}
                <ControlBar />
            </div>
        </RoomContext.Provider>
    );
}