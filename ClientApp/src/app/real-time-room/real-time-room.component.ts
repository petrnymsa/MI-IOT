import { HubService } from './../services/hub-connection.service';
import { RoomApiService } from '../services/room-api.service';
import { Component, OnInit } from '@angular/core';
import { TemperatureSensor } from '../data/TemperatureSensor';
import { Chart } from 'chart.js';
import { DateFormatPipe } from '../util/DateFormatter';

@Component({
  selector: 'app-real-time',
  templateUrl: './real-time-room.component.html'
})
export class RealTimeRoomComponent implements OnInit {
  private readonly maxEntries = 30;
  private labels: string[] = [];
  private data_temp: number[] = [];
  private data_hum: number[] = [];
  chart: Chart;
  actual_temp: number;
  actual_hum: number;

  constructor(
    private roomApi: RoomApiService,
    private hubService: HubService,
    private dateFormatPipe: DateFormatPipe
  ) { }

  ngOnInit(): void {
    this.createChart();

    this.roomApi
      .getLast(this.maxEntries)
      .subscribe((res: TemperatureSensor[]) => {
        // const part = res.reverse().slice(0, 30);
        res.forEach((p: TemperatureSensor) => {
          const date = this.dateFormatPipe.transform(p.date);
          this.labels.push(date);
          this.data_temp.push(p.temperature);
          this.data_hum.push(p.humidity);
        });
        this.chart.update();
      });

    const hubConnection = this.hubService.getConnection();

    hubConnection.on('roomUpdate', (msg: TemperatureSensor) => {
      const date = this.dateFormatPipe.transform(msg.date);
      this.addData(date, msg.temperature, msg.humidity);
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

    this.actual_temp = temp;
    this.actual_hum = hum * 100;

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
            label: 'Teplota',
            fill: false
          },
          {
            data: this.data_hum,
            yAxisID: 'B',
            label: 'Vlhkost',
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
