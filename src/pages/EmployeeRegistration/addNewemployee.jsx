import React , {useState} from 'react'
import { useNavigate } from 'react-router-dom';

const addNewemployee = () => {
    const [dropdownOpen, setDropdownOpen] = useState(false);
    const [dropdownOpenG, setDropdownOpenG] = useState(false);
    const [dropdownOpenM, setDropdownOpenM] = useState(false);
    const [selectedCountries, setSelectedCountries] = useState('Ethiopia')
    const [selectedGender, setSelectedGender] = useState('Male')
    const [selectedMartial, setSelectedMartial] = useState('Unmarried')
    const Countries = ['Ethiopia', 'Kenya', 'Nigeria','South Africa','South Africa','South Africa','South Africa']
    const gender = ['Male','Female']
    const martial = ['Married','Divorce','Unmarried']
    const navigate = useNavigate();
    return (
        <div className='font-semibold flex flex-col gap-[4rem]'>
            {/* headerContainer */}
            <div className='flex flex-col gap-[2.5rem]'>
                {/* Header */}
                <div className='flex items-center gap-[0.9375rem]'>
                    <svg onClick={()=>navigate('/employees')} className='cursor-pointer' width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
            <path d="M20 12H4M4 12L10 6M4 12L10 18" stroke="#BEE532" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
                    </svg>
                    <li className='textWhite list-decimal'>Personal Information</li>
                </div>

                {/* ProgressBar */}
                <div>
                    <div className='grid grid-cols-4'>
                    <div className='rounded-[0.53125rem] bg-lemongreen w-[22.625rem] h-[5px] '></div>
                    <div className='rounded-[0.53125rem] bg-[rgba(223,223,223,0.26)] w-[22.625rem] h-[5px] '></div>
                    <div className='rounded-[0.53125rem] bg-[rgba(223,223,223,0.26)] w-[22.625rem] h-[5px] '></div>
                    <div className='rounded-[0.53125rem] bg-[rgba(223,223,223,0.26)] w-[22.625rem] h-[5px] '></div>
                    </div>
                </div>
            </div>

            {/* MainContainer */}
            <div className='between gap-[12.25rem]'>
            {/* mainContent */}
                <div className='w-[49.5625rem] h-[37.3125rem] overflow-y-auto scrollBarDash' > 
                    <form action="" className='flex gap-[2.5625rem] px-[10px]'>
                        <div className=' flex flex-col w-[23.1875rem] gap-[35px]'>
                            {/* firstName */}
                            <div className='flex flex-col gap-[1rem]'>
                                <label htmlFor="firstName" className='text-formColor'>First Name</label>
                                <input type="text" placeholder='John' className='inputMod'/>
                            </div>
                            {/* dateOfbirth */}
                            <div className='flex flex-col gap-[1rem]'>
                                <label htmlFor="firstName" className='text-formColor'>Date Of Birth</label>
                                <input type="date"   className='inputMod pr-[1.5625rem]'/>
                            </div>
                            {/*DropDown */}
                            <div className='flex flex-col gap-[1rem] relative'>
                                <label htmlFor="firstName" className='text-formColor'>Nationality</label>
                                <div className='inputMod flex items-center justify-between pr-[1.5625rem]' >
                                    <button type="button" className='text-formColor text-left' onClick={() => setDropdownOpen(!dropdownOpen)}>{selectedCountries}</button>
                                    <svg onClick={() => setDropdownOpen(!dropdownOpen)} className={`transition-transform duration-200 ${dropdownOpen ? 'rotate-180' : ''}`} width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                    <path d="M19 9L12 15L5 9" stroke="#BEE532" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
                                    </svg>
                                </div>
                                <div className={`${dropdownOpen ? 'flex' : 'hidden'} bg-inputBack rounded-[10px] h-[11.25rem]  w-full top-[6.3125rem] absolute z-10 text-formColor flex-col center-center border border-limeLight scrollBarDash overflow-y-auto `}>
                                    <ul className='flex flex-col  gap-[2.1875rem] h-full pt-[1.3125rem]'>
                                        {Countries.map(Count => (
                                        <li key={Count} className={`cursor-pointer w-full text-center ${selectedCountries === Count ? 'text-lemongreen font-bold' : ''}`}
                                        onClick={() => {
                                            setSelectedCountries(Count)
                                            setDropdownOpen(false)
                                        }}>
                                        {Count}
                                        </li>
                                    ))}
                                    </ul>
                                </div>
                            </div>
                            <div className='flex flex-col gap-[1rem]'>
                                <label htmlFor="firstName" className='text-formColor'>Email</label>
                                <input type="email"  placeholder='example@gmail.com'  className='inputMod  pr-[1.5625rem]'/>
                            </div>
                            <div className='flex flex-col gap-[1rem]'>
                                <label htmlFor="firstName" className='text-formColor'>Address</label>
                                <input type="text"  placeholder='Bole,Addis abeba'  className='inputMod  pr-[1.5625rem]'/>
                            </div>
                        </div>
                        <div className='w-[23.1875rem]'>
                            <div className='flex flex-col w-full gap-[35px]'>
                                <div className='flex flex-col gap-[1rem]'>
                                    <label htmlFor="firstName" className='text-formColor'>Last Name</label>
                                    <input type="text" placeholder='John' className='inputMod'/>
                                </div>
                                {/*DropDown */}
                                <div className='flex flex-col gap-[1rem] relative'>
                                    <label htmlFor="firstName" className='text-formColor'>Gender</label>
                                    <div className='inputMod flex items-center justify-between pr-[1.5625rem]' >
                                        <button type="button" className='text-formColor text-left' onClick={() => setDropdownOpenG(!dropdownOpenG)}>{selectedGender}</button>
                                        <svg onClick={() => setDropdownOpenG(!dropdownOpenG)} className={`transition-transform duration-200 ${dropdownOpenG ? 'rotate-180' : ''}`} width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                        <path d="M19 9L12 15L5 9" stroke="#BEE532" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
                                        </svg>
                                    </div>
                                    <div className={`${dropdownOpenG ? 'flex' : 'hidden'} bg-inputBack rounded-[10px]   w-full top-[6.3125rem] absolute z-10 text-formColor flex-col center-center border border-limeLight`}>
                                        <ul className='flex flex-col space-y-5 py-5 '>
                                            {gender.map(Sex => (
                                            <li key={Sex} className={`cursor-pointer w-full text-center ${selectedGender === Sex ? 'text-lemongreen font-bold' : ''}`}
                                            onClick={() => {
                                                setSelectedGender(Sex)
                                                setDropdownOpenG(false)
                                            }}>
                                            {Sex}
                                            </li>
                                        ))}
                                        </ul>
                                    </div>
                                </div>
                            <div className='flex flex-col gap-[1rem] relative'>
                                <label htmlFor="firstName" className='text-formColor'>Martial</label>
                                <div className='inputMod flex items-center justify-between pr-[1.5625rem]' >
                                <button type="button" className='text-formColor text-left' onClick={() => setDropdownOpenM(!dropdownOpenM)}>{selectedMartial}</button>
                                <svg onClick={() => setDropdownOpenM(!dropdownOpenM)} className={`transition-transform duration-200 ${dropdownOpenM ? 'rotate-180' : ''}`} width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path d="M19 9L12 15L5 9" stroke="#BEE532" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
                                </svg>
                                </div>
                                <div className={`${dropdownOpenM ? 'flex' : 'hidden'} bg-inputBack rounded-[10px]   w-full top-[6.3125rem] absolute z-10 text-formColor flex-col center-center border border-limeLight`}>
                                <ul className='flex flex-col space-y-5 py-5 '>
                                    {martial.map(mart => (
                                    <li key={mart} className={`cursor-pointer w-full text-center ${selectedMartial === mart ? 'text-lemongreen font-bold' : ''}`}
                                    onClick={() => {
                                        setSelectedMartial(mart)
                                        setDropdownOpenM(false)
                                        }}>
                                        {mart}
                                    </li>
                                    ))}
                                </ul>
                                </div>
                            </div>
                            <div className='flex flex-col gap-[1rem]'>
                                <label htmlFor="firstName" className='text-formColor'>Phone Number</label>
                                <input type="text" placeholder='+251987654321' className='inputMod pr-[1.5625rem]'/>
                            </div>
                            <div className='flex flex-col gap-[1rem]'>
                                <label htmlFor="firstName" className='text-formColor'>Emergancy Contant</label>
                                <input type="text" placeholder='+251987654321' className='inputMod pr-[1.5625rem]'/>
                            </div>
                            </div>
                        </div>
                    </form>
                    <div className='w-[calc(100%-0.625rem)] h-[3.4375rem] mt-[2.5625rem] pl-[10px]'>
                        <button type="submit" onClick={()=>navigate('/AddNewemployeesecond')} className='w-full  h-full  bg-lemongreen rounded-[10px] cursor-pointer'>Next</button>
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
                    <p className='textLimegray'>Essential personal identification and key contact information — including your full name, address, phone number, and email — are required. These details help verify your identity, ensure smooth communication, and prevent delays or errors in processing your request.</p>
                    <p className='textLimegray'><strong className='text-formColor'>Tip:</strong> Double-check your spelling and numbers before submitting to avoid missed messages or verification issues</p>
                </div>
                </div>           
            </div>
            </div>
        </div>
        )
}

export default addNewemployee