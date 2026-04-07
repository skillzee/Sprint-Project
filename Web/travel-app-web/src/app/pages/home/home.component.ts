import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-home',
  imports: [RouterLink],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {
  constructor(public auth: AuthService) {}
  features = [
    { icon: '✈️', title: 'Smart Flight Search', desc: 'Search and compare flights across routes with real-time seat availability.' },
    { icon: '🏨', title: 'Curated Hotels', desc: 'Hand-picked accommodations from budget-friendly to luxury, all with honest reviews.' },
    { icon: '🗓️', title: 'Easy Booking', desc: 'Book in seconds. All your reservations stored safely in one place.' },
    { icon: '🔔', title: 'Instant Notifications', desc: 'Booking confirmations and updates delivered to you automatically.' },
    { icon: '🔒', title: 'Secure Payments', desc: 'Bank-grade security for every transaction. Pay your way.' },
    { icon: '📱', title: 'Manage On The Go', desc: 'Cancel or modify bookings anytime, from anywhere.' },
    { icon: '🤖', title: 'AI Trip Planner', desc: 'Let Gemini AI craft your perfect day-by-day itinerary — just pick a destination.' },
  ];
}
