/*
 * driver.c
 *
 *  Created on: 11 ene. 2018
 *      Author: sebas
 */
#include "printer/driver.h"

uint16_t driverGetMotorRevolutionAverage;

void DriverON()
{
	// TODO: DriverON
}

/// A motor revolution in microseconds
void DriverCalculateMotorRevolutionAverage()
{
	// TODO: DriverCalculateMotorRevolutionAverage
	driverGetMotorRevolutionAverage = 24000;
}

uint16_t DriverGetMotorRevolutionAverage()
{
	return driverGetMotorRevolutionAverage;
}
