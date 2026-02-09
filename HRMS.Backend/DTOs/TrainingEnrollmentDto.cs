using System;
using System.ComponentModel.DataAnnotations;


namespace HRMS.Backend.DTOs
{
    public class TrainingEnrollmentDto
    {
        public Guid Id { get; set; }

        public Guid ProgramId { get; set; }

        public Guid EmployeeId { get; set; }

        public Guid TenantId { get; set; }

        public Guid? OrganizationId { get; set; }

        public string? EnrollmentNote { get; set; }

        public DateTime EnrolledAt { get; set; }
    }

    public class CreateTrainingEnrollmentDto
    {
        public Guid ProgramId { get; set; }
        public string EmployeeEmail { get; set; } = null!;
        public Guid TenantId { get; set; }
        public Guid? OrganizationId { get; set; }
        public string? EnrollmentNote { get; set; }

        // New notification flags
        public bool ManagerNotify { get; set; } = false;
        public bool EmployeeNotify { get; set; } = false;
    }



    public class UpdateEnrollmentProgressByUserDto
    {
        public Guid UserId { get; set; }
        public Guid ProgramId { get; set; }
        public int Progress { get; set; } // 0 – 100
    }


    public class CreateTrainingEnrollmentByUserDto
    {
        public Guid ProgramId { get; set; }   // Program to enroll in
        public Guid UserId { get; set; }      // UserId to find Employee
    }

}
