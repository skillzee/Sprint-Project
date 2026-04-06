import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { FlightService } from '../../services/flight.service';
import { Airport, FlightOffer } from '../../models/models';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-flights',
  imports: [FormsModule, CommonModule],
  templateUrl: './flights.component.html',
  styleUrl: './flights.component.css'
})
export class FlightsComponent {

  airports = signal<Airport[]>([]);
  flights = signal<FlightOffer[]>([]);
  loading = signal(false);
  hasSearched = signal(false);
  searchError = signal('');


  origin = 'DEL';
  destination = 'BOM';
  date = '';
  adults = 1;
  cabin = 'ECONOMY';
  today = new Date().toISOString().split('T')[0];

  constructor(private flightService: FlightService){}


  ngOnInit(){

    const d = new Date();
    d.setDate(d.getDate()+ 7);
    this.date= d.toISOString().split("T")[0];

    this.flightService.getAirports().subscribe({
      next: a=> this.airports.set(a),
      error: ()=>{}
    });
  }


  originName(){
    return this.airports().find(a=>a.code == this.origin)?.city ?? this.origin
  }

  destinationName(){
    return this.airports().find(a => a.code == this.destination)?.city ?? this.destination;
  }

  swap(){
    [this.origin, this.destination] = [this.destination, this.origin]
  }

  search(){
    const originValue = this.origin?.trim();
    const destinationValue = this.destination?.trim();

    if(!originValue || !destinationValue || !this.date){
      return;
    }

    if(originValue.toUpperCase() === destinationValue.toUpperCase()){
      this.searchError.set("Origin and Destination cannot be the same");
      return;
    }

    this.origin = originValue;
    this.destination = destinationValue;
    this.searchError.set('');
    this.loading.set(true);
    this.hasSearched.set(true);

    this.flightService.searchFlights(this.origin, this.destination, this.date, this.adults, this.cabin).subscribe({
      next: f=> {
        this.flights.set(f);
        this.loading.set(false);
      },
      error: ()=>{
        this.flights.set([]);
        this.searchError.set('Failed to fetch flights. Check your Amadeus API credentials in appsettings.json.')
        this.loading.set(false);
      }
    })

  }

   formatDuration(iso: string): string {
    // "PT2H30M" → "2h 30m"
    const match = iso.match(/PT(?:(\d+)H)?(?:(\d+)M)?/);
    if (!match) return iso;
    const h = match[1] ? `${match[1]}h` : '';
    const m = match[2] ? `${match[2]}m` : '';
    return [h, m].filter(Boolean).join(' ');
  }

  cabinClass(cabin: string): string {
    const map: Record<string, string> = {
      ECONOMY: 'cabin-economy', PREMIUM_ECONOMY: 'cabin-premium',
      BUSINESS: 'cabin-business', FIRST: 'cabin-first'
    };
    return map[cabin] ?? 'cabin-economy';
  }




}
