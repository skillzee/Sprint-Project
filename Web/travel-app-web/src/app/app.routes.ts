import { Routes } from '@angular/router';
import { LoginComponent } from './pages/auth/login/login.component';
import { HomeComponent } from './pages/home/home.component';
import { FlightsComponent } from './pages/flights/flights.component';
import { RegisterComponent } from './pages/auth/register/register.component';
import { TripsComponent } from './pages/trips/trips.component';
import { BookingsComponent } from './pages/bookings/bookings.component';
import { HotelComponent } from './pages/hotels/hotel/hotel.component';
import { HotelDetailComponent } from './pages/hotels/hotel-detail/hotel-detail.component';

export const routes: Routes = [
    {path: "", component: HomeComponent},
    { path: "login", component: LoginComponent },
    {path: "register", component: RegisterComponent},
    {path: "flights", component: FlightsComponent},
    {path: "hotels", component: HotelComponent},
    {path: "hotel/:id", component: HotelDetailComponent},
    {path: "trips", component: TripsComponent},
    {path: "bookings", component: BookingsComponent}


];
