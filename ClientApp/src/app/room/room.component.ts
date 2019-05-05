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
  public error: string;

  constructor(private roomService: RoomApiService) {}

  ngOnInit() {
    this.roomService.getAll().subscribe(
      (res: TemperatureSensor[]) => {
        const part = res.reverse().slice(0, 30);
        part.forEach((p: TemperatureSensor) => {
          this.labels.push(new Date(p.date).toLocaleString());
          this.data.push(p.temperature);
        });

        this.createChart();
      },
      error => (this.error = error.message)
    ); // error path);
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
