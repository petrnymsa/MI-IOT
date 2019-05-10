import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class StatusApiService {
  constructor(private http: HttpClient) { }

  getStatus() {
    return this.http.get('/api/status', { responseType: 'text' });
  }
}
