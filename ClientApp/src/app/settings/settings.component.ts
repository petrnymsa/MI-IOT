import { Settings } from './../data/Settings';
import { SettingsApiService } from './../services/settings-api.service';
import { Component, OnInit, Input } from '@angular/core';
import { isUndefined } from 'util';

@Component({
  selector: 'app-settings',
  templateUrl: './settings.component.html'
})
export class SettingsComponent implements OnInit {
  settings: Settings;
  showMsg = false;
  constructor(private api: SettingsApiService) {
    this.settings = new Settings();
  }

  ngOnInit() {
    this.api.get().subscribe(
      (res: Settings) => {
        this.settings = res;
      },
      error => {
        console.log(error);
      }
    );
  }

  handleSubmit() {
    if (
      isUndefined(this.settings.dhT11Interval) &&
      isUndefined(this.settings.esesInterval)
    ) {
      alert('Nevalidni vsutp');
      return;
    }

    this.showMsg = false;

    this.api.send(this.settings).subscribe(
      () => {},
      error => {
        console.log(error);
        this.showMsg = false;
      },
      () => {
        //on complete
        console.log('send complete');
        this.showMsg = true;
      }
    );
  }
}
