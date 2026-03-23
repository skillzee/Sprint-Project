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