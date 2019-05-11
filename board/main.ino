//#include "DHT.h"
#include <Ethernet.h>
//#include <arduinoduedht11.h>
#include <dht.h>
#include <ArduinoHttpClient.h>

// ------ SERVER STUFF ---------------------
#define SERVER_IP "10.0.0.149"
#define SERVER_PORT 5000
#define SERVER_IP_PORT "10.0.0.149:5000"
#define API_DHT11 "/api/room"
#define API_ESES "/api/flower"
// ---------- PINS -------------------------
#define DHTPIN 5
#define LED_DHT11 47
#define LED_ESES 49
#define LED_WEB 51
#define LED_OK 53

#define PIN_ESES_D 3
#define PIN_ESES_VCC 2
#define PIN_ESES_ANALOG A0
// -------- TIME INTERVALS -----------
#define INTERVAL_DEFAULT_DHT11 900000 // each 15 minutes
#define INTERVAL_DEFAULT_ESES 1800000 // each 30 minutes
#define INTERVAL_SETTINGS 30000       // each 30 seconds
#define INTERVAL_STATUS 30000         // each 5 seconds
// -----------------------------------------
#define ESES_THRESHOLD 600 // when over, start water
#define ESES_LOW 250       // minimum - stop water //TODO
//-----------------------------------------
// Device ethernet configuration
byte mac[] = {0xDE, 0xAD, 0xBE, 0xEF, 0xFE, 0xED};
IPAddress ip(10, 0, 0, 171);

EthernetClient ethernetClient;
HttpClient httpClient = HttpClient(ethernetClient, SERVER_IP, SERVER_PORT);

// DHT11 Sensor
dht DHT;

int interval_dht11 = 0;
int interval_eses = 0;

unsigned long timer_dht11 = 0;
unsigned long timer_eses = 0;
unsigned long timer_settings = 0;
unsigned long timer_status = 0;

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

    pinMode(DHTPIN, INPUT);
    //  dht.begin();

    pinMode(LED_OK, OUTPUT);
    pinMode(LED_WEB, OUTPUT);
    pinMode(LED_ESES, OUTPUT);
    pinMode(LED_DHT11, OUTPUT);

    // setup eses
    pinMode(PIN_ESES_ANALOG, INPUT);
    pinMode(PIN_ESES_D, INPUT);
    pinMode(PIN_ESES_VCC, OUTPUT);

    digitalWrite(PIN_ESES_VCC, LOW);

    digitalWrite(LED_OK, LOW);
    digitalWrite(LED_DHT11, LOW);
    digitalWrite(LED_WEB, LOW);
    digitalWrite(LED_ESES, LOW);

    interval_dht11 = INTERVAL_DEFAULT_DHT11;
    interval_eses = INTERVAL_DEFAULT_ESES;

    delay(1000);
    http_get_settings(&interval_dht11, &interval_eses);
    Serial.println(F("INIT COMPLETE"));
}

void loop()
{

    if (millis() - timer_status > INTERVAL_STATUS)
    {
        Serial.println(F("Sending heatbeat"));
        timer_status = millis();
        send_status();
    }

    if (millis() - timer_settings > INTERVAL_SETTINGS)
    {
        timer_settings = millis();
        http_get_settings(&interval_dht11, &interval_eses);
    }

    set_leds();

    float room_temp = 0;
    float room_hum = 0;
    int eses_analog = 0;

    bool eses_read = false;
    bool dht11_read = false;
    // pokud je rozdíl mezi aktuálním časem a posledním
    // uloženým větší než 3000 ms, proveď měření
    if (millis() - timer_eses > interval_eses)
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

    if (millis() - timer_dht11 > interval_dht11)
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

        // Serial.print(F("STATUS:"));
        // Serial.print(status_sensor_dht11);
        // Serial.print(" ");
        // Serial.print(status_sensor_eses);
        // Serial.print(" ");
        // Serial.print(status_web);
        // Serial.println(" ");
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
    // READ DATA
    int chk = DHT.read11(DHTPIN);

    if (chk != DHTLIB_OK)
    {
        Serial.println(F("Can't read DHT11 data"));
        return false;
    }

    *temp = DHT.temperature;
    *hum = DHT.humidity / 100.0;

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
    String contentType = "application/text";

    httpClient.post(endpoint, contentType, data);

    int statusCode = httpClient.responseStatusCode();
    String response = httpClient.responseBody();

    if (statusCode == 200)
    {
        status_web = true;
    }
    else
    {
        Serial.print(F("Can't post data."));
        status_web = false;
    }
}

void send_status()
{
    String contentType = "text/plain";
    httpClient.post("/api/status", contentType, "dummy");
    int statusCode = httpClient.responseStatusCode();
    String response = httpClient.responseBody();

    if (statusCode == 200)
    {
        Serial.println(F("Heartbeat ... OK"));
    }
    else
    {
        Serial.print(F("Heartbeat ... "));
        Serial.print(statusCode);
        Serial.print(F(", "));
        Serial.println(response);
    }
}

bool http_get_settings(int *dht11Interval, int *esesInterval)
{
    httpClient.get("/api/settings/plain");
    int statusCode = httpClient.responseStatusCode();
    String response = httpClient.responseBody();
    int id;

    if (statusCode == 200)
    {
        sscanf(response.c_str(), "%d;%d;%d", &id, dht11Interval, esesInterval);
        *dht11Interval = *dht11Interval * 1000;
        *esesInterval = *esesInterval * 1000;

        Serial.print(F("Settings: DHT11="));
        Serial.print(*dht11Interval);
        Serial.print(F(" ,ESES="));
        Serial.println(*esesInterval);
        return true;
    }
    else
    {
        Serial.print(F("Can't get settings. Will try again at"));
        Serial.print(INTERVAL_SETTINGS);
        Serial.println(F(" seconds"));
        return false;
    }
}