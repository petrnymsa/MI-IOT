#include "DHT.h"
#include <Ethernet.h>
// ------ SERVER STUFF ---------------------
#define SERVER_IP "192.168.0.129"
#define SERVER_PORT 5000
#define SERVER_IP_PORT "192.168.0.129:5000"
#define API_DHT11 "/api/values"
#define API_ESES "/api/flower"
// ---------- PINS -------------------------
#define DHTPIN 5
#define LED_DHT11 6
#define LED_ESES 7
#define LED_WEB 8
#define LED_OK 9

#define PIN_ESES_D 3
#define PIN_ESES_VCC 2
#define PIN_ESES_ANALOG A0
// -------- TIME INTERVALS -----------
#define INTERVAL_DHT11 5000 // each 3 seconds
#define INTERVAL_ESES 10000 // each 6 seconds
// -----------------------------------------
#define ESES_THRESHOLD 600
//-----------------------------------------
// Device ethernet configuration
const byte mac[] = {0xDE, 0xAD, 0xBE, 0xEF, 0xFE, 0xED};
const byte ip[] = {192, 168, 0, 222};
EthernetClient client;

// DHT11 Sensor
DHT dht(DHTPIN, DHT11);

unsigned long timer_dht11 = 0;
unsigned long timer_eses = 0;
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

    dht.begin();

    pinMode(LED_OK, OUTPUT);
    pinMode(LED_WEB, OUTPUT);
    pinMode(LED_ESES, OUTPUT);
    pinMode(LED_DHT11, OUTPUT);

    // setup eses
    pinMode(PIN_ESES_ANALOG, INPUT);
    pinMode(PIN_ESES_D, INPUT);
    pinMode(PIN_ESES_VCC, OUTPUT);
    digitalWrite(PIN_ESES_VCC, LOW);

    delay(1000);

    Serial.println(F("INIT COMPLETE"));
}

void loop()
{
    set_leds();

    float room_temp = 0;
    float room_hum = 0;
    int eses_analog = 0;

    bool eses_read = false;
    bool dht11_read = false;
    // pokud je rozdíl mezi aktuálním časem a posledním
    // uloženým větší než 3000 ms, proveď měření
    if (millis() - timer_eses > INTERVAL_ESES)
    {
        Serial.print(F("ESES: "));
        read_eses(&eses_analog);
        timer_eses = millis();

        if (eses_analog > ESES_THRESHOLD)
            status_sensor_eses = false;
        else
            status_sensor_eses = true;

        eses_read = true;
        Serial.println(eses_analog);
    }

    if (millis() - timer_dht11 > INTERVAL_DHT11)
    {
        timer_dht11 = millis();
        Serial.print(F("DHT11: "));

        status_sensor_dht11 = read_dht11(&room_temp, &room_hum);

        if (status_sensor_dht11)
        {
            Serial.print(room_temp);
            Serial.print(F("°C, Vlhkost: "));
            Serial.print(room_hum);
            Serial.println(F("%"));
        }
        dht11_read = status_sensor_dht11;
    }

    if (dht11_read)
    {
        Serial.println(F("Sending DHT11 data"));
        send_dht11_data(room_temp, room_hum);
    }

    if (eses_read)
    {
        Serial.println(F("Sending ESES data"));
        send_eses_data(eses_analog);
    }

    if (dht11_read || eses_read)
        Serial.println(F("------------------"));
}

void set_leds()
{
    if (status_sensor_dht11 && status_sensor_eses && status_web)
    {
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
// ------------------------------------------------------------
//                     SENSORS RELATED
// ------------------------------------------------------------
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

float read_eses(int *analog)
{
    digitalWrite(PIN_ESES_VCC, HIGH);
    delay(100);

    *analog = analogRead(PIN_ESES_ANALOG);

    digitalWrite(PIN_ESES_VCC, LOW);
}
// ------------------------------------------------------------
//                     SERVER RELATED
// ------------------------------------------------------------
void send_dht11_data(const float &temp, const float &humidity)
{
    String data = String(temp);
    data += ";";
    data += String(humidity);

    http_post(data, API_DHT11);
}

void send_eses_data(const float &value)
{
    String data = String(value);

    http_post(data, API_ESES);
}

void http_post(const String &data, const char *endpoint)
{
    Serial.println(F("HTTP: Post request in process..."));

    if (client.connect(SERVER_IP, SERVER_PORT))
    {
        status_web = true;

        client.print("POST ");
        client.print(endpoint);
        client.println(" HTTP/1.1");
        client.print("Host: ");
        client.println(SERVER_IP_PORT);
        client.println("Connection: close");
        client.println("Content-Type: text/plain");
        client.print("Content-Length: ");
        client.println(data.length());
        client.println(""); //-------  I missed  ""

        client.print(data);

        //TODO check returned status code
    }
    else
    {
        Serial.println(F("ERROR: Can’t reach the server"));
        status_web = false;
    }
    client.stop();
}