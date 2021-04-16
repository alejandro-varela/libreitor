// sistemas de coordenadas de SVG
// https://www.sarasoueidan.com/blog/svg-coordinate-systems/

// scale from center
// https://www.javatpoint.com/svg-scaling-around-a-center-point

import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-mapa-conceptual',
  templateUrl: './mapa-conceptual.component.html',
  styleUrls: ['./mapa-conceptual.component.css']
})
export class MapaConceptualComponent implements OnInit {

  constructor() { }
  
  public vbx: number = 0;
  public vby: number = 0;

  public raa  : number = 5;
  public movex: number = 0;
  public movey: number = 0;
  public size : number = 1;

  eventos : string[] = [ "nada" ];

  mousewheeldiv(event : WheelEvent): void {
    event.preventDefault();
    this.size -= event.deltaY / 30;
    //console.log(event.deltaY);
    this.avisarEvento("rueda");
  }

  mousedowndiv(x: number, y: number): void {
    // guardar puntos xy  
    // this.vbx = x;
    // this.vby = y;
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

  tocastart(event: TouchEvent): void {
    event.preventDefault();
    this.avisarEvento("touchstart");
  }

  tocaend(event: TouchEvent): void {
    event.preventDefault();
    this.avisarEvento("touchend");
  }

  avisarEvento(sEvento : string) : void {
    this.eventos.push(sEvento + " :: " + new Date());
  }

  ngOnInit(): void {
    //
  }
}
