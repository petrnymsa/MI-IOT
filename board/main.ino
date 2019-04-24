#include "DHT.h"
#include <Ethernet.h>
// ------ SERVER STUFF ---------------------
#define SERVER_IP "192.168.0.129"
#define SERVER_PORT 5000
#define SERVER_IP_PORT "192.168.0.129:5000"
// ---------- PINS -------------------------
#define DHTPIN 5
#define LED_DHT11 6
#define LED_ESES 7
#define LED_WEB 8
#define LED_OK 9
// -------- TIME INTERVALS -----------
#define INTERVAL_DHT11 3000 // each 3 seconds
#define INTERVAL_ESES 6000  // each 6 seconds
// -----------------------------------------

// Device ethernet configuration
const byte mac[] = {0xDE, 0xAD, 0xBE, 0xEF, 0xFE, 0xED};
const byte ip[] = {192, 168, 0, 222};
EthernetClient client;

// DHT11 Sensor
DHT dht(DHTPIN, DHT11);

unsigned long timer_dht11 = 0;
byte readFailedCount = 0;

bool status_sensor_dht11 = true;
bool status_sensor_eses = true;
bool status_web = true;

void setup()
{
    Serial.begin(9600);
    // start the Ethernet connection:
    Serial.println(F("Initialize Ethernet"));

    Ethernet.begin(mac, ip);

    Serial.print(F("Assigned IP: "));
    Serial.println(Ethernet.localIP());

    Serial.println("DHT TEST PROGRAM ");
    Serial.print("LIBRARY VERSION: ");
    Serial.println("Type,\tstatus,\tHumidity (%),\tTemperature (C)");

    dht.begin();

    pinMode(LED_OK, OUTPUT);
    pinMode(LED_WEB, OUTPUT);
    pinMode(LED_ESES, OUTPUT);
    pinMode(LED_DHT11, OUTPUT);
    delay(1000);
}

void loop()
{
    set_leds();
    if (millis() - timer_dht11 > INTERVAL_DHT11)
    {
        timer_dht11 = millis();
        Serial.println(F("-- Reading DHT11 --"));

        float temp = 0;
        float hum = 0;
        status_sensor_dht11 = read_dht11(&temp, &hum);

        if (status_sensor_dht11)
        {
            Serial.print(F("Teplota: "));
            Serial.print(temp);
            Serial.print(F("°C, Vlhkost: "));
            Serial.print(hum);
            Serial.println(F("%"));
        }
    }

    delay(3000);

    /* Serial.println(F("Sending.."));
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
     */
}

void set_leds()
{
    Serial.println(status_sensor_dht11);
    Serial.println(status_sensor_eses);
    Serial.println(status_web);
    if (status_sensor_dht11 && status_sensor_eses && status_web)
    {
        Serial.println(F("AL RIGHT"));
        digitalWrite(LED_OK, HIGH);
        digitalWrite(LED_DHT11, LOW);
        digitalWrite(LED_WEB, LOW);
        digitalWrite(LED_ESES, LOW);
    }
    else
    {
        digitalWrite(LED_OK, LOW);

        if (!status_sensor_dht11)
            digitalWrite(LED_DHT11, HIGH);
        else
            digitalWrite(LED_DHT11, LOW);

        if (!status_sensor_eses)
            digitalWrite(LED_ESES, HIGH);
        else
            digitalWrite(LED_ESES, LOW);

        if (!status_web)
            digitalWrite(LED_WEB, HIGH);
        else
            digitalWrite(LED_WEB, LOW);
    }
}

void send_sensor(const float &temp, const float &humidity, const float &eses_hum = -1)
{
    String data = String(temp);
    data += ";";
    data += String(humidity);

    if (eses_hum >= 0)
        data += String(eses_hum);

    http_post(data);
}
// ---------- SENSORS RELATED ----------------------------------
bool read_dht11(float *temp, float *hum)
{
    // Reading temperature or humidity takes about 250 milliseconds!
    // Sensor readings may also be up to 2 seconds 'old' (its a very slow sensor)
    float h = dht.readHumidity();
    float t = dht.readTemperature();

    if (isnan(h) || isnan(t))
    {
        Serial.println(F("Failed to read from DHT sensor!"));
        return false;
    }
    *temp = t;
    *hum = h;

    return true;
}

float read_eses()
{
}
// ---------- SERVER RELATED ----------------------------------
void http_post(const String &data)
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