using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBench;
using TaskManagerData;
using TaskManagerService;
using System.Web.Http;
using System.Net.Http;
using TaskManagerService.Controllers;
using System.Data.Entity;
using Moq;

namespace TaskManger.PerfomanceTesting
{
    public class TaskMangerPFTest
    {
        
        private void SetMockedTaskModelData()
        {
            for (var cnt = 0; cnt < 100; cnt++)
            {
                tasks.Add(new TaskModelModule()
                {
                    TaskId = cnt,
                    TaskDescripton = string.Format("Task " + cnt),
                    ParentTaskModelModule =
                    new ParentTaskModelModule()
                    {
                        ParentTaskId = cnt * 100,
                        ParentTaskName = string.Format("Parent Task " + cnt * 100),
                    },
                    StartDate = DateTime.Now.Date,
                    EndDate = DateTime.Now.AddDays(2),
                    IsFinished = false,
                    Priority = 1
                });
            }
        }

        public TaskModelModulesController GetMockedTaskManagerService(bool mockedModelState = true, bool failSave = false)
        {
            var mockset = new Mock<DbSet<TaskModelModule>>();
            var modifiedMockTaskData = new List<TaskModelModule>();
            SetMockedTaskModelData();
            var mockTaskData = tasks.AsQueryable();
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
            var mockContext = new Mock<TaskManagerEntities>();
            mockContext.Setup(m => m.TaskModelModules).Returns(mockset.Object);
            var mockedController = new TaskModelModulesController(mockContext.Object as TaskManagerEntities);
            if (!mockedModelState)
            {
                mockedController.ModelState.AddModelError("Test Error Key", "Test Model Invalid");
            }
            return mockedController;
        }

        private const int AcceptableMinAddThroughput = 50;

        private TaskModelModulesController controller;

        List<TaskModelModule> tasks = new List<TaskModelModule>();

        [PerfSetup]
        public void Setup(BenchmarkContext context)
        {

            controller = GetMockedTaskManagerService();            
        }

        [PerfBenchmark(NumberOfIterations = 5, RunMode = RunMode.Throughput, TestMode = TestMode.Test, SkipWarmups = true)]
        [ElapsedTimeAssertion(MaxTimeMilliseconds = 10000, MinTimeMilliseconds = 1000)]
        public void AddTask_Throughput_IterationMode(BenchmarkContext context)
        {
            TaskModelModule msg;
            for (var i = 0; i < tasks.Count; i++)
            {
                controller.PostTaskModelModule(tasks[i]);
            }
        }

        [PerfBenchmark(NumberOfIterations = 1, RunMode = RunMode.Iterations, TestMode = TestMode.Test, SkipWarmups = true)]
        [ElapsedTimeAssertion(MaxTimeMilliseconds = 10000, MinTimeMilliseconds = 1000)]
        public void GetAllTask_Throughput_IterationMode(BenchmarkContext context)
        {
            IEnumerable<TaskModelModule> msg;
            for (var i = 0; i < AcceptableMinAddThroughput; i++)
            {                
                msg = controller.GetTaskModelModules();
            }
        }

        [PerfBenchmark(NumberOfIterations = 1, RunMode = RunMode.Throughput, TestMode = TestMode.Test, SkipWarmups = true)]
        [ElapsedTimeAssertion(MaxTimeMilliseconds = 10000, MinTimeMilliseconds = 1000)]
        public void GetTaskById_Throughput_IterationMode(BenchmarkContext context)
        {            
            for (var i = 0; i < AcceptableMinAddThroughput; i++)
            {
                var res = controller.GetTaskModelModule(1);
            }
        }

        [PerfCleanup]
        public void Cleanup(BenchmarkContext context)
        {

        }

    }
}
