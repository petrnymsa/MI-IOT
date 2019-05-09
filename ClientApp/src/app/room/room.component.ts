import { DateFormatPipe } from './../util/DateFormatter';
import { TemperatureSensor } from './../data/TemperatureSensor';
import { RoomApiService } from './../services/room-api.service';
import { Component, OnInit } from '@angular/core';
import { Chart } from 'chart.js';
import { NgbDate, NgbDateStruct, NgbTimeStruct } from '@ng-bootstrap/ng-bootstrap';
import NgbTimeStructHelper from '../util/TimeStructureHelper';
import { NgbDateCustomParserFormatter } from './../util/NgDateCustomParseFormatter';

export interface PagedResult {
  paging: {
    currentItemsCount: number;
    currentPage: number;
    maxPages: number;
    totalItems: number;
  },
  items: TemperatureSensor[]
}

@Component({
  selector: 'app-room',
  templateUrl: './room.component.html',
  styleUrls: ['./room.component.css']
})
export class RoomComponent implements OnInit {
  private labels: string[] = [];
  private dataTemp: number[] = [];
  private dataHum: number[] = [];
  public chart: Chart;
  error: string;
  count = 25;
  lastRefresh: string;

  page = 0;

  dateStart: NgbDateStruct;
  dateEnd: NgbDateStruct;
  timeStart: NgbTimeStruct;
  timeEnd: NgbTimeStruct;


  constructor(
    private api: RoomApiService,
    private dateFormatPipe: DateFormatPipe
  ) { }

  ngOnInit() {
    console.log('loa;vgd');
    this.createChart();
    this.refreshLast();
  }

  refreshSpecificDate() {
    this.error = '';
    console.log(typeof (this.dateStart), ' ', this.dateEnd);
    if (!this.dateStart || !this.dateEnd) {
      this.error = 'Datum nevyplněno';
      return;
    }
    const ngbStartDate = NgbDate.from(this.dateStart);
    const ngbEndDate = NgbDate.from(this.dateEnd);
    if (ngbStartDate.after(ngbEndDate)) {
      this.error = 'Počáteční datum nesmí být větší';
      return;
    }

    if (NgbTimeStructHelper.isGreater(this.timeStart, this.timeEnd)) {
      this.error = 'Počáteční čas nesmí být větší';
      return;
    }


    // tslint:disable-next-line: max-line-length
    const startDate = new Date(ngbStartDate.year, ngbStartDate.month - 1, ngbStartDate.day, this.timeStart.hour, this.timeStart.minute, this.timeStart.second);
    // tslint:disable-next-line: max-line-length
    const endDate = new Date(ngbEndDate.year, ngbEndDate.month - 1, ngbEndDate.day, this.timeEnd.hour, this.timeEnd.minute, this.timeEnd.second);
    console.log('StartDate: ', startDate.toDateString(), 'EndDate: ', endDate.toDateString());

    this.api.getBetween(startDate, endDate).subscribe((res: TemperatureSensor[]) => {
      this.update(res);
    },
      err => {
        this.error = err.message;
      });
  }

  refreshLast() {
    this.api.getLast(this.count).subscribe(
      (res: TemperatureSensor[]) => {
        this.update(res);

        const first = new Date(res[0].date);
        const last = new Date(res[res.length - 1].date);
        console.log('first: ', first, ' , second: ', last);

        this.dateStart = NgbDate.from({
          year: first.getFullYear(),
          month: first.getMonth() + 1,
          day: first.getDate()
        });

        this.dateEnd = NgbDate.from({
          year: last.getFullYear(),
          month: last.getMonth() + 1,
          day: last.getDate()
        });

        this.timeStart = {
          hour: first.getHours(),
          minute: first.getMinutes(),
          second: first.getSeconds()
        };

        this.timeEnd = {
          hour: last.getHours(),
          minute: last.getMinutes(),
          second: last.getSeconds()
        };

        console.log('start: ', this.dateStart, ' , end: ', this.dateEnd);
      },
      e => (this.error = e.message)
    );
  }

  refreshPage() {
    console.log('page: ', this.page);
    this.api.getPaged(this.page, 20).subscribe((res: PagedResult) => {
      console.log(res);
      console.log('page: ', res.paging.currentPage, 'first item: ', res.items[0].temperature);

    });
  }

  update(res: TemperatureSensor[]) {
    this.updateData(res);
    this.chart.update();
    this.lastRefresh = new Date().toLocaleString();
  }

  updateData(res: TemperatureSensor[]) {
    this.labels.length = 0;
    this.dataTemp.length = 0;
    this.dataHum.length = 0;
    res.forEach((p: TemperatureSensor) => {
      const date = this.dateFormatPipe.transform(p.date);
      this.labels.push(date);
      this.dataTemp.push(p.temperature);
      this.dataHum.push(p.humidity);
    });
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
