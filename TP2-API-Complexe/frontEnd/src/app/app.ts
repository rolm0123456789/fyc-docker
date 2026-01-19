import { Component, signal, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, CommonModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit {
  protected readonly title = signal('frontEnd');

  // Convert to signals for better reactivity
  protected readonly statusData = signal<any>(null);
  protected readonly isLoading = signal(true);
  protected readonly errorMsg = signal('');

  constructor(private http: HttpClient) {
  }

  ngOnInit() {
    console.log('App Initialized. Fetching status...');
    this.fetchStatus();
  }

  fetchStatus() {
    this.http.get<any>('/api/diagnostic').subscribe({
      next: (data) => {
        console.log('API Success:', data);
        // Update signals
        this.statusData.set(data);
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error('API Error:', err);
        this.errorMsg.set('Impossible de contacter le backend.');
        this.isLoading.set(false);
      }
    });
  }
}
