#include <Ethernet.h>

#define SERVER_IP "192.168.0.129"
#define SERVER_PORT 5000
#define SERVER_IP_PORT "192.168.0.129:5000"

const char *API_CONTENT_TYPE = "application/json";
const char *API_ENDPOINT = "/api/values";

const byte mac[] = {0xDE, 0xAD, 0xBE, 0xEF, 0xFE, 0xED};
const byte ip[] = {192, 168, 0, 222};

EthernetClient client;

void setup()
{
    Serial.begin(9600);
    // start the Ethernet connection:
    Serial.println(F("Initialize Ethernet"));

    Ethernet.begin(mac, ip);

    Serial.print(F("Assigned IP: "));
    Serial.println(Ethernet.localIP());
    // }

    delay(1000);
}

void loop()
{
    Serial.println(F("Sending.."));
    const float temp = 22.5f;
    const float humidity = 0.7f;

    String data = String(temp);
    data += ";";
    data += String(humidity);

    Serial.println(F(" - Post request in process - "));

    if (client.connect(SERVER_IP, SERVER_PORT))
    {
        Serial.print(F(" Sending Post request "));

        client.println("POST /api/values HTTP/1.1");
        client.print("Host: ");
        client.println(SERVER_IP_PORT);
        client.println("Connection: close");
        client.println("Content-Type: text/plain");
        client.print("Content-Length: ");
        client.println(data.length());
        client.println(""); //-------  I missed  ""

        client.print(data);
    }
    else
    {
        Serial.println(F("Can’t reach the server"));
    }
    client.stop();
    delay(5000);
}

void send_sensor(const float &temp, const float &humidity, const float &eses_hum = -1)
{
    String data = String(temp);
    data += ";";
    data += String(humidity);

    if (eses_hum >= 0)
        data += String(eses_hum);

    post_data(data);
}

void read_dht11()
{
}

float read_eses()
{
}

void post_data(const String &data)
{
    Serial.println(F(" - Post request in process - "));

    if (client.connect(SERVER_IP, SERVER_PORT))
    {
        Serial.print(F(" Sending Post request "));

        client.println("POST /api/values HTTP/1.1");
        client.print("Host: ");
        client.println(SERVER_IP_PORT);
        client.println("Connection: close");
        client.println("Content-Type: text/plain");
        client.print("Content-Length: ");
        client.println(data.length());
        client.println(""); //-------  I missed  ""

        client.print(data);
    }
    else
    {
        Serial.println(F("Can’t reach the server"));
    }
    client.stop();
}