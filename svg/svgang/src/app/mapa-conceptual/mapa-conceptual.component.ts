import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-mapa-conceptual',
  templateUrl: './mapa-conceptual.component.html',
  styleUrls: ['./mapa-conceptual.component.css']
})
export class MapaConceptualComponent implements OnInit {

  constructor() { }

  public raa : number = 5;
  public move: number = 0;
  
  muevelooo(): void {
    this.move -= 5;
  }
  
  ngOnInit(): void {
  }

}
