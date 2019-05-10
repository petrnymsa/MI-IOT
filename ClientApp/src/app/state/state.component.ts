import { StatusApiService } from './../services/status-api.service';
import { HubService } from './../services/hub-connection.service';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-state',
  templateUrl: './state.component.html',
  styleUrls: ['./state.component.css']
})
export class StateComponent implements OnInit {

  status;
  lastUpdate;

  constructor(private hubService: HubService, private statusApi: StatusApiService) { }

  ngOnInit() {

    this.statusApi.getStatus().subscribe((res: string) => {
      this.refresh(res);
    });

    this.hubService.getConnection().on('statusUpdate', (msg: string) => {
      this.refresh(msg);
    });
  }

  refresh(res: string) {
    this.status = res.toLowerCase();
    this.lastUpdate = new Date().toLocaleTimeString();
  }

}
