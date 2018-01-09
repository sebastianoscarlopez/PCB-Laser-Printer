/*
 * printer.c
 *
 *  Created on: 8 ene. 2018
 *      Author: sebas
 */

#include "printer.h"

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
	//TODO
}

void sendUnit()
{
	//TODO
}

void getSize()
{
	//TODO
}
