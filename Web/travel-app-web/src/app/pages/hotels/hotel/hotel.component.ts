import { CommonModule } from '@angular/common';
import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { Hotel } from '../../../models/models';

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


  constructor(private hotelService: ){};
}


