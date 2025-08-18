import React from 'react'

const Dashboard = () => {
  return (
    <>
      {/* mainContainer */}
      <div className='flex gap-[3.25rem]'>
        {/* firstSection */}
        <div className='flex flex-col gap-[4.875rem] font-semibold'>
          {/* firstCardsection */}
          <div className='flex gap-[2.5625rem]'>
            {/* cardDashMainContent */}
            <div className='cardDash bg-[url(./image/imageDashcard.png)]'>
              <div className='h-full between flex-col'>
                <div className='flex justify-between'>
                  <div>
                    <svg width="21" height="21" viewBox="0 0 21 21" fill="none" xmlns="http://www.w3.org/2000/svg">
  <path d="M7.875 8.75C9.808 8.75 11.375 7.183 11.375 5.25C11.375 3.317 9.808 1.75 7.875 1.75C5.942 1.75 4.375 3.317 4.375 5.25C4.375 7.183 5.942 8.75 7.875 8.75Z" stroke="#0D0F11" stroke-width="1.5"/>
  <path d="M13.125 7.875C14.5748 7.875 15.75 6.69974 15.75 5.25C15.75 3.80026 14.5748 2.625 13.125 2.625" stroke="#0D0F11" stroke-width="1.5" stroke-linecap="round"/>
  <path d="M7.875 18.375C11.2577 18.375 14 16.808 14 14.875C14 12.942 11.2577 11.375 7.875 11.375C4.49226 11.375 1.75 12.942 1.75 14.875C1.75 16.808 4.49226 18.375 7.875 18.375Z" stroke="#0D0F11" stroke-width="1.5"/>
  <path d="M15.75 12.25C17.2849 12.5866 18.375 13.439 18.375 14.4375C18.375 15.3381 17.488 16.12 16.1875 16.5116" stroke="#0D0F11" stroke-width="1.5" stroke-linecap="round"/>
                    </svg>
                  </div>
                  <div className='border-none  rounded-full center-center bg-white w-[49px] h-[49px]'>
                    <svg width="21" height="21" viewBox="0 0 21 21" fill="none" xmlns="http://www.w3.org/2000/svg">
<path d="M5.5 15.5L15.5 5.5M15.5 5.5H8M15.5 5.5V13" stroke="#3E4B0E" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
                    </svg>
                  </div>
                </div>
                <div className='flex flex-col'>
                  <span className='text-5xl'>1230</span>
                  <span>Total Employees</span>
                </div>
              </div>
            </div>
            {/* cardDashMainContent */}
            <div className='cardDash bg-[rgba(190,229,50,0.03)]'>
              <div className='h-full between flex-col'>
                <div className='flex justify-between'>
                  <div>
                    <svg width="21" height="21" viewBox="0 0 21 21" fill="none" xmlns="http://www.w3.org/2000/svg">
                    <path d="M8.75 6.125H1.75" stroke="#DFDFDF" stroke-width="1.3125" stroke-linecap="round"/>
                    <path d="M7 10.5H1.75" stroke="#DFDFDF" stroke-width="1.3125" stroke-linecap="round"/>
                    <path d="M8.75 14.875H1.75" stroke="#DFDFDF" stroke-width="1.3125" stroke-linecap="round"/>
                    <path d="M14.875 14.875C17.2912 14.875 19.25 12.9162 19.25 10.5C19.25 8.08375 17.2912 6.125 14.875 6.125C12.4588 6.125 10.5 8.08375 10.5 10.5C10.5 12.9162 12.4588 14.875 14.875 14.875Z" stroke="#DFDFDF" stroke-width="1.3125"/>
                    <path d="M14.875 8.75V10.3654L15.75 11.375" stroke="#DFDFDF" stroke-width="1.3125" stroke-linecap="round" stroke-linejoin="round"/>
                    </svg>
                  </div>
                  <div className='border-none  rounded-full center-center bg-white w-[49px] h-[49px]'>
                    <svg width="21" height="21" viewBox="0 0 21 21" fill="none" xmlns="http://www.w3.org/2000/svg">
<path d="M5.5 15.5L15.5 5.5M15.5 5.5H8M15.5 5.5V13" stroke="#3E4B0E" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
                    </svg>
                  </div>
                </div>
                <div className='flex flex-col'>
                  <span className='text-5xl text-white'>234</span>
                  <span className='text-white'>Today's Attendance</span>
                </div>
              </div>
            </div>
          </div>

          {/* secondSectionofDash */}
          <div className='space-y-[3.875rem]'>
            <div className='between'>
              {/* header */}
              <div>
                <h1 className='text-white text-xl'>Department Overview</h1>
                <h4 className='text-limegray text-[15px] font-medium'>Employee count and attendance by department</h4>
              </div>
              <button type="button" className='text-lemongreen font-medium self-end text-[15px]'>See more</button>
            </div>

            {/* headerSection */}
            <div className='space-y-[2.25rem] h-[23.0625rem] scrollBarDash overflow-y-auto'>
              <div className='space-y-[1.1875rem]'>
                <div className='between-center'>
                  <div className='flex space-x-[1.375rem]'>
                    <svg width="21" height="21" viewBox="0 0 21 21" fill="none" xmlns="http://www.w3.org/2000/svg">
    <path d="M19.25 19.25H1.75" stroke="#5D6150" stroke-width="1.3125" stroke-linecap="round"/>
    <path d="M14.875 19.25V5.25C14.875 3.60008 14.875 2.77512 14.3624 2.26257C13.8498 1.75 13.0249 1.75 11.375 1.75H9.625C7.97508 1.75 7.15012 1.75 6.63757 2.26257C6.125 2.77512 6.125 3.60008 6.125 5.25V19.25" stroke="#5D6150" stroke-width="1.3125"/>
    <path d="M18.375 19.25V10.0625C18.375 8.83356 18.375 8.21915 18.08 7.77775C17.9524 7.58667 17.7883 7.42261 17.5972 7.29493C17.1559 7 16.5414 7 15.3125 7" stroke="#5D6150" stroke-width="1.3125"/>
    <path d="M2.625 19.25V10.0625C2.625 8.83356 2.625 8.21915 2.91993 7.77775C3.04761 7.58667 3.21167 7.42261 3.40275 7.29493C3.84415 7 4.4586 7 5.6875 7" stroke="#5D6150" stroke-width="1.3125"/>
    <path d="M10.5 19.25V16.625" stroke="#5D6150" stroke-width="1.3125" stroke-linecap="round"/>
    <path d="M8.75 4.375H12.25" stroke="#5D6150" stroke-width="1.3125" stroke-linecap="round"/>
    <path d="M8.75 7H12.25" stroke="#5D6150" stroke-width="1.3125" stroke-linecap="round"/>
    <path d="M8.75 9.625H12.25" stroke="#5D6150" stroke-width="1.3125" stroke-linecap="round"/>
    <path d="M8.75 12.25H12.25" stroke="#5D6150" stroke-width="1.3125" stroke-linecap="round"/>
                    </svg>
                    <span className='text-limegray '>Engineering</span>
                  </div>
                  <div>
                    <ul className='text-white flex gap-[1.75rem]'>
                      <li className='textLimegray'>324 employees</li>
                      <li className='list-disc marker:text-lemongreen textLimegray'>95% attendance</li>
                    </ul>
                  </div>
                </div>
                {/* progressBar*/}
                <div className='h-[5px] w-full bg-white rounded-[30px]'></div>
              </div>
              <div className='space-y-[1.1875rem]'>
                <div className='between-center'>
                  <div className='flex space-x-[1.375rem]'>
                    <svg width="21" height="21" viewBox="0 0 21 21" fill="none" xmlns="http://www.w3.org/2000/svg">
    <path d="M19.25 19.25H1.75" stroke="#5D6150" stroke-width="1.3125" stroke-linecap="round"/>
    <path d="M14.875 19.25V5.25C14.875 3.60008 14.875 2.77512 14.3624 2.26257C13.8498 1.75 13.0249 1.75 11.375 1.75H9.625C7.97508 1.75 7.15012 1.75 6.63757 2.26257C6.125 2.77512 6.125 3.60008 6.125 5.25V19.25" stroke="#5D6150" stroke-width="1.3125"/>
    <path d="M18.375 19.25V10.0625C18.375 8.83356 18.375 8.21915 18.08 7.77775C17.9524 7.58667 17.7883 7.42261 17.5972 7.29493C17.1559 7 16.5414 7 15.3125 7" stroke="#5D6150" stroke-width="1.3125"/>
    <path d="M2.625 19.25V10.0625C2.625 8.83356 2.625 8.21915 2.91993 7.77775C3.04761 7.58667 3.21167 7.42261 3.40275 7.29493C3.84415 7 4.4586 7 5.6875 7" stroke="#5D6150" stroke-width="1.3125"/>
    <path d="M10.5 19.25V16.625" stroke="#5D6150" stroke-width="1.3125" stroke-linecap="round"/>
    <path d="M8.75 4.375H12.25" stroke="#5D6150" stroke-width="1.3125" stroke-linecap="round"/>
    <path d="M8.75 7H12.25" stroke="#5D6150" stroke-width="1.3125" stroke-linecap="round"/>
    <path d="M8.75 9.625H12.25" stroke="#5D6150" stroke-width="1.3125" stroke-linecap="round"/>
    <path d="M8.75 12.25H12.25" stroke="#5D6150" stroke-width="1.3125" stroke-linecap="round"/>
                    </svg>
                    <span className='text-limegray '>Engineering</span>
                  </div>
                  <div>
                    <ul className='text-white flex gap-[1.75rem]'>
                      <li className='textLimegray'>324 employees</li>
                      <li className='list-disc marker:text-lemongreen textLimegray'>95% attendance</li>
                    </ul>
                  </div>
                </div>
                {/* progressBar*/}
                <div className='h-[5px] w-full bg-white rounded-[30px]'></div>
              </div>
              <div className='space-y-[1.1875rem]'>
                <div className='between-center'>
                  <div className='flex space-x-[1.375rem]'>
                    <svg width="21" height="21" viewBox="0 0 21 21" fill="none" xmlns="http://www.w3.org/2000/svg">
    <path d="M19.25 19.25H1.75" stroke="#5D6150" stroke-width="1.3125" stroke-linecap="round"/>
    <path d="M14.875 19.25V5.25C14.875 3.60008 14.875 2.77512 14.3624 2.26257C13.8498 1.75 13.0249 1.75 11.375 1.75H9.625C7.97508 1.75 7.15012 1.75 6.63757 2.26257C6.125 2.77512 6.125 3.60008 6.125 5.25V19.25" stroke="#5D6150" stroke-width="1.3125"/>
    <path d="M18.375 19.25V10.0625C18.375 8.83356 18.375 8.21915 18.08 7.77775C17.9524 7.58667 17.7883 7.42261 17.5972 7.29493C17.1559 7 16.5414 7 15.3125 7" stroke="#5D6150" stroke-width="1.3125"/>
    <path d="M2.625 19.25V10.0625C2.625 8.83356 2.625 8.21915 2.91993 7.77775C3.04761 7.58667 3.21167 7.42261 3.40275 7.29493C3.84415 7 4.4586 7 5.6875 7" stroke="#5D6150" stroke-width="1.3125"/>
    <path d="M10.5 19.25V16.625" stroke="#5D6150" stroke-width="1.3125" stroke-linecap="round"/>
    <path d="M8.75 4.375H12.25" stroke="#5D6150" stroke-width="1.3125" stroke-linecap="round"/>
    <path d="M8.75 7H12.25" stroke="#5D6150" stroke-width="1.3125" stroke-linecap="round"/>
    <path d="M8.75 9.625H12.25" stroke="#5D6150" stroke-width="1.3125" stroke-linecap="round"/>
    <path d="M8.75 12.25H12.25" stroke="#5D6150" stroke-width="1.3125" stroke-linecap="round"/>
                    </svg>
                    <span className='text-limegray '>Engineering</span>
                  </div>
                  <div>
                    <ul className='text-white flex gap-[1.75rem]'>
                      <li className='textLimegray'>324 employees</li>
                      <li className='list-disc marker:text-lemongreen textLimegray'>95% attendance</li>
                    </ul>
                  </div>
                </div>
                {/* progressBar*/}
                <div className='h-[5px] w-full bg-white rounded-[30px]'></div>
              </div>
              <div className='space-y-[1.1875rem]'>
                <div className='between-center'>
                  <div className='flex space-x-[1.375rem]'>
                    <svg width="21" height="21" viewBox="0 0 21 21" fill="none" xmlns="http://www.w3.org/2000/svg">
    <path d="M19.25 19.25H1.75" stroke="#5D6150" stroke-width="1.3125" stroke-linecap="round"/>
    <path d="M14.875 19.25V5.25C14.875 3.60008 14.875 2.77512 14.3624 2.26257C13.8498 1.75 13.0249 1.75 11.375 1.75H9.625C7.97508 1.75 7.15012 1.75 6.63757 2.26257C6.125 2.77512 6.125 3.60008 6.125 5.25V19.25" stroke="#5D6150" stroke-width="1.3125"/>
    <path d="M18.375 19.25V10.0625C18.375 8.83356 18.375 8.21915 18.08 7.77775C17.9524 7.58667 17.7883 7.42261 17.5972 7.29493C17.1559 7 16.5414 7 15.3125 7" stroke="#5D6150" stroke-width="1.3125"/>
    <path d="M2.625 19.25V10.0625C2.625 8.83356 2.625 8.21915 2.91993 7.77775C3.04761 7.58667 3.21167 7.42261 3.40275 7.29493C3.84415 7 4.4586 7 5.6875 7" stroke="#5D6150" stroke-width="1.3125"/>
    <path d="M10.5 19.25V16.625" stroke="#5D6150" stroke-width="1.3125" stroke-linecap="round"/>
    <path d="M8.75 4.375H12.25" stroke="#5D6150" stroke-width="1.3125" stroke-linecap="round"/>
    <path d="M8.75 7H12.25" stroke="#5D6150" stroke-width="1.3125" stroke-linecap="round"/>
    <path d="M8.75 9.625H12.25" stroke="#5D6150" stroke-width="1.3125" stroke-linecap="round"/>
    <path d="M8.75 12.25H12.25" stroke="#5D6150" stroke-width="1.3125" stroke-linecap="round"/>
                    </svg>
                    <span className='text-limegray '>Engineering</span>
                  </div>
                  <div>
                    <ul className='text-white flex gap-[1.75rem]'>
                      <li className='textLimegray'>324 employees</li>
                      <li className='list-disc marker:text-lemongreen textLimegray'>95% attendance</li>
                    </ul>
                  </div>
                </div>
                {/* progressBar*/}
                <div className='h-[5px] w-full bg-white rounded-[30px]'></div>
              </div>
                            <div className='space-y-[1.1875rem]'>
                <div className='between-center'>
                  <div className='flex space-x-[1.375rem]'>
                    <svg width="21" height="21" viewBox="0 0 21 21" fill="none" xmlns="http://www.w3.org/2000/svg">
    <path d="M19.25 19.25H1.75" stroke="#5D6150" stroke-width="1.3125" stroke-linecap="round"/>
    <path d="M14.875 19.25V5.25C14.875 3.60008 14.875 2.77512 14.3624 2.26257C13.8498 1.75 13.0249 1.75 11.375 1.75H9.625C7.97508 1.75 7.15012 1.75 6.63757 2.26257C6.125 2.77512 6.125 3.60008 6.125 5.25V19.25" stroke="#5D6150" stroke-width="1.3125"/>
    <path d="M18.375 19.25V10.0625C18.375 8.83356 18.375 8.21915 18.08 7.77775C17.9524 7.58667 17.7883 7.42261 17.5972 7.29493C17.1559 7 16.5414 7 15.3125 7" stroke="#5D6150" stroke-width="1.3125"/>
    <path d="M2.625 19.25V10.0625C2.625 8.83356 2.625 8.21915 2.91993 7.77775C3.04761 7.58667 3.21167 7.42261 3.40275 7.29493C3.84415 7 4.4586 7 5.6875 7" stroke="#5D6150" stroke-width="1.3125"/>
    <path d="M10.5 19.25V16.625" stroke="#5D6150" stroke-width="1.3125" stroke-linecap="round"/>
    <path d="M8.75 4.375H12.25" stroke="#5D6150" stroke-width="1.3125" stroke-linecap="round"/>
    <path d="M8.75 7H12.25" stroke="#5D6150" stroke-width="1.3125" stroke-linecap="round"/>
    <path d="M8.75 9.625H12.25" stroke="#5D6150" stroke-width="1.3125" stroke-linecap="round"/>
    <path d="M8.75 12.25H12.25" stroke="#5D6150" stroke-width="1.3125" stroke-linecap="round"/>
                    </svg>
                    <span className='text-limegray '>Engineering</span>
                  </div>
                  <div>
                    <ul className='text-white flex gap-[1.75rem]'>
                      <li className='textLimegray'>324 employees</li>
                      <li className='list-disc marker:text-lemongreen textLimegray'>95% attendance</li>
                    </ul>
                  </div>
                </div>
                {/* progressBar*/}
                <div className='h-[5px] w-full bg-white rounded-[30px]'></div>
              </div>
              
            </div>
          </div>
        </div>
        




        {/* secondSection */}
        <div className='w-[30.375rem] center-center h-[49.9375rem] bg-[rgba(190,229,50,0.05)] rounded-3xl font-semibold'>
          <div className='space-y-[5.125rem] w-[24.1875rem]' >
            <div className='flex flex-col  gap-[3.0625rem] h-[22.3125rem]'>
              {/* activitiesContainer */}
              <div className='between'>
                <div>
                  <h1 className='text-white text-xl'>Recent Activities</h1>
                  <h4 className='text-accountColor text-[15px] font-medium'>Latest updates and notifications</h4>
                </div>
                <button type="button" className='text-lemongreen font-medium self-end text-[15px]'>See more</button>
              </div>

              {/* notificationContainer */}
              <div className='flex flex-col gap-[0.9375rem] scrollBarDash overflow-y-auto'>

                <div className='cardActivitydash'>
                  <div>
                    <svg width="21" height="21" viewBox="0 0 21 21" fill="none" xmlns="http://www.w3.org/2000/svg">
                    <path d="M8.75 6.125H1.75" stroke="#DFDFDF" stroke-width="1.3125" stroke-linecap="round"/>
                    <path d="M7 10.5H1.75" stroke="#DFDFDF" stroke-width="1.3125" stroke-linecap="round"/>
                    <path d="M8.75 14.875H1.75" stroke="#DFDFDF" stroke-width="1.3125" stroke-linecap="round"/>
                    <path d="M14.875 14.875C17.2912 14.875 19.25 12.9162 19.25 10.5C19.25 8.08375 17.2912 6.125 14.875 6.125C12.4588 6.125 10.5 8.08375 10.5 10.5C10.5 12.9162 12.4588 14.875 14.875 14.875Z" stroke="#DFDFDF" stroke-width="1.3125"/>
                    <path d="M14.875 8.75V10.3654L15.75 11.375" stroke="#DFDFDF" stroke-width="1.3125" stroke-linecap="round" stroke-linejoin="round"/>
                    </svg>
                  </div>
                  <div className='flex flex-col'>
                    <span className='text-accountColor'> New employee John Doe joined Marketing</span>
                    <span className='text-white text-sm font-normal'>1 day ago</span>
                  </div>
                </div>
                <div className='cardActivitydash'>
                  <div>
                    <svg width="21" height="21" viewBox="0 0 21 21" fill="none" xmlns="http://www.w3.org/2000/svg">
                    <path d="M8.75 6.125H1.75" stroke="#DFDFDF" stroke-width="1.3125" stroke-linecap="round"/>
                    <path d="M7 10.5H1.75" stroke="#DFDFDF" stroke-width="1.3125" stroke-linecap="round"/>
                    <path d="M8.75 14.875H1.75" stroke="#DFDFDF" stroke-width="1.3125" stroke-linecap="round"/>
                    <path d="M14.875 14.875C17.2912 14.875 19.25 12.9162 19.25 10.5C19.25 8.08375 17.2912 6.125 14.875 6.125C12.4588 6.125 10.5 8.08375 10.5 10.5C10.5 12.9162 12.4588 14.875 14.875 14.875Z" stroke="#DFDFDF" stroke-width="1.3125"/>
                    <path d="M14.875 8.75V10.3654L15.75 11.375" stroke="#DFDFDF" stroke-width="1.3125" stroke-linecap="round" stroke-linejoin="round"/>
                    </svg>
                  </div>
                  <div className='flex flex-col'>
                    <span className='text-accountColor'> New employee John Doe joined Marketing</span>
                    <span className='text-white text-sm font-normal'>1 day ago</span>
                  </div>
                </div>
                
              </div>
            </div>

            {/* SecurtiySection */}
            <div className='flex flex-col  gap-[3.0625rem] h-[16.875rem]' >
              <div className='between'>
                <div>
                  <h1 className='text-white text-xl'>System Alerts</h1>
                  <h4 className='text-accountColor text-[15px] font-medium'>Critical System Notification</h4>
                </div>
                <button type="button" className='text-lemongreen font-medium self-end text-[15px]'>See more</button>
              </div>
              {/* notificationContainer */}
              <div className='flex flex-col gap-[0.9375rem]  scrollBarDash overflow-y-auto'>
                {/* activitiesContainer */}
                <div className='cardActivitydash'>
                  <div>
                    <svg width="21" height="21" viewBox="0 0 21 21" fill="none" xmlns="http://www.w3.org/2000/svg">
                    <path d="M1.75 10.5C1.75 6.37521 1.75 4.31281 3.03141 3.03141C4.31281 1.75 6.37521 1.75 10.5 1.75C14.6248 1.75 16.6872 1.75 17.9686 3.03141C19.25 4.31281 19.25 6.37521 19.25 10.5C19.25 14.6248 19.25 16.6872 17.9686 17.9686C16.6872 19.25 14.6248 19.25 10.5 19.25C6.37521 19.25 4.31281 19.25 3.03141 17.9686C1.75 16.6872 1.75 14.6248 1.75 10.5Z" stroke="#DFDFDF" stroke-width="1.5"/>
                    <path d="M6.125 12.25L7.69728 10.3632C8.32032 9.61564 8.63184 9.24175 9.04164 9.24175C9.45149 9.24175 9.76299 9.61564 10.3861 10.3632L10.6139 10.6368C11.237 11.3844 11.5485 11.7582 11.9584 11.7582C12.3681 11.7582 12.6797 11.3844 13.3027 10.6368L14.875 8.75" stroke="#DFDFDF" stroke-width="1.5" stroke-linecap="round"/>
                    </svg>
                  </div>
                  <div className='flex flex-col '>
                    <span className='text-accountColor text-nowrap'> Performance Review Deadline</span>
                    <span className='text-white text-sm font-normal'>Q2 reviews due in 5 days</span>
                  </div>
                </div>
                <div className='cardActivitydash'>
                  <div>
                    <svg width="21" height="21" viewBox="0 0 21 21" fill="none" xmlns="http://www.w3.org/2000/svg">
                    <path d="M1.75 10.5C1.75 6.37521 1.75 4.31281 3.03141 3.03141C4.31281 1.75 6.37521 1.75 10.5 1.75C14.6248 1.75 16.6872 1.75 17.9686 3.03141C19.25 4.31281 19.25 6.37521 19.25 10.5C19.25 14.6248 19.25 16.6872 17.9686 17.9686C16.6872 19.25 14.6248 19.25 10.5 19.25C6.37521 19.25 4.31281 19.25 3.03141 17.9686C1.75 16.6872 1.75 14.6248 1.75 10.5Z" stroke="#DFDFDF" stroke-width="1.5"/>
                    <path d="M6.125 12.25L7.69728 10.3632C8.32032 9.61564 8.63184 9.24175 9.04164 9.24175C9.45149 9.24175 9.76299 9.61564 10.3861 10.3632L10.6139 10.6368C11.237 11.3844 11.5485 11.7582 11.9584 11.7582C12.3681 11.7582 12.6797 11.3844 13.3027 10.6368L14.875 8.75" stroke="#DFDFDF" stroke-width="1.5" stroke-linecap="round"/>
                    </svg>
                  </div>
                  <div className='flex flex-col '>
                    <span className='text-accountColor text-nowrap'> Performance Review Deadline</span>
                    <span className='text-white text-sm font-normal'>Q2 reviews due in 5 days</span>
                  </div>
                </div>
                                <div className='cardActivitydash'>
                  <div>
                    <svg width="21" height="21" viewBox="0 0 21 21" fill="none" xmlns="http://www.w3.org/2000/svg">
                    <path d="M1.75 10.5C1.75 6.37521 1.75 4.31281 3.03141 3.03141C4.31281 1.75 6.37521 1.75 10.5 1.75C14.6248 1.75 16.6872 1.75 17.9686 3.03141C19.25 4.31281 19.25 6.37521 19.25 10.5C19.25 14.6248 19.25 16.6872 17.9686 17.9686C16.6872 19.25 14.6248 19.25 10.5 19.25C6.37521 19.25 4.31281 19.25 3.03141 17.9686C1.75 16.6872 1.75 14.6248 1.75 10.5Z" stroke="#DFDFDF" stroke-width="1.5"/>
                    <path d="M6.125 12.25L7.69728 10.3632C8.32032 9.61564 8.63184 9.24175 9.04164 9.24175C9.45149 9.24175 9.76299 9.61564 10.3861 10.3632L10.6139 10.6368C11.237 11.3844 11.5485 11.7582 11.9584 11.7582C12.3681 11.7582 12.6797 11.3844 13.3027 10.6368L14.875 8.75" stroke="#DFDFDF" stroke-width="1.5" stroke-linecap="round"/>
                    </svg>
                  </div>
                  <div className='flex flex-col '>
                    <span className='text-accountColor text-nowrap'> Performance Review Deadline</span>
                    <span className='text-white text-sm font-normal'>Q2 reviews due in 5 days</span>
                  </div>
                </div>
                <div className='cardActivitydash'>
                  <div>
                    <svg width="21" height="21" viewBox="0 0 21 21" fill="none" xmlns="http://www.w3.org/2000/svg">
                    <path d="M1.75 10.5C1.75 6.37521 1.75 4.31281 3.03141 3.03141C4.31281 1.75 6.37521 1.75 10.5 1.75C14.6248 1.75 16.6872 1.75 17.9686 3.03141C19.25 4.31281 19.25 6.37521 19.25 10.5C19.25 14.6248 19.25 16.6872 17.9686 17.9686C16.6872 19.25 14.6248 19.25 10.5 19.25C6.37521 19.25 4.31281 19.25 3.03141 17.9686C1.75 16.6872 1.75 14.6248 1.75 10.5Z" stroke="#DFDFDF" stroke-width="1.5"/>
                    <path d="M6.125 12.25L7.69728 10.3632C8.32032 9.61564 8.63184 9.24175 9.04164 9.24175C9.45149 9.24175 9.76299 9.61564 10.3861 10.3632L10.6139 10.6368C11.237 11.3844 11.5485 11.7582 11.9584 11.7582C12.3681 11.7582 12.6797 11.3844 13.3027 10.6368L14.875 8.75" stroke="#DFDFDF" stroke-width="1.5" stroke-linecap="round"/>
                    </svg>
                  </div>
                  <div className='flex flex-col leading-4 gap-[4px] '>
                    <span className='text-accountColor text-nowrap'> Performance Review Deadline</span>
                    <span className='text-white text-sm font-normal'>Q2 reviews due in 5 days</span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>  
    </>
  )
}

export default Dashboard