import os
import time

import pika
import requests
from pydantic import ValidationError

from conversion_service import ensure_bucket_online, execute_conversion
from models import ConversionRequestedMessage


QUEUE_NAME = "conversion_jobs"

BROKER_HOST = os.getenv("RABBITMQ_HOST", "broker")
BROKER_PORT = int(os.getenv("RABBITMQ_PORT", "5672"))
BROKER_USER = os.getenv("RABBITMQ_USER", "fileconverter")
BROKER_PASSWORD = os.getenv(
    "RABBITMQ_PASSWORD",
    "DevPassword123!",
)

API_BASE_URL = os.getenv(
    "API_BASE_URL",
    "http://converter-api:8080",
).rstrip("/")

INTERNAL_API_KEY = os.environ["INTERNAL_API_KEY"]
INTERNAL_HEADERS = {
    "X-Internal-Key": INTERNAL_API_KEY,
}


def internal_post(path: str, payload=None) -> requests.Response:
    response = requests.post(
        f"{API_BASE_URL}{path}",
        json=payload,
        headers=INTERNAL_HEADERS,
        timeout=(5, 30),
    )
    response.raise_for_status()
    return response


def mark_started(conversion_id: str) -> bool:
    response = internal_post(
        f"/internal/conversions/{conversion_id}/started"
    )
    return bool(response.json()["shouldProcess"])


def mark_completed(
    conversion_id: str,
    result_payload: dict,
) -> None:
    internal_post(
        f"/internal/conversions/{conversion_id}/completed",
        result_payload,
    )


def mark_failed(conversion_id: str, error: Exception) -> None:
    message = str(error).strip() or type(error).__name__
    internal_post(
        f"/internal/conversions/{conversion_id}/failed",
        {"error": message[:255]},
    )


def requeue(channel, delivery_tag: int) -> None:
    time.sleep(3)
    channel.basic_nack(
        delivery_tag=delivery_tag,
        requeue=True,
    )


def callback(channel, method, properties, body: bytes) -> None:
    try:
        message = ConversionRequestedMessage.model_validate_json(body)
    except ValidationError as error:
        print(f"Discarding malformed conversion message: {error}", flush=True)
        channel.basic_ack(delivery_tag=method.delivery_tag)
        return

    conversion_id = str(message.conversion_id)

    try:
        should_process = mark_started(conversion_id)
    except requests.RequestException as error:
        print(f"Could not mark {conversion_id} as started: {error}", flush=True)
        requeue(channel, method.delivery_tag)
        return

    if not should_process:
        channel.basic_ack(delivery_tag=method.delivery_tag)
        return

    try:
        result = execute_conversion(message)
    except Exception as error:
        try:
            mark_failed(conversion_id, error)
        except requests.RequestException as status_error:
            print(
                f"Could not record failure for {conversion_id}: {status_error}",
                flush=True,
            )
            requeue(channel, method.delivery_tag)
            return

        print(f"Conversion {conversion_id} failed: {error}", flush=True)
        channel.basic_ack(delivery_tag=method.delivery_tag)
        return

    try:
        mark_completed(
            conversion_id,
            result.model_dump(by_alias=True),
        )
    except requests.RequestException as error:
        print(
            f"Could not record completion for {conversion_id}: {error}",
            flush=True,
        )
        requeue(channel, method.delivery_tag)
        return

    print(f"Conversion {conversion_id} completed", flush=True)
    channel.basic_ack(delivery_tag=method.delivery_tag)


def consume() -> None:
    credentials = pika.PlainCredentials(
        BROKER_USER,
        BROKER_PASSWORD,
    )
    parameters = pika.ConnectionParameters(
        host=BROKER_HOST,
        port=BROKER_PORT,
        credentials=credentials,
        heartbeat=600,
        blocked_connection_timeout=300,
    )

    connection = pika.BlockingConnection(parameters)
    channel = connection.channel()
    channel.queue_declare(
        queue=QUEUE_NAME,
        durable=True,
    )
    channel.basic_qos(prefetch_count=1)
    channel.basic_consume(
        queue=QUEUE_NAME,
        on_message_callback=callback,
        auto_ack=False,
    )

    print("Waiting for conversion jobs...", flush=True)
    channel.start_consuming()


def main() -> None:
    ensure_bucket_online()

    while True:
        try:
            consume()
        except KeyboardInterrupt:
            return
        except pika.exceptions.AMQPError as error:
            print(f"RabbitMQ connection failed: {error}", flush=True)
            time.sleep(5)


if __name__ == "__main__":
    main()
