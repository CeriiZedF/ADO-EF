using System;
using System.Data;

namespace ADO_EF.Data.Entity
{
    public class Department
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;


        //Soft delete
        //public DateTime? DeleteDt { get; set; }

        //Inverse Navigation props
        //Main - зворотна до Manager.MainDep властивість 
        public IEnumerable<Manager> MainManagers { get; set; }
        public List<Manager> SecManagers { get; set; }

    }
}
