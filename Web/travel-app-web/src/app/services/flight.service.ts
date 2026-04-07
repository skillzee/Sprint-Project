import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Airport, FlightOffer } from '../models/models';

const api = "http://localhost:5000/api/flights";
@Injectable({
  providedIn: 'root'
})

export class FlightService {

  constructor(private http: HttpClient) { }

  searchFlights(origin: string, destination: string, date: string, adults = 1, cabin = 'ECONOMY'){


    const params = new HttpParams().set('origin', origin).set('destination', destination).set('date', date).set('adults', adults).set('cabin', cabin);

    return this.http.get<FlightOffer[]>(`${api}`, {params});

  }

  getAirports(){
    return this.http.get<Airport[]>(`${api}/airports`);
  }
}
