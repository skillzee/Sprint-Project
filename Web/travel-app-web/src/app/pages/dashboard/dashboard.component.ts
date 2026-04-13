import { Component, signal } from '@angular/core';
import { CreateHotelDto, CreateRoomDto, Hotel } from '../../models/models';
import { BookingService } from '../../services/booking.service';
import { HotelService } from '../../services/hotel.service';
import { AuthService } from '../../services/auth.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-dashboard',
  imports: [FormsModule, CommonModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent {


  activeTab = signal<'bookings'|'hotels'|'approvals'>('bookings');

  bookings = signal<any[]>([]);
  totalRevenue = signal(0);
  loading = signal(true);

  hotels = signal<Hotel[]>([]);
  hotelSaving = signal(false);
  roomSaving = signal(false);
  selectedHotelId = signal<number | null>(null);

  pendingHotels = signal<Hotel[]>([]);
  pendingRooms = signal<any[]>([]);
  approvalsLoading = signal(false);


  newHotel = signal<CreateHotelDto>({
    name: '', city: '', address: '', description: '', starRating: 3, amenities: ''
  });

  newRoom = signal<CreateRoomDto>({
    type: '', pricePerNight: 0, maxOccupancy: 2, description: ''
  });

  constructor(private bookingService: BookingService, private hotelService: HotelService, public auth: AuthService){}


  ngOnInit(){
    this.bookingService.getAllBookings().subscribe({
      next: res=>{
        this.bookings.set(res.bookings);
        this.totalRevenue.set(res.totalRevenue);
        this.loading.set(false);
      },
      error: ()=>{
        this.loading.set(false);
      }
    })

    this.loadHotels();
  }

  loadHotels(){
    this.hotelService.getHotels().subscribe({
      next: res=>this.hotels.set(res),
      error: ()=>{}
    })
  }

  loadApprovals(){
    this.approvalsLoading.set(true);
    this.hotelService.getPendingHotels().subscribe({
      next: hotels => this.pendingHotels.set(hotels),
      error: () => {}
    });
    this.hotelService.getPendingRooms().subscribe({
      next: rooms => { this.pendingRooms.set(rooms); this.approvalsLoading.set(false); },
      error: () => this.approvalsLoading.set(false)
    });
  }

  switchTab(tab: 'bookings'|'hotels'|'approvals'){
    this.activeTab.set(tab);
    if(tab === 'approvals') this.loadApprovals();
  }

  approveHotel(id: number){
    this.hotelService.approveHotel(id).subscribe({
      next: () => this.pendingHotels.update(list => list.filter(h => h.id !== id)),
      error: () => {}
    });
  }

  rejectHotel(id: number){
    this.hotelService.rejectHotel(id).subscribe({
      next: () => this.pendingHotels.update(list => list.filter(h => h.id !== id)),
      error: () => {}
    });
  }

  approveRoom(hotelId: number, roomId: number){
    this.hotelService.approveRoom(hotelId, roomId).subscribe({
      next: () => this.pendingRooms.update(list => list.filter(r => r.id !== roomId)),
      error: () => {}
    });
  }

  rejectRoom(hotelId: number, roomId: number){
    this.hotelService.rejectRoom(hotelId, roomId).subscribe({
      next: () => this.pendingRooms.update(list => list.filter(r => r.id !== roomId)),
      error: () => {}
    });
  }

  createHotel(){
    if(!this.newHotel().name || !this.newHotel().city){
      return;
    }

    this.hotelSaving.set(true);
    this.hotelService.createHotel(this.newHotel()).subscribe({
      next: h=>{
        this.hotels.update(list => [h, ...list]);
        this.newHotel.set({
           name: '', city: '', address: '', description: '', starRating: 3, amenities: ''
        });
        this.hotelSaving.set(false);
      },
      error: ()=>{
        this.hotelSaving.set(false);
      }
    })
  }


  addRoom(hotelId: number){
    if(!this.newRoom().type || !this.newRoom().pricePerNight){
      return
    }

    this.roomSaving.set(true);

    this.hotelService.addRoom(hotelId, this.newRoom()).subscribe({
      next: r=>{
        this.hotels.update(list => list.map(h =>{
          if(h.id === hotelId){
            return {...h, rooms: [...h.rooms, r]};
          }
          return h;
        }));
        this.newRoom.set({
          type: '', pricePerNight: 0, maxOccupancy: 2, description: ''
        })

        this.selectedHotelId.set(null);
        this.roomSaving.set(false);
      },
      error: () => this.roomSaving.set(false)
    })
  }


  deleteHotel(id: number){
    if(!confirm('Delete this hotel and all its rooms?')) return;
    this.hotelService.deleteHotel(id).subscribe({
      next: () => this.hotels.update(list => list.filter(h => h.id !== id)),
      error: () => {}
    });
  }

  confirmedCount(){ 
    return this.bookings().filter(b => b.status === 'Confirmed').length; 
  }

  cancelledCount(){ 
    return this.bookings().filter(b => b.status === 'Cancelled').length; 
  }


}
