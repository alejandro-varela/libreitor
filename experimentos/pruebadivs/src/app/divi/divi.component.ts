// para leer https://medium.com/angular-in-depth/creating-reusable-angular-components-how-to-avoid-the-painful-trap-most-go-in-d531761d6c7a#:~:text=There%20are%20two%20main%20ways,how%20to%20render%20the%20data.

import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-divi',
  templateUrl: './divi.component.html',
  styleUrls: ['./divi.component.css']
})
export class DiviComponent implements OnInit {

  // var touchDevice = (navigator.maxTouchPoints || 'ontouchstart' in document.documentElement);
  esTouch  : boolean = 'ontouchstart' in window;
  esMobile : boolean = /iPhone|iPad|iPod|Android/i.test(navigator.userAgent);

  private cwidth  : number = 866;
  private cheight : number = 1300; 

  imgw: number = this.cwidth;
  imgh: number = this.cheight;

  private down : boolean = false;
  imgx: number = 0;
  imgy: number = 0;
  size: number = 1;

  mdown(event : MouseEvent): void{
    event.preventDefault();
    this.down = true;
  }

  mup(event : MouseEvent): void{
    event.preventDefault();
    console.log("NUNCA SE DA");
    this.down = false;
  }

  mmove(event : MouseEvent): void{
    if (this.down)
    {
      this.imgx += event.movementX / 2;
      this.imgy += event.movementY / 2;
      console.log(event.movementX, event.movementY /*, this.down, this.imgx, this.imgy*/);
    }
  }

  mwheel(event: WheelEvent) : void{
    event.preventDefault();
    
    let newSize = this.size - (event.deltaY / 1000);
    
    if (newSize < 0.1) { newSize = 0.1; }
    if (newSize > 10 ) { newSize =  10; }
    
    this.size = newSize;

    let newimgh = this.cheight * this.size;
    let newimgw = this.cwidth  * this.size;

    let despx = (newimgh - this.imgh);
    let despy = (newimgw - this.imgw);

    this.imgh = newimgh;
    this.imgw = newimgw;
    this.imgx -= despx;
    this.imgy -= despy;

    console.log(this.size, event.deltaY);
  }

  constructor() { 
    //
  }

  ngOnInit(): void {
    console.log(navigator.userAgent);
    console.log("esTouch ", this.esTouch);
    console.log("esMobile", this.esMobile);
  }

}
