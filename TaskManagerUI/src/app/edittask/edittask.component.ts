import { Component, OnInit, ViewChild, ElementRef, ChangeDetectorRef } from '@angular/core';
import { TaskModelModule } from '../Model/task-model.module';
import { ParentTaskModelModule } from '../Model/parent-task-model.module';
import { TaskserviceService } from '../taskservice.service';
import { FormArray, Form, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { FormControl } from '@angular/forms'

import { Input } from '@angular/core';
import { Router, Route, ActivatedRoute, ParamMap } from '@angular/router';
import { switchMap } from 'rxjs/internal/operators/switchMap';
import { of } from 'rxjs';

@Component({
  selector: 'app-edittask',
  templateUrl: './edittask.component.html',
  styleUrls: ['./edittask.component.css']
})
export class EdittaskComponent implements OnInit {

  startDateFormattedString: any;
  endDateFormattedString: any;
  taskForm: FormGroup;
  tName: FormControl;
  tParentTask: FormControl;
  tPriority: FormControl;      
  // tStartDate: FormControl;
  // tEndDate: FormControl;

  

  public taskModel: TaskModelModule = new TaskModelModule();
  parentTaskModel: ParentTaskModelModule;


  constructor(private taskService: TaskserviceService, private taskRouter: Router, private activeRoute: ActivatedRoute, private changeDetector: ChangeDetectorRef) {    
    this.taskModel = new TaskModelModule();
    this.taskModel.ParentTaskModelModule = new ParentTaskModelModule();   
     
    this.initFormControls();
    this.activeRoute.paramMap.pipe(switchMap((parms: ParamMap) => {
      console.log(parms);
      if (parms.has("id")) {
        let id = Number.parseInt(parms.get("id"));
        return this.taskService.getTaskById(id);
      }
      return of(new TaskModelModule());
    })).subscribe(x => {      
      this.taskModel = x                    
      
      this.tName.setValue(x.TaskDescripton);
      this.tParentTask.setValue(x.ParentTaskModelModule.ParentTaskName);
      this.tPriority.setValue(x.Priority);     
      
    });

  }  

  initFormControls()
  {
    this.tName = new FormControl(this.taskModel.TaskDescripton);
    this.tPriority = new FormControl(this.taskModel.Priority);
    this.tParentTask = new FormControl(this.taskModel.ParentTaskModelModule.ParentTaskName);
    //this.tStartDate = new FormControl(new Date(this.taskModel.StartDate).toISOString().slice(0, -1));
    // this.tStartDate = new FormControl(this.taskModel.StartDate);
    // this.tEndDate = new FormControl(this.taskModel.EndDate);  
    
    this.taskForm = new FormGroup({ taskName: this.tName, priority: this.tPriority, ParentTaskModelModule: this.tParentTask});
  }
  ngOnInit() {

    this.tName = new FormControl(this.taskModel.TaskDescripton);
    this.tPriority = new FormControl(this.taskModel.Priority);
    this.tParentTask = new FormControl(this.taskModel.ParentTaskModelModule.ParentTaskName);
    //this.tStartDate = new FormControl(new Date(this.taskModel.StartDate).toISOString().slice(0, -1));
    // this.tStartDate = new FormControl(this.taskModel.StartDate);
    // this.tEndDate = new FormControl(this.taskModel.EndDate);  

    this.taskForm = new FormGroup({ taskName: this.tName, priority: this.tPriority, ParentTaskModelModule: this.tParentTask});

  }

  AddTask() {
    if (this.taskForm.valid) {
      console.log(this.taskModel);
      console.log(this.taskModel.StartDate);
      console.log(this.taskModel.EndDate);      
      // this.taskModel.TaskDescripton = this.tName.value;
      // this.taskModel.Priority = this.tPriority.value;
      // this.taskModel.ParentTaskModelModule = this.tParentTask.value;
      // // this.taskModel.StartDate = this.tStartDate.value;
      // // this.taskModel.EndDate = this.tEndDate.value;

      // this.taskService.AddTaskDetails(this.taskModel).subscribe(x => {
      //   console.log("Task Saved...");
      // });
      // console.log("Form is Valid");
    }
  }

  MyFunction(){
console.log("test");
  }

}
