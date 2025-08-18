import React from 'react'
import Rootlayout from './layout/rootlayout'
import Dashboard from './pages/AsideLinks/Core/Dashboard'
import Organization from './pages/AsideLinks/Core/Organization'
import Employees from './pages/AsideLinks/Core/Employees'
import Attendance from './pages/AsideLinks/Core/Attendance'
import Recruitment from './pages/AsideLinks/Talent/Recruitment'
import Performance from  './pages/AsideLinks/Talent/Performance'
import Training from './pages/AsideLinks/Talent/Training'
import LeaveManegment from './pages/AsideLinks/Core/LeaveManagment'
import { createBrowserRouter, createRoutesFromElements, Route, RouterProvider } from 'react-router-dom'
import Assets from './pages/AsideLinks/Operation/Assets'
import Announcement from './pages/AsideLinks/Operation/Announcement'
import AddNewemployee from './pages/EmployeeRegistration/addNewemployee'
import AddNewemployeesecond from './pages/EmployeeRegistration/AddNewemployeesecond'
import Compensation from './pages/EmployeeRegistration/Compensation'
import System from './pages/EmployeeRegistration/System'
import AllOrganization from './pages/SuperAdmin/Organization/AllOrganization'
import CreateOrganization from './pages/SuperAdmin/Organization/CreateOrganization'
import OrganizationSetting from './pages/SuperAdmin/Organization/OrganizationSettings'
import SuperAdmin from './pages/SuperAdmin/UserManegment/SuperAdmin'
import UserStatics from './pages/SuperAdmin/Report/UserStatics'
import LoginPage from './components/LoginPage';
import ProtectedRoute from './Components/ProtectedRoute';


const App = () => {
  const router = createBrowserRouter(
      createRoutesFromElements(
        <>
           {/* Public Routes */}
           <Route path="/login" element={<LoginPage />} />
           {/* Protected Routes */}
              <Route element={<ProtectedRoute allowedRoles={["Admin", "SuperAdmin"]} />}>

              <Route path='/' element = {<Rootlayout/>}>

                <Route index element = {<Dashboard/>} />
                <Route path='organization' element = {<Organization/>}/>
                <Route path='employees' element = {<Employees/>}/>
                <Route path='attendance' element = {<Attendance/>}/>
                <Route path='leaveManagment' element = {<LeaveManegment/>}/>
        
                {/* AddNewEmployee */}
                <Route path='AddNewemployee' element = {<AddNewemployee/>}/>
                <Route path='AddNewemployeesecond' element = {<AddNewemployeesecond/>}/>
                <Route path='Compensation' element = {<Compensation/>}/>
                <Route path='System' element = {<System/>}/>


                {/* TALENT */}
                <Route path='recruitment' element = {<Recruitment/>}/>
                <Route path='performance' element = {<Performance/>}/>
                <Route path='training' element = {<Training/>}/>

                {/* OPERATION */}
                <Route path='assets' element = {<Assets/>}/>
                <Route path='announcement' element = {<Announcement/>}/>

                {/* SuperAdmin */}
                <Route path='allorganization' element = {<AllOrganization/>}/>
                <Route path='createorganization' element = {<CreateOrganization/>}/>
                <Route path='organizationsetting' element = {<OrganizationSetting/>}/>

                

                {/* REPORT */}
                 <Route path='userstatics' element={<UserStatics />} />
              <Route element={<ProtectedRoute allowedRoles={["SuperAdmin"]} />}>
                 {/* User Managment */}
                 <Route path='superadmin' element={<SuperAdmin />} />
              </Route>
              </Route>
              </Route>
          </>
    )
  )

  return (
      <RouterProvider router={router} />
  )
}

export default App