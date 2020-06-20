import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { catchError, retry } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class GridService {
  constructor(private http: HttpClient) {
    this.headers = new HttpHeaders()
      .set('Content-Type', 'application/json');
  }
  apiUrl: string = 'api/Grid/';
  headers: HttpHeaders;

  LoadGrids(): Observable<any> {
    return this.http.get(environment.serviceURL + this.apiUrl + 'LoadGrids');
  }

  SaveGrid(formData: any) {
    return this.http.post(environment.serviceURL + this.apiUrl + 'SaveGrid', formData, {
      headers: new HttpHeaders()
    });
  }
}
