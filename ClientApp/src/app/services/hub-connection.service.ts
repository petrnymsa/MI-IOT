import { Injectable } from '@angular/core';
import {
    HubConnection,
    HubConnectionBuilder,
    LogLevel,
    HttpTransportType
} from '@aspnet/signalr';

@Injectable({
    providedIn: 'root'
})
export class HubService {
    private hubConnection: HubConnection;

    constructor() { }

    getConnection(): HubConnection {
        if (!this.hubConnection) {
            this.hubConnection = new HubConnectionBuilder()
                .configureLogging(LogLevel.Error)
                .withUrl('/hub/room', {
                    skipNegotiation: true,
                    transport: HttpTransportType.WebSockets
                })
                .build();

            this.hubConnection
                .start()
                .then(() => console.log('SignalR hub connection started!'))
                .catch(err => console.log('Error while establishing connection: ' + err));
        }

        return this.hubConnection;
    }
}
