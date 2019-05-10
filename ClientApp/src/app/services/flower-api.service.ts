import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class FlowerApiService {
  constructor(private http: HttpClient) { }

  getAll() {
    return this.http.get('/api/flower');
  }

  getLast(count: number) {
    return this.http.get('/api/flower?count=' + count);
  }
}
