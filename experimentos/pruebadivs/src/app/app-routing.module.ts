import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { DiviComponent } from './divi/divi.component';

const routes: Routes = [
  { path: ""    , component: DiviComponent },
  { path: "divi", component: DiviComponent },
  { path: "**"  , component: DiviComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
