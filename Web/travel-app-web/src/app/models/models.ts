export interface User {
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


export interface CreateHotelDto {
  name: string; 
  city: string; 
  address: string;
  description: string; 
  starRating: number; 
  amenities: string;
}

export interface CreateRoomDto {
  type: string; 
  pricePerNight: number;
  maxOccupancy: number; 
  description: string;
}


export interface CreateBookingDto {
  roomId: number; 
  roomType: string; 
  hotelName: string;
  checkInDate: string; 
  checkOutDate: string; 
  pricePerNight: number;
}



export interface Booking {
  id: number; 
  userId: number; 
  userName: string;
  roomId: number; 
  roomType: string; 
  hotelName: string;
  checkInDate: string; 
  checkOutDate: string;
  totalPrice: number; 
  status: string; 
  bookingRef: string; 
  createdAt: string;
}


export interface CreateBookingDto {
  roomId: number; 
  roomType: string; 
  hotelName: string;
  checkInDate: string; 
  checkOutDate: string; 
  pricePerNight: number;
}




export interface Itinerary {
  id: number;
  tripId: number;
  dayNumber: number;
  activity: string;
  location: string;
}

export interface Trip {
  id: number;
  userId: number;
  destination: string;
  startDate: string;
  endDate: string;
  createdAt: string;
  itineraries: Itinerary[];
}

export interface CreateTripDto {
  destination: string;
  startDate: string;
  endDate: string;
}

export interface GenerateItineraryDto {
  tripId: number;
  preferences?: string;
}
