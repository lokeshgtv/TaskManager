import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { appRoutes } from './/app-routing.module';
import { AppComponent } from './app.component';
import { AddTaskComponent } from './add-task/add-task.component';
import { ViewTaskComponent } from './view-task/view-task.component';
import { RouterModule } from '@angular/router';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { TaskFilterPipe } from './Filters/TaskFilter';
import { TaskserviceService } from './taskservice.service';
import { EdittaskComponent } from './edittask/edittask.component';
import { DatePipe } from '@angular/common';

@NgModule({
  declarations: [
    AppComponent,
    AddTaskComponent,
    ViewTaskComponent,    
    TaskFilterPipe,     
    EdittaskComponent,
  ],
  imports: [
    BrowserModule,
    RouterModule.forRoot(appRoutes),
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule
  ],
  providers: [ TaskserviceService, TaskFilterPipe, DatePipe  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
