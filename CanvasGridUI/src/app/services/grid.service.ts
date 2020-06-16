import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { catchError, retry } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class GridService {
  constructor(private http: HttpClient) { }
  apiUrl: string = 'api/Grid/';
  LoadGrids(): Observable<any>{
    return this.http.get(environment.serviceURL + this.apiUrl +  'LoadGrids');
  }
}
