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
  constructor(private http: HttpClient) {}

  getAll() {
    return this.http.get('/api/room');
  }

  getFrom(from: Date) {
    console.log(from);
    const fromStr = from.toISOString();
    console.log(fromStr);
    return this.http.get('/api/room?from=' + fromStr);
  }

  getLiveConnection(): HubConnection {
    const hubConnection = new HubConnectionBuilder()
      .configureLogging(LogLevel.Debug)
      .withUrl('/hub/room', {
        skipNegotiation: true,
        transport: HttpTransportType.WebSockets
      })
      .build();

    return hubConnection;
  }
}
