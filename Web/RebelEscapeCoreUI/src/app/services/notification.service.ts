import { Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  constructor(private _snackbar: MatSnackBar) { }

  showNotification(message: string, duration: number = 2000): void {
    this._snackbar.open(message, undefined, {
      duration: duration
    })
  }

  showErrorNotification(message: string, duration: number = 2000): void {
    this._snackbar.open(message, undefined, {
      duration: duration,
      panelClass: ['red-snackbar']
    })
  }
}
