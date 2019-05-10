import { HttpClient } from '@angular/common/http';
import { Injectable, Inject } from '@angular/core';
import {
  HubConnection,
  HubConnectionBuilder,
  LogLevel,
  HttpTransportType
} from '@aspnet/signalr';

@Injectable({
  providedIn: 'root'
})
export class RoomApiService {
  // private baseUrl = 'http://10.0.0.149:5000';
  constructor(private http: HttpClient) { }

  getAll() {
    return this.http.get('/api/room');
  }

  getFrom(from: Date) {
    const fromStr = from.toISOString();
    console.log('RoomApi-getFrom: ', fromStr);
    return this.http.get('/api/room?from=' + fromStr);
  }

  getUpTo(end: Date) {
    const endStr = end.toISOString();
    console.log('RoomApi-getUpto: ', endStr);
    return this.http.get('/api/room?end=' + endStr);
  }

  getBetween(from: Date, end: Date) {
    const fromStr = from.toISOString();
    const endStr = end.toISOString();
    console.log('RoomApi-getBetween: ', fromStr, ' to: ', endStr);
    return this.http.get('/api/room?from=' + fromStr + '&end=' + endStr);
  }

  getPaged(page: number, count = 20) {
    return this.http.get('/api/room/paged?page=' + page + '&count=' + count);
  }

  getLast(count: number) {
    return this.http.get('/api/room/last/' + count);
  }
  // TODO merge it to standalone HubService
  // getLiveConnection(): HubConnection {
  //   const hubConnection = new HubConnectionBuilder()
  //     .configureLogging(LogLevel.Error)
  //     .withUrl('/hub/room', {
  //       skipNegotiation: true,
  //       transport: HttpTransportType.WebSockets
  //     })
  //     .build();

  //   return hubConnection;
  // }
}
