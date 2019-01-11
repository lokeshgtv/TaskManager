using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Results;
using TaskManagerData;
using TaskManagerService.Controllers;

namespace TaskManagerService.Tests.Controllers
{
    [TestClass]
    public class TaskManagerControllerTest
    {         
        private List<ParentTaskModelModule> GetMockedParentTaskModelData()
        {
            return new List<ParentTaskModelModule>
            {
                new ParentTaskModelModule{ ParentTaskId = 101, ParentTaskName = "Parent Task 1" },
                new ParentTaskModelModule{ ParentTaskId = 102, ParentTaskName = "Parent Task 2" },
                new ParentTaskModelModule{ ParentTaskId = 103, ParentTaskName = "Parent Task 3" },
                new ParentTaskModelModule{ ParentTaskId = 104, ParentTaskName = "Parent Task 4" },
                new ParentTaskModelModule{ ParentTaskId = 105, ParentTaskName = "Parent Task 5" }
            };
        }

        private IQueryable<TaskModelModule> GetMockedTaskModelData()
        {
            return new List<TaskModelModule>
            {
                new TaskModelModule { TaskId = 1, TaskDescripton = "Task 1", Priority = 5, StartDate = new DateTime(2018,12,01),
                    EndDate = new DateTime(2019, 12, 01), IsFinished = false, ParentTaskId = 101, ParentTaskModelModule = GetMockedParentTaskModelData()[0] },
                new TaskModelModule { TaskId = 2, TaskDescripton = "Task 2", Priority = 15, StartDate = new DateTime(2018,02,01),
                    EndDate = new DateTime(2018, 12, 21), IsFinished = true, ParentTaskId = 102, ParentTaskModelModule = GetMockedParentTaskModelData()[1] },
            }.AsQueryable();
        }
                
        public TaskModelModulesController GetMockedTaskManagerService(bool mockedModelState = true, bool failSave = false)
        {
            var mockset = new Mock<DbSet<TaskModelModule>>();
            var modifiedMockTaskData = new List<TaskModelModule>();
            var mockTaskData = GetMockedTaskModelData();
            mockset.As<IQueryable<TaskModelModule>>().Setup(m => m.Provider).Returns(mockTaskData.Provider);
            mockset.As<IQueryable<TaskModelModule>>().Setup(m => m.Expression).Returns(mockTaskData.Expression);
            mockset.As<IQueryable<TaskModelModule>>().Setup(m => m.ElementType).Returns(mockTaskData.ElementType);
            mockset.As<IQueryable<TaskModelModule>>().Setup(m => m.GetEnumerator()).Returns(() => mockTaskData.GetEnumerator());           
            mockset.Setup(m => m.Find(It.IsAny<object[]>())).Returns<object[]>(x =>
            {
                var taskIdToSearch = x.First();
                var foundData = mockTaskData.ToList().FirstOrDefault(y => y.TaskId == (int)taskIdToSearch);
                return foundData;
            });
            mockset.Setup(m => m.Add(It.IsAny<TaskModelModule>())).Callback<TaskModelModule>((item) =>
            {
                modifiedMockTaskData = mockTaskData.ToList();
                modifiedMockTaskData.Add(item);                
            });
            mockset.Setup(m => m.Remove(It.IsAny<TaskModelModule>())).Callback<TaskModelModule>((item) =>
            {
                modifiedMockTaskData = mockTaskData.ToList();
                modifiedMockTaskData.Remove(item);                
            });            

            var mockContext = new Mock<TaskManagerEntities>();          
            mockContext.Setup(m => m.TaskModelModules).Returns(mockset.Object);
            mockContext.Setup(m => m.SetModified(It.IsAny<object>()))
              .Callback<object>(item =>
              {
                  var itemToModify = item as TaskModelModule;
                  if (itemToModify != null)
                  {
                      modifiedMockTaskData = mockTaskData.ToList();
                      var itemToReplace = modifiedMockTaskData.FirstOrDefault(x => x.TaskId == itemToModify.TaskId);
                      var index = modifiedMockTaskData.IndexOf(itemToReplace);
                      if (index == -1) return;
                      modifiedMockTaskData.RemoveAt(index);
                      modifiedMockTaskData.Insert(index, itemToModify);                      
                  }                
              });

            mockContext.Setup(m => m.SaveChanges())
              .Callback(() =>
              {                  
                  if(failSave)
                  {
                      throw new DbUpdateConcurrencyException();
                  }
                  mockTaskData = modifiedMockTaskData.AsQueryable<TaskModelModule>();
              });


            var mockedController = new TaskModelModulesController(mockContext.Object as TaskManagerEntities);            
            if(!mockedModelState)
            {
                mockedController.ModelState.AddModelError("Test Error Key", "Test Model Invalid");
            }
            return mockedController;
        }

        [TestMethod]
        public void VerifyTaskManagerContext()
        {
            var mockset = new Mock<DbSet<TaskModelModule>>();

            var mockContext = new Mock<TaskManagerEntities>();
            mockContext.Setup(m => m.TaskModelModules).Returns(mockset.Object);

            var service = new TaskModelModulesController(mockContext.Object);
            var tasks = service.GetTaskModelModules();
            Assert.IsNotNull(tasks);

        }

        [TestMethod]
        public void VerifyTaskManager_GetTaskModelModules()
        {
            var mockedTaskData = GetMockedTaskModelData().ToList();
            var service = GetMockedTaskManagerService();
            var tasks = service.GetTaskModelModules().ToList();
            Assert.IsNotNull(tasks);
            Assert.AreEqual(mockedTaskData.Count(), tasks.Count());
            int i = 0;
            tasks.ForEach(serviceData =>
            {
                Assert.AreEqual(mockedTaskData[i].TaskId, serviceData.TaskId);
                Assert.AreEqual(mockedTaskData[i].TaskDescripton, serviceData.TaskDescripton);
                Assert.AreEqual(mockedTaskData[i].Priority, serviceData.Priority);
                Assert.AreEqual(mockedTaskData[i].StartDate, serviceData.StartDate);
                Assert.AreEqual(mockedTaskData[i].EndDate, serviceData.EndDate);
                Assert.AreEqual(mockedTaskData[i].ParentTaskModelModule.ParentTaskId, serviceData.ParentTaskModelModule.ParentTaskId);
                Assert.AreEqual(mockedTaskData[i++].ParentTaskModelModule.ParentTaskName, serviceData.ParentTaskModelModule.ParentTaskName);
            });

        }

        [TestMethod]
        public void VerifyTaskManager_GetTaskModelById_OKContent()
        {
            int taskId = 1;
            var mockedTask = GetMockedTaskModelData().ToList().FirstOrDefault(x => x.TaskId == taskId);
            var service = GetMockedTaskManagerService();
            IHttpActionResult httpData = service.GetTaskModelModule(taskId);
            Assert.IsNotNull(httpData);
            //var serviceData = httpData as JsonResult<TaskModelModule>;
            var serviceData = (httpData as OkNegotiatedContentResult<TaskModelModule>);
            Assert.IsNotNull(serviceData);
            Assert.IsNotNull(serviceData.Content);
            var taskDataFromService = serviceData.Content;
            Assert.AreEqual(mockedTask.TaskId, taskDataFromService.TaskId);
            Assert.AreEqual(mockedTask.TaskDescripton, taskDataFromService.TaskDescripton);
            Assert.AreEqual(mockedTask.Priority, taskDataFromService.Priority);
            Assert.AreEqual(mockedTask.StartDate, taskDataFromService.StartDate);
            Assert.AreEqual(mockedTask.EndDate, taskDataFromService.EndDate);
            Assert.AreEqual(mockedTask.ParentTaskModelModule.ParentTaskId, taskDataFromService.ParentTaskModelModule.ParentTaskId);
            Assert.AreEqual(mockedTask.ParentTaskModelModule.ParentTaskName, taskDataFromService.ParentTaskModelModule.ParentTaskName);
        }

        [TestMethod]
        public void VerifyTaskManager_GetTaskModelById_NotFoundContent()
        {
            int taskId = 35;            
            var service = GetMockedTaskManagerService();
            IHttpActionResult httpData = service.GetTaskModelModule(taskId);
            Assert.IsNotNull(httpData);            
            var serviceData = httpData as NotFoundResult;
            Assert.IsNotNull(serviceData);            
        }

        [TestMethod]
        public void VerifyTaskManager_PostTask()
        {
            var service = GetMockedTaskManagerService();
            
            var taskToAdd = GetMockedTaskModelData().First();            
            taskToAdd.TaskDescripton = "Task 25";
            taskToAdd.TaskId = 0;
            taskToAdd.Priority = 30;
            taskToAdd.ParentTaskModelModule.ParentTaskId = 255;
            taskToAdd.ParentTaskModelModule.ParentTaskName = "Parent Task 25";

            service.PostTaskModelModule(taskToAdd);

            var tasks  = service.GetTaskModelModules().ToList();
            Assert.IsNotNull(tasks.FirstOrDefault(x => x.TaskId == taskToAdd.TaskId));
            Assert.AreEqual(taskToAdd.TaskId, tasks.FirstOrDefault(x => x.TaskId == taskToAdd.TaskId).TaskId);


        }

        [TestMethod]
        public void VerifyTaskManager_PostTask_InvalidState()
        {
            var service = GetMockedTaskManagerService(false);
            var taskToAdd = GetMockedTaskModelData().First();
            taskToAdd.TaskId = 0;
            taskToAdd.TaskDescripton = "Task 25";
            taskToAdd.Priority = 30;
            taskToAdd.ParentTaskModelModule.ParentTaskId = 255;
            taskToAdd.ParentTaskModelModule.ParentTaskName = "Parent Task 25";

            IHttpActionResult httpData = service.PostTaskModelModule(taskToAdd);
            Assert.IsNotNull(httpData);
            var serviceData = httpData as InvalidModelStateResult;
            Assert.IsNotNull(serviceData);            
            var tasks = service.GetTaskModelModules().ToList();
            Assert.IsNull(tasks.FirstOrDefault(x => x.TaskId == taskToAdd.TaskId));           
        }

        [TestMethod]
        public void VerifyTaskManager_PutTask()
        {
            var service = GetMockedTaskManagerService();

            var taskToModify = GetMockedTaskModelData().ToList()[1];
            taskToModify.TaskId = 2;
            taskToModify.TaskDescripton = "Task 2 Modifed";
            taskToModify.Priority = 20;
            taskToModify.ParentTaskModelModule.ParentTaskId = 255;
            taskToModify.ParentTaskModelModule.ParentTaskName = "Parent Task 2 Modified";

            service.PutTaskModelModule(taskToModify.TaskId, taskToModify);

            var tasks = service.GetTaskModelModules().ToList();
            Assert.IsNotNull(tasks.FirstOrDefault(x => x.TaskId == taskToModify.TaskId));
            Assert.AreEqual(taskToModify.TaskDescripton, tasks.FirstOrDefault(x => x.TaskId == taskToModify.TaskId).TaskDescripton);
            Assert.AreEqual(taskToModify.ParentTaskModelModule.ParentTaskName, tasks.FirstOrDefault(x => x.TaskId == taskToModify.TaskId).ParentTaskModelModule.ParentTaskName);
        }

        [TestMethod]
        public void VerifyTaskManager_PutTask_InvlaidState()
        {
            var service = GetMockedTaskManagerService(false);

            var taskToModify = GetMockedTaskModelData().ToList()[1];
            taskToModify.TaskId = 2;
            taskToModify.TaskDescripton = "Task 2 Modifed";
            taskToModify.Priority = 20;
            taskToModify.ParentTaskModelModule.ParentTaskId = 255;
            taskToModify.ParentTaskModelModule.ParentTaskName = "Parent Task 2 Modified";

            IHttpActionResult httpData = service.PutTaskModelModule(taskToModify.TaskId, taskToModify);
            Assert.IsNotNull(httpData);
            var serviceData = httpData as InvalidModelStateResult;
            Assert.IsNotNull(serviceData);

            var tasks = service.GetTaskModelModules().ToList();
            Assert.IsNotNull(tasks.FirstOrDefault(x => x.TaskId == taskToModify.TaskId));
            Assert.AreNotSame(taskToModify.TaskDescripton, tasks.FirstOrDefault(x => x.TaskId == taskToModify.TaskId).TaskDescripton);
            Assert.AreNotSame(taskToModify.ParentTaskModelModule.ParentTaskName, tasks.FirstOrDefault(x => x.TaskId == taskToModify.TaskId).ParentTaskModelModule.ParentTaskName);
        }

        [TestMethod]
        public void VerifyTaskManager_PutTask_BadRequest()
        {
            var service = GetMockedTaskManagerService();

            var taskToModify = GetMockedTaskModelData().ToList()[1];
            taskToModify.TaskId = 2;
            taskToModify.TaskDescripton = "Task 2 Modifed";
            taskToModify.Priority = 20;
            taskToModify.ParentTaskModelModule.ParentTaskId = 255;
            taskToModify.ParentTaskModelModule.ParentTaskName = "Parent Task 2 Modified";

            IHttpActionResult httpData = service.PutTaskModelModule(1, taskToModify);
            Assert.IsNotNull(httpData);
            var serviceData = httpData as BadRequestResult;
            Assert.IsNotNull(serviceData);

            var tasks = service.GetTaskModelModules().ToList();
            Assert.IsNotNull(tasks.FirstOrDefault(x => x.TaskId == taskToModify.TaskId));
            Assert.AreNotSame(taskToModify.TaskDescripton, tasks.FirstOrDefault(x => x.TaskId == taskToModify.TaskId).TaskDescripton);
            Assert.AreNotSame(taskToModify.ParentTaskModelModule.ParentTaskName, tasks.FirstOrDefault(x => x.TaskId == taskToModify.TaskId).ParentTaskModelModule.ParentTaskName);
        }

        [TestMethod]
        public void VerifyTaskManager_PutTask_DBException()
        {
            var service = GetMockedTaskManagerService(failSave: true);

            var taskToModify = GetMockedTaskModelData().ToList()[1];
            taskToModify.TaskId = 2;
            taskToModify.TaskDescripton = "Task 2 Modifed";
            taskToModify.Priority = 20;
            taskToModify.ParentTaskModelModule.ParentTaskId = 255;
            taskToModify.ParentTaskModelModule.ParentTaskName = "Parent Task 2 Modified";

            try
            {
                IHttpActionResult httpData = service.PutTaskModelModule(2, taskToModify);
            }
            catch(DbUpdateConcurrencyException ex)
            {
                Assert.IsNotNull(ex);
            }               
        }

        [TestMethod]
        public void VerifyTaskManager_DeleteTask()
        {
            var service = GetMockedTaskManagerService();

            var taskToModify = GetMockedTaskModelData().ToList()[1];          
            service.DeleteTaskModelModule(taskToModify.TaskId);

            var tasks = service.GetTaskModelModules().ToList();
            Assert.IsNull(tasks.FirstOrDefault(x => x.TaskId == taskToModify.TaskId));            
        }

        [TestMethod]
        public void VerifyTaskManager_DeleteTask_NotFoundContent()
        {
            int taskId = 35;
            var service = GetMockedTaskManagerService();
            IHttpActionResult httpData = service.DeleteTaskModelModule(205);
            Assert.IsNotNull(httpData);
            var serviceData = httpData as NotFoundResult;
            Assert.IsNotNull(serviceData);
        }
    }  

}
