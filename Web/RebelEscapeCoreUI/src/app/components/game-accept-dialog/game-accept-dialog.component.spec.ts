import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GameAcceptDialogComponent } from './game-accept-dialog.component';

describe('GameAcceptDialogComponent', () => {
  let component: GameAcceptDialogComponent;
  let fixture: ComponentFixture<GameAcceptDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GameAcceptDialogComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(GameAcceptDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
