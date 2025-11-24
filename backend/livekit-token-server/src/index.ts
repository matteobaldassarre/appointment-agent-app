import "dotenv/config";
import cors from "cors";
import express, { Express, Request, Response } from "express";
import { AccessToken } from "livekit-server-sdk";

let LIVEKIT_API_KEY: string;
let LIVEKIT_API_SECRET: string;

const initSecrets = async (): Promise<void> => {
    LIVEKIT_API_KEY = process.env.LIVEKIT_API_KEY as string;
    LIVEKIT_API_SECRET = process.env.LIVEKIT_API_SECRET as string;

    if (!LIVEKIT_API_KEY || !LIVEKIT_API_SECRET) {
        console.error(
            "LIVEKIT_API_KEY and LIVEKIT_API_SECRET must be provided as environment variables"
        );
        process.exit(1);
    }
};

initSecrets().then(() => {
    const createToken = async (): Promise<string> => {
        const roomName = "appointment-agent-room";
        const participantName = "customer";

        const accessToken = new AccessToken(
            LIVEKIT_API_KEY,
            LIVEKIT_API_SECRET,
            {
                identity: participantName,
                ttl: "30m",
            }
        );
        accessToken.addGrant({
            roomJoin: true,
            room: roomName,
        });

        return await accessToken.toJwt();
    };

    const app: Express = express();
    const port: number = 3000;

    app.use(cors({ origin: process.env.FRONTEND_ORIGIN_URL }));

    app.get(
        "/api/getToken",
        async (_: Request, response: Response): Promise<void> => {
            try {
                const token = await createToken();
                response.send(token);
            } 
            catch (error) {
                response.status(500).send("Error generating token.");
            }
        }
    );

    app.listen(port, () => {
        console.log(`Server listening on port ${port}`);
    });
});
