import { Component, OnInit } from '@angular/core';
import { TaskModelModule } from '../Model/task-model.module'
import { TaskserviceService } from '../taskservice.service'
import { Input } from '@angular/core';
import { TaskFilterPipe } from '../Filters/TaskFilter'
import { ParentTaskModelModule } from '../Model/parent-task-model.module';
import { ActivatedRoute,Router } from '@angular/router';
import { RouterPreloader } from '@angular/router/src/router_preloader';

@Component({
  selector: 'app-view-task',
  templateUrl: './view-task.component.html',
  styleUrls: ['./view-task.component.css']
})
export class ViewTaskComponent implements OnInit {

  filterTaskDetails : TaskModelModule;
  filteredDetails : TaskModelModule[];
  IsTaskCompleted : boolean;

  @Input()
  AllTasksInfo: TaskModelModule[];

  constructor(private taskService: TaskserviceService, private taskFilering : TaskFilterPipe, private router:Router) { 
    this.filterTaskDetails = new TaskModelModule();
    this.filterTaskDetails.ParentTaskModelModule = new ParentTaskModelModule();

    
  }

  ngOnInit() {
    
    this.taskService.getAllTasks().subscribe(x =>
      { 
        console.log(x);
        this.AllTasksInfo = x;
        this.filteredDetails = this.AllTasksInfo;
        // x.forEach(element => {
        //   console.log("Filter Element" + element.ParentTaskModelModule);           
        //});
        console.log("Filter " + this.filteredDetails);        
      });    
      
  }

  OnSearchCriteriaChanged(searchValue : string ) 
  {     

    this.filteredDetails = this.taskFilering.transform(this.AllTasksInfo, this.filterTaskDetails);      
  }

  OnEdit(selectedItem : TaskModelModule){
    console.log(selectedItem)
    this.router.navigate(["/addtask", selectedItem.TaskId]);
  }

  OnEnd(selectedItem : TaskModelModule){
    console.log(selectedItem)
    selectedItem.IsFinished = true;
    // this.taskService.UpdateTaskDetails(selectedItem).subscribe(x => {
    //   console.log("Updated Task to End State...")});
  }


}
