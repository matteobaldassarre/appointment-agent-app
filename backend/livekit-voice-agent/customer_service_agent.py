from pathlib import Path
from livekit.agents import Agent, RunContext, ToolError, function_tool, utils
import os
import aiohttp
import asyncio

BASE = Path(__file__).parent
PROMPTS = BASE / "prompts"

DEFAULT_INSTRUCTIONS = (PROMPTS / "default_agent_instructions.txt").read_text(encoding="utf-8")
ON_ENTER_INSTRUCTIONS = (PROMPTS / "on_enter_instructions.txt").read_text(encoding="utf-8")

class CustomerServiceAgent(Agent):
    def __init__(self) -> None:
        super().__init__(instructions = DEFAULT_INSTRUCTIONS)

    async def on_enter(self):
        await self.session.generate_reply(
            instructions = ON_ENTER_INSTRUCTIONS,
            allow_interruptions = True
        )

    @function_tool(name = "book_appointment")
    async def _http_tool_book_appointment(
        self, 
        context: RunContext, 
        first_name: str, 
        last_name: str,
        date: str,
        phone: str
    ) -> str:
        """
        The purpose of this tool is to gather the required information, step-by-step, to book an appointment for the user. 
        After sending the API call the agent will let the user know how it went.
        If it was successful you can say that you successfully book an appointment for them.
        If an error occurs the agent should not disclose too much information and just provide a generic error response.

        Args:
            first_name: The first name of the customer.
            last_name: The last name of the customer.
            date: The date of the appointment for the customer.
            phone: The phone number of the customer.
        """

        context.disallow_interruptions()

        url = f"{os.getenv("APPOINTMENTS_API_BASE_URL")}/api/v1/appointments"
        payload = {
            "customer": {
                "firstName": first_name,
                "lastName": last_name,
                "phone": phone
            },
            "date": f"{date}Z",
            "status": "Scheduled"
        }

        try:
            session = utils.http_context.http_session()
            timeout = aiohttp.ClientTimeout(total=10)
            async with session.post(url, timeout = timeout, json = payload) as response:
                body = await response.text()
                if response.status >= 400:
                    raise ToolError(f"error: HTTP {response.status}: {body}")
                return body
        except ToolError:
            raise
        except (aiohttp.ClientError, asyncio.TimeoutError) as error:
            raise ToolError(f"error: {error!s}") from error
