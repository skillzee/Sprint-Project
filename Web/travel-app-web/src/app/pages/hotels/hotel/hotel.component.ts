import { CommonModule } from '@angular/common';
import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { Hotel } from '../../../models/models';
import { HotelService } from '../../../services/hotel.service';

@Component({
  selector: 'app-hotel',
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './hotel.component.html',
  styleUrl: './hotel.component.css'
})
export class HotelComponent {
  hotels = signal<Hotel[]>([]);
  loading = signal(false);
  city = '';


  constructor(private hotelService: HotelService){};

  ngOnInit(){
    this.search();
  }

  search(){
    this.loading.set(true);
    this.hotelService.getHotels(this.city||undefined).subscribe({
      next: h=> {
        this.hotels.set(h)
        this.loading.set(false)
      },
      error: ()=>{
        this.loading.set(false);
      }
    })
  }


  reset(){
    this.city = "";
    this.search();
  }

  getStars(n: number){
    return Array(Math.round(n)).fill(0);
  }

  getAmenities(a: string) 
  { 
    return a.split(',').slice(0, 4); 
  }


  getMinPrice(h: Hotel) 
  { 
    return h.rooms.length ? Math.min(...h.rooms.map(r => r.pricePerNight)) : 0; 
  }


}


