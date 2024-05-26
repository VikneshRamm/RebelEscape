import { Component, Inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import {
  MatDialogTitle,
  MatDialogContent,
  MatDialogActions,
  MatDialogClose,
  MatDialogRef,
  MAT_DIALOG_DATA,
} from '@angular/material/dialog';
import { GameConfirmationResult } from '../../models/gameconfirmation.model';

@Component({
  selector: 'app-game-accept-dialog',
  standalone: true,
  imports: [
    MatButtonModule,
    MatDialogTitle,
    MatDialogContent,
    MatDialogActions,
    MatDialogClose,],
  templateUrl: './game-accept-dialog.component.html',
  styleUrl: './game-accept-dialog.component.scss'
})
export class GameAcceptDialogComponent {
  
  constructor(public dialogRef: MatDialogRef<GameAcceptDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public playerName: string,) {
  }

  onNoButtonClick() {
    this.dialogRef.close(GameConfirmationResult.Rejected);
  }

  onAcceptButtonClick() {
    this.dialogRef.close(GameConfirmationResult.Accepted)
  }
}
