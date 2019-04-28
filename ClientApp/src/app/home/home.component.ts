import { TemperatureSensor } from './../data/TemperatureSensor';
import { Chart } from 'chart.js';
import { Component, OnInit } from '@angular/core';
import { RoomApiService } from '../services/room-api.service';
import { MatDatepickerInputEvent } from '@angular/material';
import {
  HubConnection,
  HubConnectionBuilder,
  LogLevel,
  HttpTransportType
} from '@aspnet/signalr';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html'
})
export class HomeComponent implements OnInit {
  private snapshots: TemperatureSensor[] = [];
  events: string[] = [];

  private date: string[] = [];
  private temp: number[] = [];
  chart: Chart;

  private hubConnection: signalR.HubConnection;

  constructor(private roomService: RoomApiService) {}

  ngOnInit(): void {
    this.roomService.getFrom(new Date(2019, 3, 27)).subscribe((res: any[]) => {
      this.snapshots = res;

      this.snapshots.forEach(y => {
        const d = new Date(y.date).toLocaleString();
        this.date.push(d);
        this.temp.push(y.temperature);
      });

      this.refreshChart();
    });

    this.hubConnection = new HubConnectionBuilder()
      .configureLogging(LogLevel.Debug)
      .withUrl('http://localhost:5000/hub/room', {
        skipNegotiation: true,
        transport: HttpTransportType.WebSockets
      })
      .build();

    this.hubConnection
      .start()
      .then(() => console.log('Connection started!'))
      .catch(err => console.log('Error while establishing connection: ' + err));

    this.hubConnection.on('roomUpdate', (msg: TemperatureSensor) => {
      const text = `${msg.humidity} ${msg.temperature} ${msg.date}`;
      this.events.push(text);
      this.temp.push(msg.temperature);
      this.date.push(msg.date.toLocaleString());

      this.refreshChart();
    });
  }

  addEvent(event: MatDatepickerInputEvent<Date>) {
    this.events.push(`${event.value}`);
  }

  refreshChart() {
    this.chart = new Chart('canvas', {
      type: 'line',
      data: {
        labels: this.date,
        datasets: [
          {
            data: this.temp,
            borderColor: '#3cba9f',
            fill: false
          }
        ]
      },
      options: {
        legend: {
          display: false
        },
        scales: {
          xAxes: [
            {
              display: true
            }
          ],
          yAxes: [
            {
              display: true
            }
          ]
        }
      }
    });
  }
}
