import { DateFormatPipe } from './../util/DateFormatter';
import { TemperatureSensor } from './../data/TemperatureSensor';
import { RoomApiService } from './../services/room-api.service';
import { Component, OnInit } from '@angular/core';
import { Chart } from 'chart.js';

@Component({
  selector: 'app-room',
  templateUrl: './room.component.html'
})
export class RoomComponent implements OnInit {
  private labels: string[] = [];
  private dataTemp: number[] = [];
  private dataHum: number[] = [];
  public chart: Chart;
  public error: string;
  count = 25;
  lastRefresh: string;

  constructor(
    private roomService: RoomApiService,
    private dateFormatPipe: DateFormatPipe
  ) {}

  ngOnInit() {
    this.createChart();
    this.refresh();
  }

  refresh() {
    this.labels.length = 0;
    this.dataTemp.length = 0;
    this.dataHum.length = 0;
    this.roomService.getLast(this.count).subscribe(
      (res: TemperatureSensor[]) => {
        //const part = res.reverse().slice(0, 30);
        res.forEach((p: TemperatureSensor) => {
          const date = this.dateFormatPipe.transform(p.date);
          this.labels.push(date);
          this.dataTemp.push(p.temperature);
          this.dataHum.push(p.humidity);
        });
        console.log(res.length);
        this.chart.update();
        this.lastRefresh = new Date().toLocaleString();
      },
      error => (this.error = error.message)
    ); // error path);
  }

  createChart() {
    this.chart = new Chart('roomCanvas', {
      type: 'line',
      data: {
        labels: this.labels,
        datasets: [
          {
            data: this.dataTemp,
            borderColor: '#3cba9f',
            yAxisID: 'A',
            label: 'Teplota',
            fill: false
          },
          {
            data: this.dataHum,
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
