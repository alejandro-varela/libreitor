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

  mousedowndiv(x: number, y: number): void {
    // guardar puntos xy  
    console.log(x, y);
  }

  mouseupdiv(): void {
    // calcular distancia recorrida
    console.log("mouseup");
  }

  mouseoutdiv(): void {
    console.log("mouseout");
  }

  click_pelotita_que_se_agranda(event : Event): void {
    event.stopPropagation();
    console.log("click " + new Date());
  }

  ngOnInit(): void {
    //
  }
}
