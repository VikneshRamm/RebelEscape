import { Component } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { AuthService } from '../../services/auth.service';
import { FormsModule } from '@angular/forms';
import { NotificationService } from '../../services/notification.service';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';


@Component({
  selector: 'app-login',
  standalone: true,
  imports: [MatCardModule, MatInputModule, MatButtonModule, FormsModule, CommonModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
    
  public userName: string;

  constructor(private _authService: AuthService,
     private _notificationService: NotificationService,
     private _routeService: Router) {
      this.userName = '';
   }
  
  onLoginClick() {
    this._authService.login(this.userName).subscribe({
      next: () => {
        this._routeService.navigateByUrl('/players-list');
      },
      error: (error) => {
        console.log(error);
        this._notificationService.
        showErrorNotification(`Error during login. Error: ${error.statusText}`,
         5000);
      }
    });
  }
}
