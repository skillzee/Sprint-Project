import { Component, signal } from '@angular/core';
import { AuthService } from '../../../services/auth.service';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-register',
  imports: [FormsModule, RouterLink],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {

  name = ''; email = ''; password = '';
  loading = signal(false);
  error = signal(''); 
  success = signal('');

  constructor(private auth: AuthService, private router: Router) { }

  register() {
    this.loading.set(true);
    this.error.set('');
    this.auth.register(this.name, this.email, this.password).subscribe({
      next: () =>{
        this.success.set('Account created!');
        this.router.navigate(['/'])
      },
      error: err => { this.error.set(err.error?.message || 'Registration failed'); this.loading.set(false); }
    });
  }
}
