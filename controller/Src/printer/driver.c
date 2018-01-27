/*
 * driver.c
 *
 *  Created on: 11 ene. 2018
 *      Author: sebas
 */
#include "printer/driver.h"

uint16_t driverGetMotorRevolutionAverage;
uint32_t capture[20];
uint16_t i = 0;

void HAL_TIM_IC_CaptureCallback(TIM_HandleTypeDef *htim)
{
	/*
	capture[i++] = __HAL_TIM_GET_COMPARE(&htim2, TIM_CHANNEL_2);
//	capture[i++] = __HAL_TIM_GET_COUNTER(&htim2);
	if(i==20)
	{
		i = 0;
	}
	*/
}

void DriverON()
{
	// TODO: DriverON
	HAL_TIM_PWM_Start(&htim3, TIM_CHANNEL_1);
	HAL_TIM_IC_Start_DMA(&htim2, TIM_CHANNEL_1, (uint32_t*)capture, 10);
}

/// A motor revolution in microseconds
void DriverCalculateMotorRevolutionAverage()
{
	// TODO: DriverCalculateMotorRevolutionAverage
	driverGetMotorRevolutionAverage = (double)capture[0]/1000000.0 * controllerUnit;
}

uint16_t DriverGetMotorRevolutionAverage()
{
	return driverGetMotorRevolutionAverage;
}
