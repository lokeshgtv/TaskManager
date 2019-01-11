import { Pipe, PipeTransform } from '@angular/core';
import { TaskModelModule } from '../Model/task-model.module';
import { IMPLICIT_REFERENCE } from '@angular/compiler/src/render3/view/util';
import { DatePipe } from '@angular/common'

@Pipe({
name: 'taskFilter'
})

export class TaskFilterPipe implements PipeTransform
{
    constructor(public datepipe: DatePipe){}
    
    transform(items : TaskModelModule[], filter : TaskModelModule) : any {
        console.log("Filer Pipe fired " + filter)        
        if(!items || filter == null)
        {
            console.log("Filer returned");
            console.log(items);
            return items;
        }
        console.log(items);
        console.log(filter);
        items = items.filter(item =>          
                 (!filter.TaskDescripton ? true : item.TaskDescripton.indexOf(filter.TaskDescripton) == 0) &&
                 (!filter.ParentTaskModelModule.ParentTaskName ? true : item.ParentTaskModelModule.ParentTaskName.indexOf(filter.ParentTaskModelModule.ParentTaskName) != -1) &&
                 (!filter.Priority ? true : item.Priority >= filter.Priority) &&
                 (!filter.PriorityTo ? true : item.PriorityTo <= filter.PriorityTo) &&
                 (!filter.StartDate ? true : this.GetFormattedDate(item.StartDate) >= this.GetFormattedDate(filter.StartDate)) &&
                 (!filter.EndDate ? true :this.GetFormattedDate(item.EndDate) <= this.GetFormattedDate(filter.EndDate))                  
        );       
        return items;
    }

    GetFormattedDate(dateToFormat : Date){
        console.log(this.datepipe.transform(dateToFormat, 'yyyy-MM-dd'));
        return this.datepipe.transform(dateToFormat, 'yyyy-MM-dd');
    }
}