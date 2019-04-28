import { HttpClient } from '@angular/common/http';
import { Injectable, Inject } from '@angular/core';
import { httpFactory } from '@angular/http/src/http_module';

@Injectable({
  providedIn: 'root'
})
export class RoomApiService {
  private baseUrl = 'http://localhost:5000';
  constructor(private http: HttpClient) {}

  getAll() {
    return this.http.get(this.baseUrl + '/api/room');
  }
}
