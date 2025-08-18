import React from 'react'

const Header = ({ readPath }) => {
  // Object with code-friendly keys
  const parag = {
    Dashboard: "Welcome back! Here's what's happening at your organization today.",
    Organization: "Manage company structure, departments, and organizational hierarchy.",
    Employee: "Manage employee profile, roles, and organizational structure.",
    Attendance: "Monitor employee attendance, working hours, and presence status.",
    Leave_Management: "Manage employee leave requests, balances, and policies.",
    Recruitment: "",
    Performance:"",
    Training:"",
    Announcement:"",
    Assets:"",
    Add_New_Employee:"Enter employee details to create a new profile",
    All_Organizations: 'Manage all organizations in your HRMS platform',
    Create_Organization: 'Set up a new organization in your HRMS platform',
    Organization_Settings: 'Set up a new organization in your HRMS platform',
    Super_Administrators: 'Manage system administrators and their permissions',
    User_Statics: ''

    }

    const handleLogout = () => {
        localStorage.removeItem("jwtToken");
        window.location.href = "/login";
        // or use `useNavigate()` from react-router
    };

    return (
        <header>
            <button onClick={handleLogout}>Logout</button>
        </header>
    );

  // Map human-readable titles to object keys
  const keyMap = {
    "Dashboard": "Dashboard",
    "organization": "Organization",
    "employees": "Employee",
    "attendance": "Attendance",
    "leaveManagment": "Leave_Management",
    "recruitment": "Recruitment",
    "performance":"Performance",
    "training":"Training",
    "announcement":"Announcement",
    "assets":"Assets",
    "AddNewemployee": 'Add_New_Employee',
    "AddNewemployeesecond": 'Add_New_Employee',
    "Compensation": "Add_New_Employee",
    "System": "Add_New_Employee",
    "allorganization": 'All_Organizations',
    "createorganization": "Create_Organization",
    "organizationsetting":'Organization_Settings',
    'superadmin':'Super_Administrators',
    'userstatics': 'User_Statics'
  }

  // Safely access the right key
  const description = parag[keyMap[readPath]] || ""

  return (
    <div className='flex items-center w-full'>
      <header className='w-[calc(100%-3.0625rem)] flex items-center justify-between'>
        <div className='flex flex-col  leading-none space-y-[0.4375rem]'>
          <h1 className='text-white text-[2rem] font-semibold'>{keyMap[readPath].replace(/_/g, ' ')}</h1>
          <h4 className='text-limegray text-[15px] font-medium'>{description}</h4>
        </div>
        <div className='h-full w-[13.375rem] text-nowrap'>
          <div className='flex items-center justify-between'>
            <div className='w-[138px]'>
              <span className='text-accountColor font-medium'>Welcome, Benjamin</span>
            </div>
            <div className='border rounded-full flex p-[15px] items-center justify-center bg-black'>
              <svg  width="21" height="21" viewBox="0 0 21 21" fill="none" xmlns="http://www.w3.org/2000/svg">
<path d="M10.5 8.75C12.433 8.75 14 7.183 14 5.25C14 3.317 12.433 1.75 10.5 1.75C8.567 1.75 7 3.317 7 5.25C7 7.183 8.567 8.75 10.5 8.75Z" stroke="#BEE532" stroke-width="1.5"/>
<path d="M10.5 18.375C13.8827 18.375 16.625 16.808 16.625 14.875C16.625 12.942 13.8827 11.375 10.5 11.375C7.11726 11.375 4.375 12.942 4.375 14.875C4.375 16.808 7.11726 18.375 10.5 18.375Z" stroke="#BEE532" stroke-width="1.5"/>
              </svg>
            </div>
          </div>
        </div>
      </header>
    </div>
  )
}

export default Header
