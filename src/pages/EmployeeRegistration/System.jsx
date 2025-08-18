import React , {useState} from 'react'
import { useNavigate } from 'react-router-dom'

const System = () => {
    const navigate = useNavigate()
    const [dropdownOpenR, setDropdownOpenR] = useState(false);
    const [selectedRole, setSelectedRole] = useState('Employee')
    const Role = ['Employee','Admin','HR','CTO']
    const [dropdownOpenS, setDropdownOpenS] = useState(false);
    const [selectedShift, setSelectedShift] = useState('Morning Shift')
    const Shift = ['Morning Shift','Night Shift']
    const [dropdownOpenW, setDropdownOpenW] = useState(false);
    const [selectedWork, setSelectedWork] = useState('Office')
    const Work = ['Office','Home']
  return (
    <div className='font-semibold flex flex-col gap-[4rem]'>
        <div className='flex flex-col gap-[2.5rem]'>
            {/* Header */}
            <div className='flex items-center gap-[0.9375rem]'>
                <svg onClick={()=>navigate('/Compensation')} className='cursor-pointer' width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
        <path d="M20 12H4M4 12L10 6M4 12L10 18" stroke="#BEE532" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
                </svg>
                <li className='textWhite list-none'>4. System Access & Work Details</li>
            </div>

            {/* ProgressBar */}
            <div>
                <div className='grid grid-cols-4'>
                    <div className='rounded-[0.53125rem] bg-lemongreen w-[22.625rem] h-[5px] '></div>
                    <div className='rounded-[0.53125rem] bg-lemongreen w-[22.625rem] h-[5px] '></div>
                    <div className='rounded-[0.53125rem] bg-lemongreen w-[22.625rem] h-[5px] '></div>
                    <div className='rounded-[0.53125rem] bg-lemongreen w-[22.625rem] h-[5px] '></div>
                </div>
            </div>
        </div>
        <div className='between gap-[12.25rem]'> 
            <div className='w-[49.5625rem] h-[36.3125rem] overflow-y-auto scrollBarDash'>
                <form action="" className='flex gap-[2.5625rem] px-[10px]'>
                    <div className='flex flex-col w-[23.1875rem] gap-[35px]'>
                        {/* username */}
                        <div className='flex flex-col gap-[1rem]'>
                            <label htmlFor="username" className='text-formColor'>Username</label>
                            <input type="text" id='username' placeholder='e.x bereketdan' className='inputMod'/>
                        </div>
                        {/* Role */}
                        <div className='flex flex-col gap-[1rem] relative'>
                            <label htmlFor="firstName" className='text-formColor'>Role</label>
                            <div className='inputMod flex items-center justify-between pr-[1.5625rem]' >
                                <button type="button" className='text-formColor text-left' onClick={() => setDropdownOpenR(!dropdownOpenR)}>{selectedRole}</button>
                                <svg onClick={() => setDropdownOpenR(!dropdownOpenR)} className={`transition-transform duration-200 ${dropdownOpenR ? 'rotate-180' : ''}`} width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path d="M19 9L12 15L5 9" stroke="#BEE532" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
                                </svg>
                            </div>
                            <div className={`${dropdownOpenR ? 'flex' : 'hidden'} bg-inputBack rounded-[10px] h-[11.25rem]  w-full top-[6.3125rem] absolute z-10 text-formColor flex-col center-center border border-limeLight scrollBarDash overflow-y-auto `}>
                                <ul className='flex flex-col  gap-[2.1875rem] h-full pt-[1.3125rem]'>
                                    {Role.map(role => (
                                    <li key={role} className={`cursor-pointer w-full text-center ${selectedRole === role ? 'text-lemongreen font-bold' : ''}`}
                                    onClick={() => {
                                        setSelectedRole(role)
                                        setDropdownOpenR(false)
                                    }}>
                                    {role}
                                    </li>
                                ))}
                                </ul>
                            </div>
                        </div>
                        {/* Shift */}
                        <div className='flex flex-col gap-[1rem] relative'>
                            <label htmlFor="firstName" className='text-formColor'>Shift Details</label>
                            <div className='inputMod flex items-center justify-between pr-[1.5625rem]' >
                                <button type="button" className='text-formColor text-left' onClick={() => setDropdownOpenS(!dropdownOpenS)}>{selectedShift}</button>
                                <svg onClick={() => setDropdownOpenS(!dropdownOpenS)} className={`transition-transform duration-200 ${dropdownOpenS ? 'rotate-180' : ''}`} width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path d="M19 9L12 15L5 9" stroke="#BEE532" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
                                </svg>
                            </div>
                            <div className={`${dropdownOpenS ? 'flex' : 'hidden'} bg-inputBack rounded-[10px] w-full top-[6.3125rem] absolute z-10 text-formColor flex-col center-center border border-limeLight scrollBarDash overflow-y-auto `}>
                                <ul className='flex flex-col  gap-[2.1875rem] py-[1.3125rem]'>
                                    {Shift.map(shift => (
                                    <li key={shift} className={`cursor-pointer w-full text-center ${selectedShift === shift ? 'text-lemongreen font-bold' : ''}`}
                                    onClick={() => {
                                        setSelectedShift(shift)
                                        setDropdownOpenS(false)
                                    }}>
                                    {shift}
                                    </li>
                                ))}
                                </ul>
                            </div>
                        </div>
                    </div>
                    <div className='w-[23.1875rem] flex flex-col gap-[35px]'>
                        {/* password */}
                        <div className='flex flex-col gap-[1rem]'>
                            <label htmlFor="password" className='text-formColor'>Password</label>
                            <input id='password' type="password" placeholder='*************' className='inputMod'/>
                        </div>
                        {/* Work Location */}
                        <div className='flex flex-col gap-[1rem] relative'>
                            <label htmlFor="firstName" className='text-formColor'>Work Location</label>
                            <div className='inputMod flex items-center justify-between pr-[1.5625rem]' >
                                <button type="button" className='text-formColor text-left' onClick={() => setDropdownOpenW(!dropdownOpenW)}>{selectedWork}</button>
                                <svg onClick={() => setDropdownOpenW(!dropdownOpenW)} className={`transition-transform duration-200 ${dropdownOpenW ? 'rotate-180' : ''}`} width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path d="M19 9L12 15L5 9" stroke="#BEE532" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
                                </svg>
                            </div>
                            <div className={`${dropdownOpenW ? 'flex' : 'hidden'} bg-inputBack rounded-[10px] w-full top-[6.3125rem] absolute z-10 text-formColor flex-col center-center border border-limeLight scrollBarDash overflow-y-auto `}>
                                <ul className='flex flex-col  gap-[2.1875rem] py-[1.3125rem]'>
                                    {Work.map(work => (
                                    <li key={work} className={`cursor-pointer w-full text-center ${selectedWork === work ? 'text-lemongreen font-bold' : ''}`}
                                    onClick={() => {
                                        setSelectedWork(work)
                                        setDropdownOpenW(false)
                                    }}>
                                    {work}
                                    </li>
                                ))}
                                </ul>
                            </div>
                        </div>
                        {/* Certefication */}
                        <div className='flex flex-col gap-[1rem]'>
                            <label htmlFor="Certefication" className='text-formColor'>Certefication</label>
                            <label htmlFor="Certefication" className='inputModfile cursor-pointer' >
                                <svg width="22" height="22" viewBox="0 0 22 22" fill="none" xmlns="http://www.w3.org/2000/svg">
    <path d="M12.8337 1.83301V5.49967C12.8337 5.9859 13.0268 6.45222 13.3706 6.79604C13.7144 7.13985 14.1808 7.33301 14.667 7.33301H18.3337M9.16699 8.24967H7.33366M14.667 11.9163H7.33366M14.667 15.583H7.33366M13.7503 1.83301H5.50033C5.0141 1.83301 4.54778 2.02616 4.20396 2.36998C3.86015 2.7138 3.66699 3.18011 3.66699 3.66634V18.333C3.66699 18.8192 3.86015 19.2856 4.20396 19.6294C4.54778 19.9732 5.0141 20.1663 5.50033 20.1663H16.5003C16.9866 20.1663 17.4529 19.9732 17.7967 19.6294C18.1405 19.2856 18.3337 18.8192 18.3337 18.333V6.41634L13.7503 1.83301Z" stroke="#BEE532" stroke-width="1.375" stroke-linecap="round" stroke-linejoin="round"/>
                                </svg>
                                <span className='text-limeLight'>Upload Certefication</span>
                            </label>
                            <input type="file" id='Certefication' className='hidden'/>
                        </div>
                    </div>
                </form>
                <div className='w-full h-[3.4375rem] my-[4rem] px-[10px]  flex gap-[2.5625rem]'>
                    <button type="button" onClick={()=>navigate('/Compensation')} className='w-[23.1875rem] border border-formColor text-formColor rounded-[10px] cursor-pointer'>Back</button>
                    <button type="submit" onClick={()=>navigate('/')} className='w-[23.1875rem] bg-lemongreen rounded-[10px] cursor-pointer'>Complete</button>
                </div>
            </div>
            <div className='flex-1'>
                <div className='border border-limegray w-[31rem]  rounded-[1.1875rem] px-[2.25rem] pt-[1.5625rem] pb-[1.9375rem]'>
                    <div className='flex items-center gap-[10px] pb-[0.8125rem]'>
                        <svg width="22" height="22" viewBox="0 0 22 22" fill="none" xmlns="http://www.w3.org/2000/svg">
            <path d="M11.0001 8.25004V11.9167M11.0001 15.5834H11.0101M19.9192 16.5L12.5859 3.66671C12.426 3.38456 12.1941 3.14988 11.9139 2.98661C11.6337 2.82333 11.3152 2.7373 10.9909 2.7373C10.6666 2.7373 10.3481 2.82333 10.0679 2.98661C9.78767 3.14988 9.55579 3.38456 9.39589 3.66671L2.06256 16.5C1.90093 16.78 1.81618 17.0976 1.8169 17.4208C1.81761 17.7441 1.90377 18.0614 2.06663 18.3406C2.2295 18.6197 2.46328 18.8509 2.74428 19.0106C3.02528 19.1704 3.34351 19.253 3.66672 19.25H18.3334C18.655 19.2497 18.971 19.1648 19.2494 19.0037C19.5278 18.8427 19.759 18.6112 19.9197 18.3326C20.0804 18.0539 20.1649 17.7379 20.1648 17.4162C20.1648 17.0946 20.0801 16.7786 19.9192 16.5Z" stroke="#DFDFDF" stroke-width="1.375" stroke-linecap="round" stroke-linejoin="round"/>
                        </svg>
                        <span className='textFormColor'><strong>Important:</strong></span>
                    </div>
                    <div className='space-y-[2.25rem]'>
                        <p className='textLimegray'>Provide accurate information about your current employment status, including your job title, employer's name, and contact information. This helps establish your professional background and may be necessary for verification or eligibility purposes.</p>
                        <p className='textLimegray'><strong className='text-formColor'>Tip:</strong> Make sure to list your employer's official name and provide a valid work email or phone number if requested.</p>
                    </div>
                </div>           
            </div>
        </div>
    </div>
  )
}

export default System