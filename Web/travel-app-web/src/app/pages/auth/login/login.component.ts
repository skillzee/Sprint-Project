import { Component, signal } from '@angular/core';
import { AuthService } from '../../../services/auth.service';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { GoogleSigninButtonModule, SocialAuthService } from '@abacritt/angularx-social-login';

@Component({
  selector: 'app-login',
  imports: [FormsModule, RouterLink, GoogleSigninButtonModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  email = ''; password = '';
  loading = signal(false); error = signal('');

   constructor(
     private auth: AuthService, 
     private router: Router,
     private socialAuthService: SocialAuthService
   ) {
     this.socialAuthService.authState.subscribe((user) => {
       if (user) {
         this.loading.set(true);
         this.auth.googleLogin(user.idToken!).subscribe({
           next: () => this.router.navigate(['/']),
           error: err => { 
             this.error.set(err.error?.message || 'Google login failed'); 
             this.loading.set(false); 
           }
         });
       }
     });
   }

    login() {
    this.loading.set(true); this.error.set('');
    this.auth.login(this.email, this.password).subscribe({
      next: () => this.router.navigate(['/']),
      error: err => { this.error.set(err.error?.message || 'Invalid credentials'); this.loading.set(false); }
    });
  }

}
