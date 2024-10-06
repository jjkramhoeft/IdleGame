import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PageInventoryComponent } from './page-inventory.component';

describe('PageInventoryComponent', () => {
  let component: PageInventoryComponent;
  let fixture: ComponentFixture<PageInventoryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PageInventoryComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PageInventoryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
