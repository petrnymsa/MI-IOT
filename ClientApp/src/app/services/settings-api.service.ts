import { Settings } from './../data/Settings';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class SettingsApiService {
  constructor(private http: HttpClient) {}

  get() {
    return this.http.get('/api/settings');
  }

  send(settings: Settings) {
    return this.http.post('api/settings', settings);
  }
}
