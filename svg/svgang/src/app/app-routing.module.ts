import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { MapaConceptualComponent } from './mapa-conceptual/mapa-conceptual.component';

const routes: Routes = [
  { path: "", component: MapaConceptualComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
