/*
 * printer.c
 *
 *  Created on: 8 ene. 2018
 *      Author: sebas
 */

#include "printer/controller.h"

void printWait(){
	uint8_t isWaiting = 1;
	char* data;

	while(isWaiting){
		data = uartWaitReceive();
		isWaiting = strcmp("printStart", data);
	}

	printStart();
}

void printStart(){
	char str[50];

	DriverON();

	sprintf(str, "Controller Unit:%u", controllerUnit);
	uartWaitSend(str);

	DriverCalculateMotorRevolutionAverage();
	uint16_t revolutionAVG = DriverGetMotorRevolutionAverage();
	sprintf(str, "MotorRevolutionAverage:%u", revolutionAVG);
	uartWaitSend(str);

	char* data;
	data = uartWaitReceive();
	uint16_t width, height;
	strToUint16_t(data, &width);
	data = uartWaitReceive();
	strToUint16_t(data, &height);
	setWidth(width);
	setHeight(height);

	uint16_t rowIdx = width, columnIdx = 0; // Start in left upper corner
	do{

	}while(rowIdx > 0);
}

void sendUnit()
{
	//TODO
}

void getSize()
{
	//TODO
}
