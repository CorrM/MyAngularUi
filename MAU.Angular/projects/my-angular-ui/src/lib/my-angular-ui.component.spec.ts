import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MyAngularUiComponent } from './my-angular-ui.component';

describe('MyAngularUiComponent', () => {
  let component: MyAngularUiComponent;
  let fixture: ComponentFixture<MyAngularUiComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MyAngularUiComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MyAngularUiComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
