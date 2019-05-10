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

  constructor(private hubService: HubService, private statusApi: StatusApiService) { }

  ngOnInit() {

    this.statusApi.getStatus().subscribe((res: string) => {
      this.status = res.toLowerCase();
    });

    this.hubService.getConnection().on('statusUpdate', (msg: string) => {
      this.status = msg.toLowerCase();
    });
  }

}
