import { Component, OnInit } from '@angular/core';
import {
  HubConnectionBuilder,
  LogLevel,
  HttpTransportType
} from '@aspnet/signalr';
import { TemperatureSensor } from '../data/TemperatureSensor';
import { Chart } from 'chart.js';

@Component({
  selector: 'app-real-time',
  templateUrl: './real-time.component.html',
  styleUrls: ['./real-time.component.css']
})
export class RealTimeComponent implements OnInit {
  private readonly maxEntries = 30;
  private labels: string[] = [];
  private data_temp: number[] = [];
  private data_hum: number[] = [];
  chart: Chart;

  private hubConnection: signalR.HubConnection;

  constructor() {}

  ngOnInit(): void {
    this.createChart();

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
      this.addData(d, msg.temperature, msg.humidity);
    });
  }

  addData(label: string, temp: number, hum: number) {
    if (this.data_temp.length > this.maxEntries) {
      this.data_temp.shift();
      this.data_hum.shift();
      this.labels.shift();
    }

    this.data_temp.push(temp);
    this.data_hum.push(hum);
    this.labels.push(label);

    this.chart.update();
  }

  createChart() {
    this.chart = new Chart('real-time-canvas', {
      type: 'line',
      data: {
        labels: this.labels,
        datasets: [
          {
            data: this.data_temp,
            borderColor: '#3cba9f',
            yAxisID: 'A',
            label: 'Temperature',
            fill: false
          },
          {
            data: this.data_hum,
            yAxisID: 'B',
            label: 'Humidity',
            borderColor: '#eb5511',
            fill: false
          }
        ]
      },
      options: {
        legend: {
          display: true
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
              id: 'A',
              type: 'linear',
              position: 'left'
            },
            {
              id: 'B',
              type: 'linear',
              position: 'right',
              ticks: {
                max: 1,
                min: 0
              }
            }
          ]
        }
      }
    });
  }
}
