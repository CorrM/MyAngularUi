import { TestBed } from '@angular/core/testing';

import { MyAngularUiService } from './my-angular-ui.service';

describe('MyAngularUiService', () => {
  let service: MyAngularUiService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(MyAngularUiService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
