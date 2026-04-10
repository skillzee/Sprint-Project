import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CreateHotelDto, CreateRoomDto, Hotel, Room } from '../models/models';


const api = 'http://localhost:5000/api/hotels';

@Injectable({
  providedIn: 'root'
})


export class HotelService {
  
  constructor(private http : HttpClient) { }

  getHotels(city?: string){
    let params = new HttpParams();
    if(city){
      params = params.set('city', city);
    }

    return this.http.get<Hotel[]>(`${api}`, {params});

  }

  getHotel(id: number){
    return this.http.get<Hotel>(`${api}/${id}`);
  } 

  createHotel(dto: CreateHotelDto){
    return this.http.post<Hotel>(`${api}`, dto);
  }

  addRoom(hotelId: number, dto: CreateRoomDto){
    return this.http.post<Room>(`${api}/${hotelId}/rooms`, dto);
  }

  getMyHotels(){
    return this.http.get<Hotel[]>(`${api}/my`);
  }

  getPendingHotels(){
    return this.http.get<Hotel[]>(`${api}/pending`);
  }

  getPendingRooms(){
    return this.http.get<Room[]>(`${api}/rooms/pending`);
  }

  approveHotel(id: number){
    return this.http.put<Hotel>(`${api}/${id}/approve`, {});
  }

  rejectHotel(id: number){
    return this.http.put<Hotel>(`${api}/${id}/reject`, {});
  }

  approveRoom(hotelId: number, roomId: number){
    return this.http.put<Room>(`${api}/${hotelId}/rooms/${roomId}/approve`, {});
  }

  rejectRoom(hotelId: number, roomId: number){
    return this.http.put<Room>(`${api}/${hotelId}/rooms/${roomId}/reject`, {});
  }

}
