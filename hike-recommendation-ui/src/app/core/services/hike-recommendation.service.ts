import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { HikeRecommendation } from '../../models/hike-recommendation.model';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class HikeRecommendationService {
  private readonly baseUrl = 'http://localhost:5117/api/HikeRecommendation';

  constructor(private http: HttpClient) {}

  getRecommendation(employeeId: string): Observable<HikeRecommendation> {
    return this.http.get<HikeRecommendation>(`${this.baseUrl}/${employeeId}`);
  }
}
