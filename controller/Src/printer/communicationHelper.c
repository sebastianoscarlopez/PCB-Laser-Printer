/*
 * communicationHelper.c
 *
 *  Created on: 8 ene. 2018
 *      Author: sebas
 */
#include "helper/communicationHelper.h"
#include "usart.h"

uint8_t* uartWaitReceive()
{
	uint8_t receiveOk = 0;
	uint8_t* data;
	do
	{
		while(!uartIsReady());
		uartStartReceive();
		while(!uartIsReady());
		data = uartGetData();

		uint8_t bcc = getBCC(data);
		receiveOk = bcc == data[0];

		uartTransmit(receiveOk ? "OK": "FAILED", bcc);
	}while(!receiveOk);
	return &data[1];
}

void uartWaitSend(uint8_t* data){
	uint8_t sendOk;
	do
	{
		while(!uartIsReady());
		uint8_t bcc = getBCC(data);
		uartTransmit(data, bcc);
		while(!uartIsReady());
		uartStartReceive();

		uint8_t* dataConfirm = uartGetData();
		sendOk = dataConfirm[0] == bcc && strcmp(&dataConfirm[1], "OK");
	}while(!sendOk);
}


/// Calculate block check character
uint8_t getBCC(data){
	uint8_t dv = 0;
	size_t len = strlen(data) - 1;
	if(USE_BCC == 0 || len < 2 || len > MAX_MESSAGE){
		return dv;
	}
	dv = 0;
	for(uint16_t i = 1; i < len; i++)
	{
		dv ^= data[i];
	}
	return dv;
}
