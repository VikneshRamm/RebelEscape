import { TestBed } from '@angular/core/testing';

import { HubCommunicationService } from './hubcommunication.service';

describe('HubcommunicationService', () => {
  let service: HubCommunicationService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(HubCommunicationService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
