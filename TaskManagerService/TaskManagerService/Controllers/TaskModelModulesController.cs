using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using TaskManagerData;

namespace TaskManagerService.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class TaskModelModulesController : ApiController
    {
        private TaskManagerEntities db = new TaskManagerEntities();

        public TaskModelModulesController()
        {
        }

        public TaskModelModulesController(TaskManagerEntities et)
        {
            db = et;
        }

        // GET: api/TaskModelModules
        public IQueryable<TaskModelModule> GetTaskModelModules()
        {
            return db.TaskModelModules;
        }

        // GET: api/TaskModelModules/5
        [ResponseType(typeof(TaskModelModule))]
        public IHttpActionResult GetTaskModelModule(int id)
        {
            TaskModelModule taskModelModule = db.TaskModelModules.Find(id);
            if (taskModelModule == null)
            {
                return NotFound();
            }

            return Ok(taskModelModule);
        }

        // PUT: api/TaskModelModules/5
        [HttpPut]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutTaskModelModule(int id, [FromBody] TaskModelModule taskModelModule)
        {            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != taskModelModule.TaskId)
            {
                return BadRequest();
            }

            db.SetModified(taskModelModule);
            db.SetModified(taskModelModule.ParentTaskModelModule);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskModelModuleExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpPost]
        // POST: api/TaskModelModules
        [ResponseType(typeof(TaskModelModule))]
        public IHttpActionResult PostTaskModelModule(TaskModelModule taskModelModule)
        {
            if(taskModelModule.TaskId != 0)
            {
                return PutTaskModelModule(taskModelModule.TaskId, taskModelModule);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.TaskModelModules.Add(taskModelModule);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = taskModelModule.TaskId }, taskModelModule);
        }

        // DELETE: api/TaskModelModules/5
        [ResponseType(typeof(TaskModelModule))]
        public IHttpActionResult DeleteTaskModelModule(int id)
        {
            TaskModelModule taskModelModule = db.TaskModelModules.Find(id);
            if (taskModelModule == null)
            {
                return NotFound();
            }

            db.TaskModelModules.Remove(taskModelModule);
            db.SaveChanges();

            return Ok(taskModelModule);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TaskModelModuleExists(int id)
        {
            return db.TaskModelModules.Count(e => e.TaskId == id) > 0;
        }       
    }
}