import { TestBed } from '@angular/core/testing';

import { HubconnectionService } from './hubconnection.service';

describe('HubconnectionService', () => {
  let service: HubconnectionService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(HubconnectionService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
