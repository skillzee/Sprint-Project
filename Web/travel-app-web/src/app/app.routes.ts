import { Routes } from '@angular/router';
import { LoginComponent } from './pages/auth/login/login.component';
import { HomeComponent } from './pages/home/home.component';
import { FlightsComponent } from './pages/flights/flights.component';
import { RegisterComponent } from './pages/auth/register/register.component';
import { HotelsComponent } from './pages/hotels/hotels.component';
import { TripsComponent } from './pages/trips/trips.component';

export const routes: Routes = [
    {path: "", component: HomeComponent},
    { path: "login", component: LoginComponent },
    {path: "register", component: RegisterComponent},
    {path: "flights", component: FlightsComponent},
    {path: "hotels", component: HotelsComponent},
    {path: "trips", component: TripsComponent}

];
