import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Booking, CreateBookingDto } from '../models/models';

const api = 'http://localhost:5000/api/bookings'

@Injectable({
  providedIn: 'root'
})
export class BookingService {

  constructor(private http: HttpClient) { }

  create(dto: CreateBookingDto){
    return this.http.post<Booking>(`${api}`, dto);
  }

  getMyBooking(){
    return this.http.get<Booking[]>(`${api}/my`);
  }

  getAllBookings(){
    return this.http.get<any>(`${api}`)
  }
  cancel(id: number){ 
    return this.http.put<any>(`${api}/cancel/${id}`, {}); 
  }

}
