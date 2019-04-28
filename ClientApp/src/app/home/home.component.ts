import { TemperatureSensor } from './../data/TemperatureSensor';
import { Chart, ChartData } from 'chart.js';
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

      this.createChart();
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
      const d = new Date(msg.date).toLocaleString();
      this.addData(d, msg.temperature);
    });
  }

  addData(label, temp) {
    if (this.temp.length > 36) {
      this.temp.shift();
      this.date.shift();
    }

    this.temp.push(temp);
    this.date.push(label);

    this.chart.update();
  }

  createChart() {
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
        animation: {
          duration: 1000
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
