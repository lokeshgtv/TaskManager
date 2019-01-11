import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {RouterModule, Routes} from '@angular/router';
import { AddTaskComponent } from './add-task/add-task.component'
import { ViewTaskComponent } from './view-task/view-task.component';
import { EdittaskComponent } from './edittask/edittask.component';

// @NgModule({
//   declarations: [],
//   imports: [
//     CommonModule
//   ]
// })
// export class AppRoutingModule { }

export const appRoutes: Routes =
[
  {path : 'addtask', component : AddTaskComponent},
  {path : 'viewtask', component : ViewTaskComponent},
  {path : 'addtask/:id', component : AddTaskComponent}
]
