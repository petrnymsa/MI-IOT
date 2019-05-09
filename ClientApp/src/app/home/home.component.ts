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
export class HomeComponent {


}
