import { HubService } from './../services/hub-connection.service';
import { FlowerApiService } from './../services/flower-api.service';
import { EsesSensor } from './../data/EsesSensor';
import { Component, OnInit } from '@angular/core';
import { Chart } from 'chart.js';
import { DateFormatPipe } from '../util/DateFormatter';

@Component({
  selector: 'app-flower-real-time',
  templateUrl: './flower-real-time.component.html'
})
export class FlowerRealTimeComponent implements OnInit {
  private readonly maxEntries = 30;
  private labels: string[] = [];
  private data_hum: number[] = [];
  chart: Chart;
  actual_temp: number;
  actual_hum: number;

  status: string;
  isOk = true;

  // private hubConnection: signalR.HubConnection;

  constructor(
    private api: FlowerApiService,
    private hubService: HubService,
    private dateFormatPipe: DateFormatPipe
  ) { }

  ngOnInit(): void {
    this.createChart();

    this.api.getLast(this.maxEntries).subscribe((res: EsesSensor[]) => {
      res.forEach((p: EsesSensor) => {
        const date = this.dateFormatPipe.transform(p.date);
        this.labels.push(date);
        this.data_hum.push(p.humidity);
        this.actual_hum = p.humidity;
      });
      this.chart.update();
      this.refreshStatus();
    });

    const hubConnection = this.hubService.getConnection();

    hubConnection.on('flowerUpdate', (msg: EsesSensor) => {
      const date = this.dateFormatPipe.transform(msg.date);
      this.addData(date, msg.humidity);
      this.refreshStatus();
    });
  }

  addData(label: string, hum: number) {
    if (this.data_hum.length > this.maxEntries) {
      this.data_hum.shift();
      this.labels.shift();
    }
    this.data_hum.push(hum);
    this.labels.push(label);

    this.actual_hum = hum;

    this.chart.update();
  }

  refreshStatus() {
    if (this.actual_hum >= 600) {
      this.status = 'Potřebuji zalít';
      this.isOk = false;
    } else if (this.actual_hum >= 300) {
      this.status = 'Jsem v pohodě';
      this.isOk = true;
    } else {
      this.status = 'Uber vodu nebo se utopím';
      this.isOk = false;
    }
  }

  createChart() {
    this.chart = new Chart('real-time-flower-canvas', {
      type: 'line',
      data: {
        labels: this.labels,
        datasets: [
          {
            data: this.data_hum,
            borderColor: '#3cba9f',
            yAxisID: 'A',
            label: 'Vlhkost',
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
