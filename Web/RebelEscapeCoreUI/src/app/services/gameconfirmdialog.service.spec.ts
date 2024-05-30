import { TestBed } from '@angular/core/testing';

import { GameConfirmDialogService } from './gameconfirmdialog.service';

describe('GameconfirmdialogService', () => {
  let service: GameConfirmDialogService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(GameConfirmDialogService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
