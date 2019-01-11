import { ParentTaskModelModule } from './parent-task-model.module';

export class TaskModelModule { 
  TaskId : number;
  TaskDescripton : string;
  ParentTaskModelModule : ParentTaskModelModule;
  Priority: Number;
  PriorityTo : Number;
  StartDate : Date;
  EndDate : Date;
  IsFinished : boolean; 
}
