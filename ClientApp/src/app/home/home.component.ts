import { TemperatureSensor } from './../data/TemperatureSensor';
import { Chart } from 'chart.js';
import { Component, OnInit } from '@angular/core';
import { RoomApiService } from '../services/room-api.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html'
})
export class HomeComponent implements OnInit {
  private snapshots: TemperatureSensor[] = [];

  private date: string[] = [];
  private temp: number[] = [];
  chart: Chart;

  constructor(private roomService: RoomApiService) {}

  ngOnInit(): void {
    this.roomService.getAll().subscribe((res: any[]) => {
      this.snapshots = res;
      this.refreshChart();
    });
  }

  refreshChart() {
    this.snapshots.forEach(y => {
      var d = new Date(y.date).toDateString();
      this.date.push(d);
      this.temp.push(y.temperature);
    });

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
