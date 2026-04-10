import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CreateHotelDto, CreateRoomDto, Hotel, Room } from '../../models/models';
import { HotelService } from '../../services/hotel.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-hotel-manager-dashboard',
  imports: [CommonModule, FormsModule],
  templateUrl: './hotel-manager-dashboard.component.html',
  styleUrl: './hotel-manager-dashboard.component.css'
})
export class HotelManagerDashboardComponent {
  hotels = signal<Hotel[]>([]);
  loading = signal(true);

  showHotelForm = signal(false);
  hotelSaving = signal(false);
  hotelError = signal('');

  selectedHotelId = signal<number | null>(null);
  roomSaving = signal(false);
  roomError = signal('');

  newHotel: CreateHotelDto = {
    name: '', city: '', address: '', description: '', starRating: 3, amenities: ''
  };

  newRoom: CreateRoomDto = {
    type: '', pricePerNight: 0, maxOccupancy: 2, description: ''
  };

  constructor(private hotelService: HotelService, public auth: AuthService) {}

  ngOnInit() {
    this.hotelService.getMyHotels().subscribe({
      next: hotels => { this.hotels.set(hotels); this.loading.set(false); },
      error: () => this.loading.set(false)
    });
  }

  toggleHotelForm() {
    this.showHotelForm.update(v => !v);
    this.hotelError.set('');
    this.newHotel = { name: '', city: '', address: '', description: '', starRating: 3, amenities: '' };
  }

  createHotel() {
    if (!this.newHotel.name.trim() || !this.newHotel.city.trim()) {
      this.hotelError.set('Name and city are required.');
      return;
    }
    this.hotelSaving.set(true);
    this.hotelError.set('');
    this.hotelService.createHotel(this.newHotel).subscribe({
      next: hotel => {
        this.hotels.update(list => [hotel, ...list]);
        this.showHotelForm.set(false);
        this.hotelSaving.set(false);
        this.newHotel = { name: '', city: '', address: '', description: '', starRating: 3, amenities: '' };
      },
      error: () => {
        this.hotelError.set('Failed to submit hotel. Please try again.');
        this.hotelSaving.set(false);
      }
    });
  }

  addRoom(hotelId: number) {
    if (!this.newRoom.type.trim() || !this.newRoom.pricePerNight) {
      this.roomError.set('Room type and price are required.');
      return;
    }
    this.roomSaving.set(true);
    this.roomError.set('');
    this.hotelService.addRoom(hotelId, this.newRoom).subscribe({
      next: room => {
        this.hotels.update(list => list.map(h =>
          h.id === hotelId ? { ...h, rooms: [...h.rooms, room] } : h
        ));
        this.selectedHotelId.set(null);
        this.roomSaving.set(false);
        this.newRoom = { type: '', pricePerNight: 0, maxOccupancy: 2, description: '' };
      },
      error: () => {
        this.roomError.set('Failed to submit room. Please try again.');
        this.roomSaving.set(false);
      }
    });
  }

  statusClass(status: string): string {
    if (status === 'Approved') return 'badge-approved';
    if (status === 'Rejected') return 'badge-rejected';
    return 'badge-pending';
  }
}
