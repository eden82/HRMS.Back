import React from 'react'

import { Outlet,useLocation } from 'react-router-dom'
import MainBody from '../Components/mainBody'
import Header from '../Components/Header'
import LoginPage from '../Components/LoginPage'
import SuperAdminBody from '../Components/SuperAdminBody'

const RootLayout = () => {
    const location = useLocation()
    
    const path = location.pathname

    const readPath = path === '/' ? 'Dashboard' : path.replace('/', '')
    // Get role from localStorage (or decode JWT)
    const role = localStorage.getItem("userRole") // make sure you save this at login

    return (
        <div className='flex gap-[4.4375rem]'>
            {role === "SuperAdmin" ? (
                <SuperAdminBody readPath={readPath} />
            ) : (
                <MainBody readPath={readPath} />
            )}

            <div className='flex flex-col flex-1 gap-[4.25rem]'>
                <div className='flex pt-[3.5rem]'>
                    <Header readPath={readPath} />
                </div>
                <div className='w-[calc(100%-2.75rem)]'>
                    <Outlet />
                </div>
            </div>
        </div>
    )
}

export default RootLayout
