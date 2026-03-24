import { CommonModule } from '@angular/common';
import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import {  CreateTripDto, Trip } from '../../models/models';
import { TripService } from '../../services/trip.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-trips',
  imports: [FormsModule, CommonModule],
  templateUrl: './trips.component.html',
  styleUrl: './trips.component.css'
})
export class TripsComponent {

  trips = signal<Trip[]>([]);
  selectedTrip = signal<Trip | null>(null);
  loadingTrips = signal(true);

  showForm = signal(false);
  isCreating = signal(false);
  formError = signal('');

  isGenerating = signal(false);
  generateError = signal('');
  preferences = '';

  form:CreateTripDto = {
    destination : '',
    startDate: '',
    endDate: ''
  }


  constructor(private tripService: TripService, public auth: AuthService){}


  ngOnInit() { this.loadTrips(); }

  loadTrips() {
    this.loadingTrips.set(true);
    this.tripService.getMyTrips().subscribe({
      next: trips => { this.trips.set(trips); this.loadingTrips.set(false); },
      error: () => this.loadingTrips.set(false)
    });
  }


  toggleForm(){
    this.showForm.update(v => !v);
    this.formError.set('');
    this.form = { destination: '', startDate: '', endDate: '' };
  }

  createTrip(){
    this.formError.set('');
     if (!this.form.destination.trim()) { this.formError.set('Destination is required.'); return; }
    if (!this.form.startDate) { this.formError.set('Start date is required.'); return; }
    if (!this.form.endDate) { this.formError.set('End date is required.'); return; }
    if (new Date(this.form.endDate) <= new Date(this.form.startDate)) {
      this.formError.set('End date must be after start date.'); return;
    }


    this.isCreating.set(true);
    this.tripService.createTrip(this.form).subscribe({
      next: trip =>{
        this.trips.update(ts => [trip, ...ts]);
        this.selectedTrip.set(trip);
        this.showForm.set(false);
        this.isCreating.set(false);
        this.form = { destination: '', startDate: '', endDate: '' };
      },
      error: () => {
        this.formError.set('Failed to create trip. Please try again.');
        this.isCreating.set(false);
      }
    })

  }

  selectTrip(trip: Trip){
    this.selectedTrip.set(trip);
    this.generateError.set('');
  }


  generateItinerary(){
    const trip = this.selectedTrip();
    if(!trip){
      return;
    }

    this.isGenerating.set(true)
    this.generateError.set('')
    this.tripService.generateItinerary({
      tripId: trip.id,
      preferences: this.preferences || undefined
    }).subscribe({
      next: updated =>{
        this.selectedTrip.set(updated);
        this.trips.update(ts => ts.map(t => t.id === updated.id ? updated : t));
        this.isGenerating.set(false);
      },
      error: () => {
        this.generateError.set('AI generation failed. Check your Gemini API key and try again.');
        this.isGenerating.set(false);
      }
    })
  }

  deleteTrip(id: number, event: Event) {
    event.stopPropagation();
    if (!confirm('Delete this trip and its itinerary?')) return;
    this.tripService.deleteTrip(id).subscribe({
      next: () => {
        this.trips.update(ts => ts.filter(t => t.id !== id));
        if (this.selectedTrip()?.id === id) this.selectedTrip.set(null);
      }
    });
  }

  getDays(trip: Trip): number {
    const diff = new Date(trip.endDate).getTime() - new Date(trip.startDate).getTime();
    return Math.max(1, Math.round(diff / (1000 * 60 * 60 * 24)) + 1);
  }

  // Parse "Morning: x. Afternoon: y. Evening: z." into parts
  parseActivity(activity: string): { period: string; text: string }[] {
    const parts: { period: string; text: string }[] = [];
    const regex = /(Morning|Afternoon|Evening):\s*([^.]+(?:\.[^A-Z][^.]*)*\.?)/gi;
    let match;
    while ((match = regex.exec(activity)) !== null) {
      parts.push({ period: match[1], text: match[2].trim().replace(/\.$/, '') });
    }
    if (parts.length === 0) parts.push({ period: 'Activity', text: activity });
    return parts;
  }

}
