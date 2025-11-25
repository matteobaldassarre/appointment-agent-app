from datetime import datetime
from zoneinfo import ZoneInfo
from pathlib import Path
from livekit.agents import Agent, RunContext, ToolError, function_tool, utils
import os
import aiohttp
import asyncio

BASE = Path(__file__).parent
PROMPTS = BASE / "prompts"

DEFAULT_INSTRUCTIONS = (PROMPTS / "default_agent_instructions.txt").read_text(encoding = "utf-8")
ON_ENTER_INSTRUCTIONS = (PROMPTS / "on_enter_instructions.txt").read_text(encoding = "utf-8")

class CustomerServiceAgent(Agent):
    def __init__(self) -> None:
        super().__init__(instructions = DEFAULT_INSTRUCTIONS)

    async def on_enter(self):
        await self.session.generate_reply(
            instructions = ON_ENTER_INSTRUCTIONS,
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
        The purpose of this tool is to book an appointment after gathering and confirming all required information step-by-step.
        Only call this tool once you have confirmed the details with the user to avoid errors from misheard speech.
        - Ask for first name, last name, phone, and date one at a time if needed.
        - Confirm the full details verbally (e.g., "So, booking for John Doe on 2025-12-01 at 10:00 AM with phone one two three four five six seven eight nine zero?").
        - Date should be in ISO format like '2025-12-01T10:00:00'.
        - If any info seems incorrect (e.g., past date or invalid phone), prompt the user to clarify before calling.
        
        Examples:
        - User says "Book for Alice Smith on December 1st 2025 at 10 AM, phone 3312092200": Parse to first_name='Alice', last_name='Smith', date='2025-12-01T10:00:00', phone='3312092200'.
        - Handle common voice errors like 'tomorrow' by resolving to actual date in the agent before calling.

        After the API call, inform the user based on the response outcome, make sure to complete the tool execution before saying that you successfully booked the appointment.

        Args:
            first_name: The first name of the customer (e.g., 'John'). Must not be empty.
            last_name: The last name of the customer (e.g., 'Doe'). Must not be empty.
            date: The appointment date/time in ISO-like string (e.g., '2025-12-01T10:00:00').
            phone: The customer's phone number (e.g., '1234567890') it should be exactly 10 digits long.
        """

        context.disallow_interruptions()

        # Input validation and sanitization for full name
        first_name = first_name.strip().capitalize()
        last_name = last_name.strip().capitalize()

        if not first_name or not last_name:
            raise ToolError("error: First or last name cannot be empty.")

        # Validate phone
        if len(phone) != 10:
            raise ToolError("error: Invalid phone number format. Please provide a valid number.")

        url = f"{os.getenv('NEXT_PUBLIC_APPOINTMENTS_API_BASE_URL')}/api/v1/appointments"
        utc_date = to_utc(date)

        payload = {
            "customer": {
                "firstName": first_name,
                "lastName": last_name,
                "phone": phone
            },
            "date": utc_date,
            "status": "Scheduled"
        }

        try:
            session = utils.http_context.http_session()
            timeout = aiohttp.ClientTimeout(total = 10)

            async with session.post(url, timeout = timeout, json = payload) as response:
                body = await response.text()
                if response.status >= 400:
                    raise ToolError(f"error: HTTP {response.status}: {body}")
                    
                return body
        except (aiohttp.ClientError, asyncio.TimeoutError) as error:
            raise ToolError("error: Network issue - please try again later.") from error

def to_utc(
    iso_datetime: str,
    user_timezone: str = "Europe/Rome"
) -> str:
    """
    Accepts a naive ISO-like string like '2025-12-01T11:00:00' (or '2025-12-01 11:00:00'),
    treats it as user_timezone local time, converts to UTC and returns 'YYYY-MM-DDTHH:MM:SSZ'.
    """
    date = datetime.fromisoformat(iso_datetime)
    if date.tzinfo is None:
        date = date.replace(tzinfo = ZoneInfo(user_timezone))

    datetime_utc = date.astimezone(ZoneInfo("UTC"))
    return datetime_utc.strftime("%Y-%m-%dT%H:%M:%SZ")