import React from 'react'

const Organization = () => {
  return (
    <>
    {/* theMaincontainer */}
      <div className='space-y-[4.0625rem] font-semibold'>
        {/* theHeadersection */}
        <div className='flex items-center justify-between pl-[5.125rem] pr-[3.4375rem] h-[12.1875rem]  rounded-2xl bg-[rgba(190,229,50,0.05)]'>
          {/* firstsectionHeader */}
          <div className='flex items-center gap-[1.6875rem]'>
            <div className='border-4 border-white bg-center bg-[url(./image/OrganizationCircle.png)]  w-[7.40625rem] h-[7.4375rem]  rounded-[7.410625rem]'></div>
              <div>
                <h1 className='text-white text-xl'>Onyx Technology Inc.</h1>
                <h4 className='textLimegray'>Technology Solution Company</h4>
              </div>
          </div>
          {/* firstsectionHeaderEnd */}

          {/* secondPartheader */}
          <div className='w-[14.875rem] h-[3.4375rem]  rounded-[0.625rem] bg-lemongreen'>
            <button type='button'  className='cursor-pointer w-full h-full rounded-[0.625rem] flex items-center justify-center gap-[10px]'>
              <svg width="21" height="21" viewBox="0 0 21 21" fill="none" xmlns="http://www.w3.org/2000/svg">
<path fill-rule="evenodd" clip-rule="evenodd" d="M10.5 20.5C16.0228 20.5 20.5 16.0228 20.5 10.5C20.5 4.97715 16.0228 0.5 10.5 0.5C4.97715 0.5 0.5 4.97715 0.5 10.5C0.5 16.0228 4.97715 20.5 10.5 20.5ZM11.25 7.5C11.25 7.08579 10.9142 6.75 10.5 6.75C10.0858 6.75 9.75 7.08579 9.75 7.5V9.75H7.5C7.08579 9.75 6.75 10.0858 6.75 10.5C6.75 10.9142 7.08579 11.25 7.5 11.25H9.75V13.5C9.75 13.9142 10.0858 14.25 10.5 14.25C10.9142 14.25 11.25 13.9142 11.25 13.5V11.25H13.5C13.9142 11.25 14.25 10.9142 14.25 10.5C14.25 10.0858 13.9142 9.75 13.5 9.75H11.25V7.5Z" fill="#0D0F11"/>
              </svg>
              <span>Edit Organization</span>
            </button>
            
          </div>
          {/* secondPartheaderEnd */}
        </div>
        {/* themainContainer */}
        <div className=' space-y-[1.4375rem] pb-[7rem]'>
          <h1 className='text-white'>Department Overvire(5)</h1>
          
          {/* themainHolder */}
          <div className='w-full grid grid-cols-4 gap-[2.75rem]'>
              <div className='relative organizationCard'>
                <div className='flex flex-col gap-[3.75rem]'>
                  <div>
                    <h1 className='textWhite'>Engineering</h1>
                    <h4 className='text-limegray'>Head: Benjamin Endale</h4>
                  </div>
                  <div className='rounded-[2.0625rem] w-[9rem] h-[2.8125rem] bg-[rgba(190,229,50,0.05)] center-center'>
                    <h4 className='text-lemongreen text-sm'>324 employees</h4>
                  </div>
                </div>
              </div>
              <div className='organizationCard'>
                <div className='flex flex-col gap-[3.75rem]'>
                  <div>
                    <h1 className='textWhite'>Engineering</h1>
                    <h4 className='text-limegray'>Head: Benjamin Endale</h4>
                  </div>
                  <div className='rounded-[2.0625rem] w-[9rem] h-[2.8125rem] bg-[rgba(190,229,50,0.05)] center-center'>
                    <h4 className='text-lemongreen text-sm'>324 employees</h4>
                  </div>
                </div>
              </div>
              <div className='organizationCard'>
                <div className='flex flex-col gap-[3.75rem]'>
                  <div>
                    <h1 className='textWhite'>Engineering</h1>
                    <h4 className='text-limegray'>Head: Benjamin Endale</h4>
                  </div>
                  <div className='rounded-[2.0625rem] w-[9rem] h-[2.8125rem] bg-[rgba(190,229,50,0.05)] center-center'>
                    <h4 className='text-lemongreen text-sm'>324 employees</h4>
                  </div>
                </div>
              </div>
              <div className='organizationCard'>
                <div className='flex flex-col gap-[3.75rem]'>
                  <div>
                    <h1 className='textWhite'>Engineering</h1>
                    <h4 className='text-limegray'>Head: Benjamin Endale</h4>
                  </div>
                  <div className='rounded-[2.0625rem] w-[9rem] h-[2.8125rem] bg-[rgba(190,229,50,0.05)] center-center'>
                    <h4 className='text-lemongreen text-sm'>324 employees</h4>
                  </div>
                </div>
              </div>
                            <div className='organizationCard'>
                <div className='flex flex-col gap-[3.75rem]'>
                  <div>
                    <h1 className='textWhite'>Engineering</h1>
                    <h4 className='text-limegray'>Head: Benjamin Endale</h4>
                  </div>
                  <div className='rounded-[2.0625rem] w-[9rem] h-[2.8125rem] bg-[rgba(190,229,50,0.05)] center-center'>
                    <h4 className='text-lemongreen text-sm'>324 employees</h4>
                  </div>
                </div>
              </div>
              <div className='organizationCard1 '>
                <div className='flex-col center-center gap-[8px]'>
                  <svg width="47" height="48" viewBox="0 0 47 48" fill="none" xmlns="http://www.w3.org/2000/svg">
<path fill-rule="evenodd" clip-rule="evenodd" d="M23.5 43.5832C34.3155 43.5832 43.0834 34.8153 43.0834 23.9998C43.0834 13.1843 34.3155 4.4165 23.5 4.4165C12.6844 4.4165 3.91669 13.1843 3.91669 23.9998C3.91669 34.8153 12.6844 43.5832 23.5 43.5832ZM24.9688 18.1248C24.9688 17.3137 24.3112 16.6561 23.5 16.6561C22.6889 16.6561 22.0313 17.3137 22.0313 18.1248V22.5311H17.625C16.8139 22.5311 16.1563 23.1887 16.1563 23.9998C16.1563 24.811 16.8139 25.4686 17.625 25.4686H22.0313V29.8748C22.0313 30.686 22.6889 31.3436 23.5 31.3436C24.3112 31.3436 24.9688 30.686 24.9688 29.8748V25.4686H29.375C30.1862 25.4686 30.8438 24.811 30.8438 23.9998C30.8438 23.1887 30.1862 22.5311 29.375 22.5311H24.9688V18.1248Z" fill="#DFDFDF"/>
                  </svg>
                  <h4 className='text-limegray'>Add Department </h4>
                </div>
              </div>              
          </div>
        </div>
      </div>
    </>
  )
}

export default Organization