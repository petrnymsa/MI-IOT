import { HttpClient } from '@angular/common/http';
import { Injectable, Inject } from '@angular/core';
import { httpFactory } from '@angular/http/src/http_module';
import {
  HubConnection,
  HubConnectionBuilder,
  LogLevel,
  HttpTransportType
} from '@aspnet/signalr';

@Injectable({
  providedIn: 'root'
})
export class FlowerApiService {
  // private baseUrl = 'http://10.0.0.149:5000';
  constructor(private http: HttpClient) { }

  getAll() {
    return this.http.get('/api/flower');
  }

  getLast(count: number) {
    return this.http.get('/api/flower?count=' + count);
  }

  //TODO merge it to standalone HubService
  getLiveConnection(): HubConnection {
    const hubConnection = new HubConnectionBuilder()
      .configureLogging(LogLevel.Error)
      .withUrl('/hub/room', {
        skipNegotiation: true,
        transport: HttpTransportType.WebSockets
      })
      .build();

    return hubConnection;
  }
}
