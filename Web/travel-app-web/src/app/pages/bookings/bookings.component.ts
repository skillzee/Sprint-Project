import { Component, signal } from '@angular/core';
import { Booking } from '../../models/models';
import { BookingService } from '../../services/booking.service';
import { AuthService } from '../../services/auth.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-bookings',
  imports: [CommonModule],
  templateUrl: './bookings.component.html',
  styleUrl: './bookings.component.css'
})
export class BookingsComponent {

  bookings = signal<Booking[]>([]);
  loading = signal(true);
  cancelling = signal<number | null>(null)

  constructor(private bookingService: BookingService, public auth: AuthService){}


  ngOnInit(){
    this.bookingService.getMyBooking().subscribe({
      next: b=>{
        this.bookings.set(b);
        this.loading.set(false);
      },
      error: ()=>{
        this.loading.set(false);
      }
    })
  }


  cancel(id: number){
    this.cancelling.set(id);
    this.bookingService.cancel(id).subscribe({
      next: ()=>{
        this.bookings.update(bs=>bs.map(b => b.id === id ? { ...b, status: 'Cancelled' } : b));
        this.cancelling.set(null);
      },
      error: ()=>{
        this.cancelling.set(null);
      }
    })
  }


}
