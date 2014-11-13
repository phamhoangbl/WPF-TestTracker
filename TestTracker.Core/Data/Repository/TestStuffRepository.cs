using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestTracker.Core.Data.Model;

namespace TestTracker.Core.Data.Repository
{
    public class TestStuffRepository : ITestStuffRepository
    {
        private TestTrackerContext db = null;

         public TestStuffRepository()
         {
             this.db = new TestTrackerContext();
         }

         public TestStuffRepository(TestTrackerContext db)
         {
             this.db = db;
         }

         public IEnumerable<TestStuff> SelectAll()
         {
             return db.TestStuffs.ToList();
         }

         public TestStuff SelectByID(int id)
         {
             return db.TestStuffs.Find(id);
         }

         public TestStuff Select(string deviceId, string verdorId, string port)
         {
             return db.TestStuffs.SingleOrDefault(x => x.DeviceId == deviceId && x.VerdorId == verdorId && x.Port == port); 
         }

         public void Insert(TestStuff obj, out int testStuffId)
         {
             var a = db.TestStuffs.Add(obj);
             db.SaveChanges();
             testStuffId = a.TestStuffId;
         }

         public void Update(TestStuff obj)
         {
             db.Entry(obj).State = EntityState.Modified;
         }

         public void Delete(string id)
         {
             TestStuff existing = db.TestStuffs.Find(id);
             db.TestStuffs.Remove(existing);
         }

         public void Save()
         {
             db.SaveChanges();
         }
    }
}
