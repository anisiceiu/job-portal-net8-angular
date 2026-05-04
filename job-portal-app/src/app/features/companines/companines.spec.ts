import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Companines } from './companines';

describe('Companines', () => {
  let component: Companines;
  let fixture: ComponentFixture<Companines>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Companines]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Companines);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
