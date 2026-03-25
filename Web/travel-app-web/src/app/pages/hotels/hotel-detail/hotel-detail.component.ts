import { Component, signal } from '@angular/core';
import { Hotel, Room } from '../../../models/models';
import { ActivatedRoute } from '@angular/router';
import { HotelService } from '../../../services/hotel.service';
import { BookingService } from '../../../services/booking.service';
import { AuthService } from '../../../services/auth.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-hotel-detail',
  imports: [CommonModule, FormsModule],
  templateUrl: './hotel-detail.component.html',
  styleUrl: './hotel-detail.component.css'
})
export class HotelDetailComponent {

  hotel = signal<Hotel | null>(null);
  loading = signal(true);
  selectedRoom = signal<Room | null>(null);
  checkIn = '';
  checkOut = '';
  bookingLoading = signal(false);
  bookingError = signal('');
  confirmedBooking = signal<any>('');
  today = new Date().toISOString().split('T')[0];


  constructor(
    private route: ActivatedRoute,
    private hotelService: HotelService,
    private bookingService: BookingService,
    public auth: AuthService
  ) { }


  ngOnInit() {
    const id = +this.route.snapshot.paramMap.get('id')!;
    this.hotelService.getHotel(id).subscribe({
      next: h => {
        this.hotel.set(h);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
      }
    })
  }


  selectRoom(r: Room) {
    this.selectedRoom.set(this.selectedRoom()?.id === r.id ? null : r);
    this.confirmedBooking.set(false);
  }

  getStars(n: number) { 
    return Array(Math.round(n)).fill(0); 
  }
  getAmenities(a: string){
    return a.split(',').map(s => s.trim()); 
  }


  nightCount() {
    if (!this.checkIn || !this.checkOut) return 0;
    const diff = new Date(this.checkOut).getTime() - new Date(this.checkIn).getTime();
    return Math.max(0, Math.floor(diff / 86400000));
  }


  totalPrice() {
    return this.nightCount() * (this.selectedRoom()?.pricePerNight ?? 0);
  }


  confirmBooking() {
    const room = this.selectedRoom(); const h = this.hotel();
    if (!room || !h || !this.checkIn || !this.checkOut || this.nightCount() <= 0) return;
    this.bookingLoading.set(true); 
    this.bookingError.set('');

    this.bookingService.create({
      roomId: room.id, roomType: room.type, hotelName: h.name,
      checkInDate: this.checkIn, checkOutDate: this.checkOut,
      pricePerNight: room.pricePerNight
    }).subscribe({
      next: booking => {
        this.confirmedBooking.set(booking);
        this.bookingLoading.set(false);
      },
      error: err => {
        this.bookingError.set(err.error?.message ?? err.error ?? 'Booking failed. Please try again.');
        this.bookingLoading.set(false);
      }
    });
  }


}
