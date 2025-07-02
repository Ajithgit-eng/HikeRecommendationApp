import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClientModule, HttpClient } from '@angular/common/http';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, HttpClientModule, MatCardModule, MatProgressSpinnerModule, FormsModule],
  templateUrl: './hike-result.component.html',
  styleUrls: ['./hike-result.component.scss']
})
export class HikeResultComponent implements OnInit {
  data: any = null;
  loading = false;
  employeeId: string = '';
  error: string = '';

  constructor(private http: HttpClient) {}

  ngOnInit(): void {}

  fetchRecommendation() {
    if (!this.employeeId) {
      this.error = 'Please enter an Employee ID.';
      return;
    }
    this.error = '';
    this.loading = true;
    this.data = null;
    this.http
      .get(`http://localhost:5117/api/HikeRecommendation/${this.employeeId}`)
      .subscribe({
        next: (res) => {
          this.data = res;
          this.loading = false;
        },
        error: (err) => {
          this.error = 'Failed to fetch data';
          this.loading = false;
        }
      });
  }
}
