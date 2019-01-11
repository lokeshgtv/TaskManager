import { Injectable } from '@angular/core';
import { TaskModelModule } from './Model/task-model.module';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs/internal/Observable';
import { ParentTaskModelModule } from './Model/parent-task-model.module';
import { of } from 'rxjs';
// import { HttpHeaders } from '@angular/common/http';
// import { URLSearchParams } from 'url';
// import { HttpParams } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class TaskserviceService {

  constructor(private httpService: HttpClient) { }

  getAllTasks(): Observable<TaskModelModule[]> {  

      console.log("Test Connection")
      return this.httpService.get<TaskModelModule[]>("http://localhost/TaskManagerService/api/taskmodelmodules");
    
    // var tasks = this.httpService.get("http://localhost:59757/api/taskmodelmodules")
    //   .map((resp) =>
    //   {
    //       return resp.json();
    //   })
    //   .map(tasks : Array<any>)
    //   .map(item =>
    //       {
    //           var task = new TaskModelModule()
    //           task.TaskId = item.TaskId;
    //           task.TaskDescripton = item.TaskDescripton;
    //           task.Priority = item.Priority;
    //           task.StartDate = item.StartDate;
    //           task.EndDate = item.EndDate;
    //           task.ParentTaskModelModule = new ParentTaskModelModule();
    //           task.ParentTaskModelModule.ParentTaskId = item.ParentTaskModelModule.ParentTaskId;
    //           task.ParentTaskModelModule.ParentTaskName = item.ParentTaskModelModule.ParentTaskName;
    //           console.log("From Mapper: " + task);
    //           return task;

    //       });
    //   });

    // return of(tasks);
    //return tasks;
    //return of(this.taskData);
  }

  getTaskById(id:number): Observable<TaskModelModule> {

    return this.httpService.get<TaskModelModule>("http://localhost/TaskManagerService/api/taskmodelmodules/"+id);
    //return of(this.taskData.find(x => x.TaskId == id));
  }

  AddTaskDetails(task: TaskModelModule) {

    console.log("From Add : " + task);
    return this.httpService.post("http://localhost/TaskManagerService/api/taskmodelmodules", task);
  }

  UpdateTaskDetails(task: TaskModelModule) {

    //return this.httpService.put("http://localhost/TaskManagerService/api/taskmodelmodules/"+task.TaskId,task);    
    return this.httpService.post("http://localhost/TaskManagerService/api/taskmodelmodules", task);
  }

  private taskData: TaskModelModule[] = [
    { TaskId: 1, TaskDescripton: "Task 1", ParentTaskModelModule: { ParentTaskId: 1, ParentTaskName: "Parent Task1" }, Priority: 1, PriorityTo: 30, StartDate: new Date(2018, 12, 6), EndDate: new Date(2018, 12, 20), IsFinished:true },
    { TaskId: 2, TaskDescripton: "Task 2", ParentTaskModelModule: { ParentTaskId: 2, ParentTaskName: "Parent Task2" }, Priority: 30, PriorityTo: 30, StartDate: new Date(2018, 11, 25), EndDate: new Date(2018, 12, 10), IsFinished:false },
    { TaskId: 3, TaskDescripton: "Task 3", ParentTaskModelModule: { ParentTaskId: 2, ParentTaskName: "Parent Task3" }, Priority: 20, PriorityTo: 30, StartDate: new Date(2018, 10, 11), EndDate: new Date(2018, 10, 28), IsFinished:false },
    { TaskId: 4, TaskDescripton: "Task 4", ParentTaskModelModule: null, Priority: 20, PriorityTo: 30, StartDate: new Date(2018, 10, 11), EndDate: new Date(2018, 10, 28), IsFinished:false }
  ];

  private ParentTaskModelModule: ParentTaskModelModule[] = [
    { ParentTaskId: 1, ParentTaskName: "Parent Task1" }, { ParentTaskId: 2, ParentTaskName: "Parent Task2" },
  ];
}
