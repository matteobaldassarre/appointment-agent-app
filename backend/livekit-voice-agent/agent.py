import logging
from pathlib import Path
from dotenv import load_dotenv
from livekit import rtc
from livekit.agents import AgentServer, JobContext, AgentSession, inference, room_io, cli
from livekit.plugins import noise_cancellation, silero

from customer_service_agent import CustomerServiceAgent

BASE = Path(__file__).parent
logger = logging.getLogger("agent-AppointmentAgent")
load_dotenv(".env")

server = AgentServer()

def prewarm(proc):
    proc.userdata["vad"] = silero.VAD.load()

server.setup_fnc = prewarm

@server.rtc_session()
async def entrypoint(context: JobContext):
    session = AgentSession(
        stt = inference.STT(model = "assemblyai/universal-streaming", language = "en"),
        llm = inference.LLM(model = "openai/gpt-4.1-mini"),
        tts = inference.TTS(
            model = "cartesia/sonic-3",
            voice = "9626c31c-bec5-4cca-baa8-f8ba9e84c8bc",
            language = "en-US"
        ),
        turn_detection = "vad",
        vad = context.proc.userdata["vad"],
        preemptive_generation = True
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
    cli.run_app(server)
