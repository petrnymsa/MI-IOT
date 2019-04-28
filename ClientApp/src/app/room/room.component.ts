import { TemperatureSensor } from './../data/TemperatureSensor';
import { RoomApiService } from './../services/room-api.service';
import { Component, OnInit } from '@angular/core';
import { Chart } from 'chart.js';

@Component({
  selector: 'app-room',
  templateUrl: './room.component.html',
  styleUrls: ['./room.component.css']
})
export class RoomComponent implements OnInit {
  private labels: string[] = [];
  private data: number[] = [];
  public chart: Chart;

  constructor(private roomService: RoomApiService) {}

  ngOnInit() {
    this.roomService.getAll().subscribe((res: TemperatureSensor[]) => {
      console.log('length: ' + res.length);
      res.forEach(y => {
        const d = new Date(y.date).toLocaleString();
        if (this.labels.length < 20) {
          this.labels.push(d);
          this.data.push(y.temperature);
        }
      });
      console.log('length: ' + this.labels.length);
      this.createChart();
    });
  }

  createChart() {
    this.chart = new Chart('canvas', {
      type: 'line',
      data: {
        labels: this.labels,
        datasets: [
          {
            data: this.data,
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
