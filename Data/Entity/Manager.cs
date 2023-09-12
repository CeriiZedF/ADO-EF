using System;

namespace ADO_EF.Data.Entity
{
    public class Manager
    {
        public Guid Id { get; set; }
        public String Surname { get; set; } = null!;
        public String Name { get; set; } = null!;
        public String Secname { get; set; } = null!;
        public String Login { get; set; } = null!;
        public String PassSalt { get; set; } = null!;  // стандарт rfc2898
        public String PassDk { get; set; } = null!;  // стандарт rfc2898
        public Guid IdMainDep { get; set; }  // отдел в котором работает
        public Guid? IdSecDep { get; set; }  // доп. отдел
        public Guid? IdChief { get; set; }
        public DateTime CreateDt { get; set; }
        public DateTime? DeleteDt { get; set; }
        public String Email { get; set; } = null!;
        public String? Avatar { get; set; }

        //Navigation props

        public Department MainDep { get; set; } //навігаційна властивість

        public Department? SecDep { get; set; } //опціональна властивість

        public Department? ChiefDep { get; set; }

        public IEnumerable<Department> Subordinates { get; set; }
    }
}
