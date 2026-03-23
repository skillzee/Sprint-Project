import { HttpClient } from '@angular/common/http';
import { Injectable, signal } from '@angular/core';
import { Router } from '@angular/router';
import { tap } from 'rxjs';
import { User } from '../models/models';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly api = "http://localhost:5000/api/auth"; // This is my API Gatway
  currentUser = signal<User | null>(null);


  constructor(private http: HttpClient, private router: Router) 
  { 
    const saved = localStorage.getItem('travel_user');
    if(saved){
      this.currentUser.set(JSON.parse(saved));
    }
  }


  register(name: string, email: string, password: string, role = "User"){
    return this.http.post(`${this.api}/register`, {name, email, password, role}).pipe(tap(user => this.setUser(user)));

  }

  login(email: string, password: string){
    return this.http.post(`${this.api}/login`, {email, password}).pipe(tap(user => this.setUser(user)));
  }

  logout(){
    localStorage.removeItem('travel_user');
    this.currentUser.set(null);
    this.router.navigate(['/login']);
  }



  isLoggedIn(){
    return !!this.currentUser();
  }
  getToken() { 
    return this.currentUser()?.token; 
  }
  isAdmin(){
    return this.currentUser()?.role === 'Admin';
  }



  private setUser(user: any){
    localStorage.setItem('travel_user', JSON.stringify(user));
    this.currentUser.set(user);
  }
}
