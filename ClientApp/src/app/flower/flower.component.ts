import { EsesSensor } from './../data/EsesSensor';
import { FlowerApiService } from './../services/flower-api.service';
import { Component, OnInit } from '@angular/core';
import { DateFormatPipe } from '../util/DateFormatter';
import { Chart } from 'chart.js';

@Component({
  selector: 'app-flower',
  templateUrl: './flower.component.html'
})
export class FlowerComponent implements OnInit {
  private labels: string[] = [];
  private dataHum: number[] = [];
  public chart: Chart;
  public error: string;
  count = 25;
  lastRefresh: string;

  constructor(
    private api: FlowerApiService,
    private dateFormatPipe: DateFormatPipe
  ) { }

  ngOnInit() {
    this.createChart();
    this.refresh();
  }

  refresh() {
    this.labels.length = 0;
    this.dataHum.length = 0;
    this.api.getLast(this.count).subscribe(
      (res: EsesSensor[]) => {
        res.forEach((p: EsesSensor) => {
          const date = this.dateFormatPipe.transform(p.date);
          this.labels.push(date);
          this.dataHum.push(p.humidity);
        });
        this.chart.update();
        this.lastRefresh = new Date().toLocaleString();
      },
      error => (this.error = error.message)
    );
  }

  createChart() {
    this.chart = new Chart('flowerCanvas', {
      type: 'line',
      data: {
        labels: this.labels,
        datasets: [
          {
            data: this.dataHum,
            borderColor: '#3cba9f',
            yAxisID: 'A',
            label: 'Vlhkost',
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
            }
          ]
        }
      }
    });
  }
}
