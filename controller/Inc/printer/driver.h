/*
 * driver.h
 *
 *  Created on: 11 ene. 2018
 *      Author: sebas
 */

#ifndef PRINTER_DRIVER_H_
#define PRINTER_DRIVER_H_



#endif /* PRINTER_DRIVER_H_ */

#include "stm32f3xx_hal.h"
#include "tim.h"
#include "printer/config.h"

void DriverON();
void DriverCalculateMotorRevolutionAverage();
uint16_t DriverGetMotorRevolutionAverage();
