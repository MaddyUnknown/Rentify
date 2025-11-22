import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Utility, CreateUtility, UpdateUtility } from '../models/utility.model';

// @Injectable({
//   providedIn: 'root'
// })
export class UtilityService {
  private apiUrl = 'https://localhost:7000/api/utilities';

  constructor(private http: HttpClient) {}

  getAllUtilities(): Observable<Utility[]> {
    return this.http.get<Utility[]>(this.apiUrl);
  }

  getUtilityById(id: number): Observable<Utility> {
    return this.http.get<Utility>(`${this.apiUrl}/${id}`);
  }

  createUtility(utility: CreateUtility): Observable<Utility> {
    return this.http.post<Utility>(this.apiUrl, utility);
  }

  updateUtility(id: number, utility: UpdateUtility): Observable<Utility> {
    return this.http.put<Utility>(`${this.apiUrl}/${id}`, utility);
  }

  deleteUtility(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
