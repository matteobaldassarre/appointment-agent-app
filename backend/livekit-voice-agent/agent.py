from dotenv import load_dotenv

from livekit import rtc, agents
from livekit.agents import AgentServer, AgentSession, room_io, JobContext
from livekit.plugins import openai, noise_cancellation

from customer_service_agent import CustomerServiceAgent

load_dotenv(".env")

server = AgentServer()

@server.rtc_session()
async def entrypoint(context: JobContext):
    session = AgentSession(
        llm = openai.realtime.RealtimeModel(
            voice = "marin"
        )
    )

    await session.start(
        agent = CustomerServiceAgent(),
        room = context.room,
        room_options = room_io.RoomOptions(
            audio_input = room_io.AudioInputOptions(
                noise_cancellation = lambda params: noise_cancellation.BVCTelephony() 
                    if params.participant.kind == rtc.ParticipantKind.PARTICIPANT_KIND_SIP 
                    else noise_cancellation.BVC()
            )
        )
    )

if __name__ == "__main__":
    agents.cli.run_app(server)
