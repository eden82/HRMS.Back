import React, {useState} from 'react'


export default function AddAdmin({onClose}) {
    const [dropdownOpenR, setDropdownOpenR] = useState(false);
    const [selectedRole, setSelectedRole] = useState('Employee')
    const Role = ['Employee','Admin','HR','CTO']
  return (
    <div className='px-[3rem] pt-[2.875rem] space-y-[3.125rem] font-semibold w-full'>
        <div className='flex justify-between'>
            <div className=''>
                <h1 className='textFormColor'>Add Super Administrator</h1>
                <h4 className='text-limegray'>Create a new super administrator account</h4>
            </div>
            <button onClick={onClose} className='rounded-full center-center cursor-pointer'>
                <svg width="26" height="26" viewBox="0 0 26 26" fill="none" xmlns="http://www.w3.org/2000/svg">
        <path fill-rule="evenodd" clip-rule="evenodd" d="M25.5 13C25.5 19.9035 19.9035 25.5 13 25.5C6.09644 25.5 0.5 19.9035 0.5 13C0.5 6.09644 6.09644 0.5 13 0.5C19.9035 0.5 25.5 6.09644 25.5 13ZM9.21204 9.21206C9.57815 8.84595 10.1717 8.84595 10.5379 9.21206L13 11.6741L15.462 9.21209C15.8281 8.84597 16.4218 8.84597 16.7879 9.21209C17.154 9.5782 17.154 10.1718 16.7879 10.5379L14.3258 13L16.7879 15.462C17.154 15.8281 17.154 16.4218 16.7879 16.7879C16.4218 17.154 15.8281 17.154 15.462 16.7879L13 14.3259L10.5379 16.7879C10.1718 17.154 9.57818 17.154 9.21206 16.7879C8.84595 16.4218 8.84595 15.8281 9.21206 15.4621L11.6741 13L9.21204 10.5379C8.84591 10.1718 8.84591 9.57818 9.21204 9.21206Z" fill="#BEE532"/>
                </svg>
            </button>
        </div>
        {/* FormSection */}
        <div className=''>
            <form action="" className='space-y-[2.6875rem]'>
                {/* formContainer */}
                <div className='space-y-[1.9375rem]'>
                    <div className='flex flex-col gap-[1rem]'>
                        <label htmlFor="fullName" className='text-formColor'>Name</label>
                        <input id='fullName' type="text" placeholder='ex. John Don' className='inputMod'/>
                    </div>
                    <div className='flex flex-col gap-[1rem]'>
                        <label htmlFor="emailAdd" className='text-formColor'>Email</label>
                        <input id='emailAdd' type="email" placeholder='ex. example@Gmail.com' className='inputMod'/>
                    </div>
                    <div className='space-y-[2.125rem]'>      
                        <div className='flex flex-col gap-[1rem]'>
                            <label htmlFor="phoneNumber" className='text-formColor'>Phone</label>
                            <input id='phoneNumber' type="text" placeholder='ex. example@Gmail.com' className='inputMod'/>
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
                        <div className='flex flex-col gap-[1rem]'>
                            <label htmlFor="password" className='text-formColor'>Default password</label>
                            <input id='password' type="password" placeholder='admin1234' className='inputMod'/>
                        </div>
                    </div>
                </div>
                <div className='w-full h-[3.4375rem]'>
                    <button type="submit" onClick={()=>navigate('/')} className='w-full h-full bg-lemongreen rounded-[10px] cursor-pointer'>Add Administrator</button>
                </div>
            </form>
        </div>
    </div>
  )
}
