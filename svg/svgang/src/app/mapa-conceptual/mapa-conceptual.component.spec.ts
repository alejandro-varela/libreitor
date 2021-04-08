import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MapaConceptualComponent } from './mapa-conceptual.component';

describe('MapaConceptualComponent', () => {
  let component: MapaConceptualComponent;
  let fixture: ComponentFixture<MapaConceptualComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MapaConceptualComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MapaConceptualComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
