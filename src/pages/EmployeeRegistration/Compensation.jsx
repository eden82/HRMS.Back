import React , {useState} from 'react'
import { useNavigate } from 'react-router-dom'

const Compensation = () => {
    const navigate = useNavigate()
    const [dropdownOpenP, setDropdownOpenP] = useState(false);
    const [selectedPayments, setSelectedPayments] = useState('Bank Transfer')
    const Payments = ['Bank Transfer', 'Hard Cash', 'ATM', 'Agenet']
    const [dropdownOpenC, setDropdownOpenC] = useState(false);
    const [selectedCurrencies, setSelectedCurrencies] = useState('USD')
    const Currencies = ['USD', 'EURO', 'BIRR']
    const [dropdownOpenB, setDropdownOpenB] = useState(false);
    const [selectedBenefits, setSelectedBenefits] = useState('Health Insurance')
    const Benefits = ['Health Insurance', 'Utility Insurance', 'Life insurance']
 
  return (
    <div className='font-semibold flex flex-col gap-[4rem]'>
        <div className='flex flex-col gap-[2.5rem]'>
            {/* Header */}
            <div className='flex items-center gap-[0.9375rem]'>
                <svg onClick={()=>navigate('/AddNewemployeesecond')} className='cursor-pointer' width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
        <path d="M20 12H4M4 12L10 6M4 12L10 18" stroke="#BEE532" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
                </svg>
                <li className='textWhite list-none'>3. Compensation & Legal</li>
            </div>

            {/* ProgressBar */}
            <div>
                <div className='grid grid-cols-4'>
                <div className='rounded-[0.53125rem] bg-lemongreen w-[22.625rem] h-[5px] '></div>
                <div className='rounded-[0.53125rem] bg-lemongreen w-[22.625rem] h-[5px] '></div>
                <div className='rounded-[0.53125rem] bg-lemongreen w-[22.625rem] h-[5px] '></div>
                <div className='rounded-[0.53125rem] bg-[rgba(223,223,223,0.26)] w-[22.625rem] h-[5px] '></div>
                </div>
            </div>
        </div>
        {/* MainContainer */}
        <div className='between gap-[12.25rem]'>
            <div className='w-[49.5625rem] h-[36.3125rem] overflow-y-auto scrollBarDash'>
                <form action="" className='flex gap-[2.5625rem] px-[10px]'>
                    <div className='flex flex-col w-[23.1875rem] gap-[35px]'>
                        {/* Salary */}
                        <div className='flex flex-col gap-[1rem]'>
                            <label htmlFor="Salary" className='text-formColor'>Salary</label>
                            <input id='Salary' type="number" placeholder='e.x 1000.00 $' className='inputMod'/>
                        </div>
                        {/* Payment */}
                        <div className='flex flex-col gap-[1rem] relative'>
                            <label htmlFor="Payment" className='text-formColor'>Payment Method</label>
                            <div className='inputMod flex items-center justify-between pr-[1.5625rem]' >
                                <button type="button" className='text-formColor text-left' onClick={() => setDropdownOpenP(!dropdownOpenP)}>{selectedPayments}</button>
                                <svg onClick={() => setDropdownOpenP(!dropdownOpenP)} className={`transition-transform duration-200 ${dropdownOpenP ? 'rotate-180' : ''}`} width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path d="M19 9L12 15L5 9" stroke="#BEE532" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
                                </svg>
                            </div>
                            <div className={`${dropdownOpenP ? 'flex' : 'hidden'} bg-inputBack rounded-[10px] h-[11.25rem]  w-full top-[6.3125rem] absolute z-10 text-formColor flex-col center-center border border-limeLight scrollBarDash overflow-y-auto `}>
                                <ul className='flex flex-col  gap-[2.1875rem] h-full pt-[1.3125rem]'>
                                    {Payments.map(Pay => (
                                    <li key={Pay} className={`cursor-pointer w-full text-center ${selectedPayments === Pay ? 'text-lemongreen font-bold' : ''}`}
                                    onClick={() => {
                                        setSelectedPayments(Pay)
                                        setDropdownOpenP(false)
                                    }}>
                                    {Pay}
                                    </li>
                                ))}
                                </ul>
                            </div>
                        </div>
                        {/* Tax */}
                        <div className='flex flex-col gap-[1rem]'>
                            <label htmlFor="Salary" className='text-formColor'>Tax Identification Number</label>
                            <input type="number" placeholder='e.x 78567578' className='inputMod'/>
                        </div>
                        {/* Passport */}
                        <div className='flex flex-col gap-[1rem]'>
                            <label htmlFor="Passport" className='text-formColor'>Passport Number</label>
                            <input id='Passport' type="number" placeholder='e.x 1000 234 153 6855' className='inputMod'/>
                        </div>
                        {/* contact */}
                        <div className='flex flex-col gap-[1rem]'>
                            <label htmlFor="Contact" className='text-formColor'>Contact File</label>
                            <label htmlFor="Contact" className='inputModfile cursor-pointer' >
                                <svg width="22" height="22" viewBox="0 0 22 22" fill="none" xmlns="http://www.w3.org/2000/svg">
    <path d="M12.8337 1.83301V5.49967C12.8337 5.9859 13.0268 6.45222 13.3706 6.79604C13.7144 7.13985 14.1808 7.33301 14.667 7.33301H18.3337M9.16699 8.24967H7.33366M14.667 11.9163H7.33366M14.667 15.583H7.33366M13.7503 1.83301H5.50033C5.0141 1.83301 4.54778 2.02616 4.20396 2.36998C3.86015 2.7138 3.66699 3.18011 3.66699 3.66634V18.333C3.66699 18.8192 3.86015 19.2856 4.20396 19.6294C4.54778 19.9732 5.0141 20.1663 5.50033 20.1663H16.5003C16.9866 20.1663 17.4529 19.9732 17.7967 19.6294C18.1405 19.2856 18.3337 18.8192 18.3337 18.333V6.41634L13.7503 1.83301Z" stroke="#BEE532" stroke-width="1.375" stroke-linecap="round" stroke-linejoin="round"/>
                                </svg>
                                <span className='text-limeLight'>Upload Contract File</span>
                            </label>
                            <input type="file" id='Contact' className='hidden'/>
                        </div>
                    </div>
                    <div className='w-[23.1875rem]'>
                        <div className='flex flex-col w-full gap-[35px]'>
                            {/* Curruncy */}
                            <div className='flex flex-col gap-[1rem] relative'>
                                <label htmlFor="firstName" className='text-formColor'>Currency</label>
                                <div className='inputMod flex items-center justify-between pr-[1.5625rem]' >
                                    <button type="button" className='text-formColor text-left' onClick={() => setDropdownOpenC(!dropdownOpenC)}>{selectedCurrencies}</button>
                                    <svg onClick={() => setDropdownOpenC(!dropdownOpenC)} className={`transition-transform duration-200 ${dropdownOpenC ? 'rotate-180' : ''}`} width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                    <path d="M19 9L12 15L5 9" stroke="#BEE532" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
                                    </svg>
                                </div>
                                <div className={`${dropdownOpenC ? 'flex' : 'hidden'} bg-inputBack rounded-[10px] h-[11.25rem]  w-full top-[6.3125rem] absolute z-10 text-formColor flex-col center-center border border-limeLight scrollBarDash overflow-y-auto `}>
                                    <ul className='flex flex-col  gap-[2.1875rem] h-full pt-[1.3125rem]'>
                                        {Currencies.map(Cny => (
                                        <li key={Cny} className={`cursor-pointer w-full text-center ${selectedCurrencies === Cny ? 'text-lemongreen font-bold' : ''}`}
                                        onClick={() => {
                                            setSelectedCurrencies(Cny)
                                            setDropdownOpenC(false)
                                        }}>
                                        {Cny}
                                        </li>
                                    ))}
                                    </ul>
                                </div>
                            </div>
                            {/* BankAccount */}
                            <div className='flex flex-col gap-[1rem]'>
                                <label htmlFor="Salary" className='text-formColor'>Bank Account Number</label>
                                <input type="number" placeholder='e.x 1000 234 153 6855' className='inputMod'/>
                            </div>
                            {/* Benefits */}
                            <div className='flex flex-col gap-[1rem] relative'>
                                <label htmlFor="firstName" className='text-formColor'>Benefits Enrollment</label>
                                <div className='inputMod flex items-center justify-between pr-[1.5625rem]' >
                                    <button type="button" className='text-formColor text-left' onClick={() => setDropdownOpenB(!dropdownOpenB)}>{selectedBenefits}</button>
                                    <svg onClick={() => setDropdownOpenB(!dropdownOpenB)} className={`transition-transform duration-200 ${dropdownOpenB ? 'rotate-180' : ''}`} width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                    <path d="M19 9L12 15L5 9" stroke="#BEE532" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
                                    </svg>
                                </div>
                                <div className={`${dropdownOpenB ? 'flex' : 'hidden'} bg-inputBack rounded-[10px] h-[11.25rem]  w-full top-[6.3125rem] absolute z-10 text-formColor flex-col center-center border border-limeLight scrollBarDash overflow-y-auto `}>
                                    <ul className='flex flex-col  gap-[2.1875rem] h-full pt-[1.3125rem]'>
                                        {Benefits.map(Bft => (
                                        <li key={Bft} className={`cursor-pointer w-full text-center ${selectedBenefits === Bft ? 'text-lemongreen font-bold' : ''}`}
                                        onClick={() => {
                                            setSelectedBenefits(Bft)
                                            setDropdownOpenB(false)
                                        }}>
                                        {Bft}
                                        </li>
                                    ))}
                                    </ul>
                                </div>
                            </div>
                            <div className='flex flex-col gap-[1rem]'>
                                <label htmlFor="Resume" className='text-formColor'>Resume</label>
                                <label htmlFor="Resume" className='inputModfile cursor-pointer'>
                                    <svg width="22" height="22" viewBox="0 0 22 22" fill="none" xmlns="http://www.w3.org/2000/svg">
        <path d="M12.8337 1.83301V5.49967C12.8337 5.9859 13.0268 6.45222 13.3706 6.79604C13.7144 7.13985 14.1808 7.33301 14.667 7.33301H18.3337M9.16699 8.24967H7.33366M14.667 11.9163H7.33366M14.667 15.583H7.33366M13.7503 1.83301H5.50033C5.0141 1.83301 4.54778 2.02616 4.20396 2.36998C3.86015 2.7138 3.66699 3.18011 3.66699 3.66634V18.333C3.66699 18.8192 3.86015 19.2856 4.20396 19.6294C4.54778 19.9732 5.0141 20.1663 5.50033 20.1663H16.5003C16.9866 20.1663 17.4529 19.9732 17.7967 19.6294C18.1405 19.2856 18.3337 18.8192 18.3337 18.333V6.41634L13.7503 1.83301Z" stroke="#BEE532" stroke-width="1.375" stroke-linecap="round" stroke-linejoin="round"/>
                                    </svg>
                                    <span className='text-limeLight'>Upload Cv</span>
                                </label>
                                <input type="file"  id='Resume' className='hidden w-full h-full'/>
                            </div>
                        </div>   
                    </div>
                </form>
                <div className='w-full h-[3.4375rem] my-[4rem] px-[10px]  flex gap-[2.5625rem]'>
                    <button type="button" onClick={()=>navigate('/AddNewemployeesecond')} className='w-[23.1875rem] border border-formColor text-formColor rounded-[10px] cursor-pointer'>Back</button>
                    <button type="submit" onClick={()=>navigate('/System')} className='w-[23.1875rem] bg-lemongreen rounded-[10px] cursor-pointer'>Next</button>
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

export default Compensation