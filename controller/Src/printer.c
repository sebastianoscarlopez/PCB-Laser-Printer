/*
 * printer.c
 *
 *  Created on: 8 ene. 2018
 *      Author: sebas
 */

#include "printer.h"

void waitPrintig(){
	int8_t isWaiting = 1;
	while(isWaiting){
		  uartStartReceive();
		  while(!uartIsReady());
	}
}

void startPrint();

void sendUnit();

void getSize();
