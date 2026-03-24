import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CreateTripDto, GenerateItineraryDto, Trip } from '../models/models';

const api = 'http://localhost:5000/api/trips'

@Injectable({
  providedIn: 'root'
})
export class TripService {

  constructor(private http: HttpClient) { }

  getMyTrips(){
    return this.http.get<Trip[]>(api);
  }

  getTripById(id: number){
    return this.http.get<Trip>(`${api}/${id}`)
  }

  createTrip(dto: CreateTripDto){ 
    return this.http.post<Trip>(`${api}`, dto); 
  }
  generateItinerary(dto: GenerateItineraryDto){ 
    return this.http.post<Trip>(`${api}/generate-itinerary`, dto); 
  }
  deleteTrip(id: number){ 
    return this.http.delete<void>(`${api}/${id}`); 
  }
}
