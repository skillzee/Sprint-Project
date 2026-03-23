export interface User{
    id: number,
    name: string,
    email: string,
    role: string,
    token: string
}


export interface FlightOffer {
  id: string;
  airline: string;
  airlineCode: string;
  flightNumber: string;
  origin: string;
  originIata: string;
  destination: string;
  destinationIata: string;
  departureTime: string;
  arrivalTime: string;
  duration: string;
  price: number;
  currency: string;
  seatsAvailable: number;
  cabinClass: string;
  isNonStop: boolean;
  bookingUrl: string;  // Google Flights deeplink
}

export interface Airport {
  code: string;
  city: string; 
  country: string;
}


export interface Hotel {
  id: number;
  name: string; 
  city: string; 
  address: string;
  description: string; 
  starRating: number; 
  amenities: string; 
  rooms: Room[];
}

export interface Room {
  id: number; 
  hotelId: number; 
  type: string;
  pricePerNight: number; 
  isAvailable: boolean;
  maxOccupancy: number; 
  description: string;
}
