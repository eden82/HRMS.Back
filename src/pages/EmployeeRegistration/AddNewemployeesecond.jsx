import React , {useState} from 'react'
import { useNavigate } from 'react-router-dom';

const AddNewemployeesecond = () => {
  const navigate = useNavigate();
  const [dropdownOpen, setDropdownOpen] = useState(false);
  const [selectedEmployment, setSelectedEmployment] = useState('Full Time')
  const [dropdownOpenM, setDropdownOpenM] = useState(false);
  const [selectedManeger, setSelectedManeger] = useState('Maneger')
  const [dropdownOpenD, setDropdownOpenD] = useState(false);
  const [selectedDepartment, setSelectedDepartment] = useState('Department')
  const Employment = ['Full Time','Half Time','Remote']
  const Maneger = ['Manager','Manager','Manager']
  const Department = ['Department','Department1','Department2']
  return (
    <div className='font-semibold flex flex-col gap-[4rem]'>
      {/* headerContainer */}
      <div className='flex flex-col gap-[2.5rem]'>
      {/* Header */}
        <div className='flex items-center gap-[0.9375rem]'>
            <svg onClick={()=>navigate('/AddNewemployee')} className='cursor-pointer' width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
    <path d="M20 12H4M4 12L10 6M4 12L10 18" stroke="#BEE532" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
            </svg>
            <li className='textWhite list-none'>2. Employment Details</li>
        </div>

      {/* ProgressBar */}
        <div>
            <div className='grid grid-cols-4'>
              <div className='rounded-[0.53125rem] bg-lemongreen w-[22.625rem] h-[5px] '></div>
              <div className='rounded-[0.53125rem] bg-lemongreen w-[22.625rem] h-[5px] '></div>
              <div className='rounded-[0.53125rem] bg-[rgba(223,223,223,0.26)] w-[22.625rem] h-[5px] '></div>
              <div className='rounded-[0.53125rem] bg-[rgba(223,223,223,0.26)] w-[22.625rem] h-[5px] '></div>
            </div>
        </div>
      </div>
        <div className='between gap-[12.25rem]'>
          {/* mainContent */}
          <div className='w-[49.5625rem] h-[36.3125rem] overflow-y-auto scrollBarDash' > 
              <form action="" className='flex gap-[2.5625rem] px-[10px]'>
                <div className='flex flex-col w-[23.1875rem] gap-[35px]'>
                  <div className='flex flex-col gap-[1rem]'>
                      <label htmlFor="firstName" className='text-formColor'>Job Title</label>
                      <input type="text" placeholder='e.x senior developer' className='inputMod'/>
                  </div>
                  <div className='flex flex-col gap-[1rem] relative'>
                    <label htmlFor="firstName" className='text-formColor'>Employment Type</label>
                    <div className='inputMod flex items-center justify-between pr-[1.5625rem]' >
                        <button type="button" className='text-formColor text-left' onClick={() => setDropdownOpen(!dropdownOpen)}>{selectedEmployment}</button>
                        <svg onClick={() => setDropdownOpen(!dropdownOpen)} className={`transition-transform duration-200 ${dropdownOpen ? 'rotate-180' : ''}`} width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
        <path d="M19 9L12 15L5 9" stroke="#BEE532" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
                        </svg>
                    </div>
                    <div className={`${dropdownOpen ? 'flex' : 'hidden'} bg-inputBack rounded-[10px] h-[11.25rem]  w-full top-[6.3125rem] absolute z-10 text-formColor flex-col center-center border border-limeLight scrollBarDash overflow-y-auto `}>
                        <ul className='flex flex-col  gap-[2.1875rem] h-full pt-[1.3125rem]'>
                            {Employment.map(Employ => (
                            <li key={Employ} className={`cursor-pointer w-full text-center ${selectedEmployment === Employ ? 'text-lemongreen font-bold' : ''}`}
                            onClick={() => {
                                setSelectedEmployment(Employ)
                                setDropdownOpen(false)
                            }}>
                            {Employ}
                            </li>
                        ))}
                        </ul>
                    </div>
                  </div>
                  {/* Manager */}
                  <div className='flex flex-col gap-[1rem] relative'>
                    <label htmlFor="firstName" className='text-formColor'>Manager</label>
                    <div className='inputMod flex items-center justify-between pr-[1.5625rem]' >
                        <button type="button" className='text-formColor text-left' onClick={() => setDropdownOpen(!dropdownOpenM)}>{selectedManeger}</button>
                        <svg onClick={() => setDropdownOpenM(!dropdownOpenM)} className={`transition-transform duration-200 ${dropdownOpenM ? 'rotate-180' : ''}`} width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
        <path d="M19 9L12 15L5 9" stroke="#BEE532" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
                        </svg>
                    </div>
                    <div className={`${dropdownOpenM ? 'flex' : 'hidden'} bg-inputBack rounded-[10px] h-[11.25rem]  w-full top-[6.3125rem] absolute z-10 text-formColor flex-col center-center border border-limeLight scrollBarDash overflow-y-auto `}>
                        <ul className='flex flex-col  gap-[2.1875rem] h-full pt-[1.3125rem]'>
                            {Maneger.map(Maneg => (
                            <li key={Maneg} className={`cursor-pointer w-full text-center ${selectedManeger === Maneg ? 'text-lemongreen font-bold' : ''}`}
                            onClick={() => {
                                setSelectedManeger(Maneg)
                                setDropdownOpenM(false)
                            }}>
                            {Maneg}
                            </li>
                        ))}
                        </ul>
                    </div>
                  </div>
                </div>
                <div className='w-[23.1875rem] flex flex-col gap-[35px]'>
                  <div className='flex flex-col gap-[1rem] relative'>
                    <label htmlFor="firstName" className='text-formColor'>Department</label>
                    <div className='inputMod flex items-center justify-between pr-[1.5625rem]' >
                        <button type="button" className='text-formColor text-left' onClick={() => setDropdownOpenD(!dropdownOpenD)}>{selectedDepartment}</button>
                        <svg onClick={() => setDropdownOpenD(!dropdownOpenD)} className={`transition-transform duration-200 ${dropdownOpenD ? 'rotate-180' : ''}`} width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
        <path d="M19 9L12 15L5 9" stroke="#BEE532" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
                        </svg>
                    </div>
                    <div className={`${dropdownOpenD ? 'flex' : 'hidden'} bg-inputBack rounded-[10px] h-[11.25rem]  w-full top-[6.3125rem] absolute z-10 text-formColor flex-col center-center border border-limeLight scrollBarDash overflow-y-auto `}>
                        <ul className='flex flex-col  gap-[2.1875rem] h-full pt-[1.3125rem]'>
                            {Department.map(Depmt => (
                            <li key={Depmt} className={`cursor-pointer w-full text-center ${selectedDepartment === Depmt ? 'text-lemongreen font-bold' : ''}`}
                            onClick={() => {
                                setSelectedDepartment(Depmt)
                                setDropdownOpenD(false)
                            }}>
                            {Depmt}
                            </li>
                        ))}
                        </ul>
                    </div>
                  </div>
                  <div className='flex flex-col gap-[1rem]'>
                    <label htmlFor="firstName" className='text-formColor'>Joining Date</label>
                    <input type="date"   className='inputMod pr-[1.5625rem]'/>
                  </div>
                </div>
              </form>
              <div className='w-full h-[3.4375rem] mt-[4rem] flex gap-[2.5625rem]'>
                <button type="button" onClick={()=>navigate('/AddNewemployee')} className='w-[23.1875rem] border border-formColor text-formColor rounded-[10px] cursor-pointer'>Back</button>
                <button type="submit" onClick={()=>navigate('/Compensation')} className='w-[23.1875rem] bg-lemongreen rounded-[10px] cursor-pointer'>Next</button>
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
                <p className='textLimegray'>Provide accurate information about your current employment status, including your job title, employe's name, and contact information. This helps establish your professional background and may be necessary for verification or eligibility purposes.</p>
                <p className='textLimegray'><strong className='text-formColor'>Tip:</strong> Make sure to list your employer's official name and provide a valid work email or phone number if requested.</p>
            </div>
            </div>           
        </div>
      </div>
    </div>

  )
}

export default AddNewemployeesecond