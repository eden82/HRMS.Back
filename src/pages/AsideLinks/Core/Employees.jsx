import React from 'react'
import {useNavigate} from 'react-router-dom'
const Employees = () => {

const navigate = useNavigate();


  return (
    <div className='font-semibold flex flex-col gap-[3.9375rem]'>
      {/* headerSearcharea */}
      <div className='flex items-center gap-[2.125rem]'>
          <div className='w-[71.9375rem] h-[3.4375rem]  flex items-center gap-[1.1875rem] bg-[#1D2015] rounded-[0.625rem] px-[1.4375rem] '>
            <svg width="21" height="21" viewBox="0 0 21 21" fill="none" xmlns="http://www.w3.org/2000/svg">
<path d="M10.0625 18.375C14.6534 18.375 18.375 14.6534 18.375 10.0625C18.375 5.47163 14.6534 1.75 10.0625 1.75C5.47163 1.75 1.75 5.47163 1.75 10.0625C1.75 14.6534 5.47163 18.375 10.0625 18.375Z" stroke="#5D6150" stroke-width="1.3125"/>
<path d="M17.5 17.5L19.25 19.25" stroke="#5D6150" stroke-width="1.3125" stroke-linecap="round"/>
            </svg>
            <input type="search" placeholder="Search employee by name,email or ID" className='placeholder-input text-white  w-full h-full outline-0' name="" id="" />
          </div>
          <div className='w-[18.125rem] h-[3.4375rem]  flex items-center justify-center rounded-[0.625rem] bg-[#151812] gap-[4.6875rem]'>
            <div className='flex items-center gap-[0.625rem]'>
              <svg width="20" height="18" viewBox="0 0 20 18" fill="none" xmlns="http://www.w3.org/2000/svg">
  <path d="M15.8334 1.5H4.16675C2.98824 1.5 2.39898 1.5 2.03286 1.8435C1.66675 2.187 1.66675 2.73985 1.66675 3.84555V4.4204C1.66675 5.28527 1.66675 5.7177 1.88308 6.07618C2.09941 6.43467 2.49464 6.65715 3.2851 7.10213L5.71261 8.46867C6.24296 8.76717 6.50814 8.9165 6.69801 9.08133C7.09341 9.42458 7.33681 9.82792 7.44711 10.3227C7.50008 10.5602 7.50008 10.8382 7.50008 11.3941V13.6187C7.50008 14.3767 7.50008 14.7556 7.71001 15.0511C7.91996 15.3465 8.29281 15.4922 9.03858 15.7838C10.6041 16.3958 11.3868 16.7018 11.9435 16.3537C12.5001 16.0055 12.5001 15.2099 12.5001 13.6187V11.3941C12.5001 10.8382 12.5001 10.5602 12.5531 10.3227C12.6633 9.82792 12.9067 9.42458 13.3022 9.08133C13.492 8.9165 13.7572 8.76717 14.2876 8.46867L16.7151 7.10213C17.5055 6.65715 17.9007 6.43467 18.1171 6.07618C18.3334 5.7177 18.3334 5.28527 18.3334 4.4204V3.84555C18.3334 2.73985 18.3334 2.187 17.9673 1.8435C17.6012 1.5 17.0119 1.5 15.8334 1.5Z" stroke="#5D6150" stroke-width="1.5"/>
              </svg>
              <span className='text-white'>All Department</span>
            </div>
            <svg width="16" height="8" viewBox="0 0 16 8" fill="none" xmlns="http://www.w3.org/2000/svg">
<path d="M15 1L8 7L1 1" stroke="#BEE532" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
            </svg>
          </div>
      </div>

      {/* headerSection2 */}
      <div className='between'>
        <div>
          <h1 className='textWhite'>Employees Directory</h1>
          <h4 className='textLimegray'>Manage employee profiles roles, and organization structure</h4>
        </div>
        <button type="button" className='cursor-pointer ' onClick={()=>navigate('/AddNewemployee')}>
          <div className='center-center w-[12.75rem] h-[3.125rem] rounded-[0.625rem] gap-[0.625rem] bg-lemongreen'>
            <svg width="21" height="20" viewBox="0 0 21 20" fill="none" xmlns="http://www.w3.org/2000/svg">
  <path fill-rule="evenodd" clip-rule="evenodd" d="M10.5 20C16.0228 20 20.5 15.5228 20.5 10C20.5 4.47715 16.0228 0 10.5 0C4.97715 0 0.5 4.47715 0.5 10C0.5 15.5228 4.97715 20 10.5 20ZM11.25 7C11.25 6.58579 10.9142 6.25 10.5 6.25C10.0858 6.25 9.75 6.58579 9.75 7V9.25H7.5C7.08579 9.25 6.75 9.5858 6.75 10C6.75 10.4142 7.08579 10.75 7.5 10.75H9.75V13C9.75 13.4142 10.0858 13.75 10.5 13.75C10.9142 13.75 11.25 13.4142 11.25 13V10.75H13.5C13.9142 10.75 14.25 10.4142 14.25 10C14.25 9.5858 13.9142 9.25 13.5 9.25H11.25V7Z" fill="#0D0F11"/>
            </svg>
            <span className='text-black'>Add Employees</span>
          </div>
        </button>
      </div>
      {/* mainContentArea */}
      <div className='h-[31.25rem]  overflow-y-auto scrollBarDash'>
        <table className='w-full text-left'>
          <thead className='text-white border-b border-limegray'>
            <tr>
              <th className='pb-[0.8125rem] pr-[7.5rem]'>Employee ID</th>
              <th className='pb-[0.8125rem] pr-[7.5rem]'>Employee Name</th>
              <th className='pb-[0.8125rem] pr-[7.5rem] '>Department</th>
              <th className='pb-[0.8125rem] pr-[16.875rem]'>Position</th>
              <th className='pb-[0.8125rem] '>Action</th>
            </tr>
          </thead>
          <tbody className='text-input'>
            <tr>
              <td className='pt-[2.25rem]'>EMP002</td>
              <td className='pt-[2.25rem]'>Benjamin Endale</td>
              <td className='pt-[2.25rem]'>Engineering</td>
              <td className='pt-[2.25rem] max-w-[150px] whitespace-normal break-words'>Senior Software Developer</td>
              <td className='flex items-center gap-[2.5625rem] pt-[2.25rem]'>
                <button type="button" className='cursor-pointer'>
                  <svg width="22" height="22" viewBox="0 0 22 22" fill="none" xmlns="http://www.w3.org/2000/svg">
    <path d="M1.89006 11.3191C1.81366 11.1133 1.81366 10.8869 1.89006 10.6811C2.63412 8.87695 3.89712 7.33437 5.51893 6.24891C7.14075 5.16345 9.04835 4.58398 10.9999 4.58398C12.9514 4.58398 14.859 5.16345 16.4809 6.24891C18.1027 7.33437 19.3657 8.87695 20.1097 10.6811C20.1861 10.8869 20.1861 11.1133 20.1097 11.3191C19.3657 13.1232 18.1027 14.6658 16.4809 15.7513C14.859 16.8367 12.9514 17.4162 10.9999 17.4162C9.04835 17.4162 7.14075 16.8367 5.51893 15.7513C3.89712 14.6658 2.63412 13.1232 1.89006 11.3191Z" stroke="#BEE532" stroke-width="1.375" stroke-linecap="round" stroke-linejoin="round"/>
    <path d="M10.9999 13.7501C12.5187 13.7501 13.7499 12.5189 13.7499 11.0001C13.7499 9.4813 12.5187 8.25009 10.9999 8.25009C9.48111 8.25009 8.24989 9.4813 8.24989 11.0001C8.24989 12.5189 9.48111 13.7501 10.9999 13.7501Z" stroke="#BEE532" stroke-width="1.375" stroke-linecap="round" stroke-linejoin="round"/>
                  </svg>
                </button>
                <button type="button" className='cursor-pointer'>
                  <svg width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
    <path fill-rule="evenodd" clip-rule="evenodd" d="M3.25 22C3.25 21.5858 3.58579 21.25 4 21.25H20C20.4142 21.25 20.75 21.5858 20.75 22C20.75 22.4142 20.4142 22.75 20 22.75H4C3.58579 22.75 3.25 22.4142 3.25 22Z" fill="#BEE532"/>
    <path d="M11.5201 14.9294L17.4368 9.01261C16.6315 8.67746 15.6777 8.12692 14.7757 7.22491C13.8736 6.32274 13.323 5.36882 12.9879 4.56348L7.07106 10.4803C6.60937 10.942 6.37846 11.1729 6.17992 11.4275C5.94571 11.7277 5.74491 12.0526 5.58107 12.3964C5.44219 12.6878 5.33894 12.9976 5.13245 13.6171L4.04356 16.8837C3.94194 17.1886 4.02128 17.5247 4.2485 17.7519C4.47573 17.9791 4.81182 18.0585 5.11667 17.9568L8.38334 16.868C9.00281 16.6615 9.31256 16.5582 9.60398 16.4193C9.94775 16.2555 10.2727 16.0547 10.5729 15.8205C10.8275 15.6219 11.0584 15.3911 11.5201 14.9294Z" fill="#BEE532"/>
    <path d="M19.0786 7.37044C20.3071 6.14188 20.3071 4.14999 19.0786 2.92142C17.85 1.69286 15.8581 1.69286 14.6296 2.92142L13.9199 3.63105C13.9296 3.6604 13.9397 3.69015 13.9502 3.72028C14.2103 4.47 14.701 5.45281 15.6243 6.37602C16.5475 7.29923 17.5303 7.78999 18.28 8.05009C18.31 8.0605 18.3396 8.07054 18.3688 8.08021L19.0786 7.37044Z" fill="#BEE532"/>
                  </svg>
                </button>
                <button type="button" className='cursor-pointer'>
                  <svg width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
    <path d="M3 6.38597C3 5.90152 3.34538 5.50879 3.77143 5.50879L6.43567 5.50832C6.96502 5.49306 7.43202 5.11033 7.61214 4.54412C7.61688 4.52923 7.62232 4.51087 7.64185 4.44424L7.75665 4.05256C7.8269 3.81241 7.8881 3.60318 7.97375 3.41617C8.31209 2.67736 8.93808 2.16432 9.66147 2.03297C9.84457 1.99972 10.0385 1.99986 10.2611 2.00002H13.7391C13.9617 1.99986 14.1556 1.99972 14.3387 2.03297C15.0621 2.16432 15.6881 2.67736 16.0264 3.41617C16.1121 3.60318 16.1733 3.81241 16.2435 4.05256L16.3583 4.44424C16.3778 4.51087 16.3833 4.52923 16.388 4.54412C16.5682 5.11033 17.1278 5.49353 17.6571 5.50879H20.2286C20.6546 5.50879 21 5.90152 21 6.38597C21 6.87043 20.6546 7.26316 20.2286 7.26316H3.77143C3.34538 7.26316 3 6.87043 3 6.38597Z" fill="#BEE532"/>
    <path fill-rule="evenodd" clip-rule="evenodd" d="M11.5956 22.0006H12.4044C15.1871 22.0006 16.5785 22.0006 17.4831 21.1147C18.3878 20.2288 18.4803 18.7756 18.6654 15.8691L18.9321 11.6812C19.0326 10.1042 19.0828 9.31573 18.6289 8.81607C18.1751 8.31641 17.4087 8.31641 15.876 8.31641H8.12404C6.59127 8.31641 5.82488 8.31641 5.37105 8.81607C4.91722 9.31573 4.96744 10.1042 5.06788 11.6812L5.33459 15.8691C5.5197 18.7756 5.61225 20.2288 6.51689 21.1147C7.42153 22.0006 8.81289 22.0006 11.5956 22.0006ZM10.2463 12.1891C10.2051 11.7553 9.83753 11.4387 9.42537 11.4821C9.01321 11.5255 8.71251 11.9124 8.75372 12.3462L9.25372 17.6094C9.29494 18.0432 9.66247 18.3598 10.0746 18.3164C10.4868 18.273 10.7875 17.8861 10.7463 17.4523L10.2463 12.1891ZM14.5746 11.4821C14.9868 11.5255 15.2875 11.9124 15.2463 12.3462L14.7463 17.6094C14.7051 18.0432 14.3375 18.3598 13.9254 18.3164C13.5132 18.273 13.2125 17.8861 13.2537 17.4523L13.7537 12.1891C13.7949 11.7553 14.1625 11.4387 14.5746 11.4821Z" fill="#BEE532"/>
                  </svg>
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
      {/* mainContentAreafile */}
    </div>
  )
}

export default Employees