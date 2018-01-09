/*
 * communicationHelper.c
 *
 *  Created on: 8 ene. 2018
 *      Author: sebas
 */
#include "helper/communicationHelper.h"
#include "usart.h"

char bufferAux[MAX_MESSAGE];

/// Wait to receive data and it's returned.
char* uartWaitReceive()
{
	uint8_t receiveOk = 0;
	char* data;
	size_t len;
	do
	{
		uartStartReceive();
		waitReady();
		data = uartGetData();

		uint8_t bcc = getBCC(data);
		len = strlen((char*)&data[1]);
		receiveOk = bcc == data[0] && data[len] == CHARACTER_END;
 		uartTransmit(receiveOk ? "OK": "FAILED", bcc);
	}while(!receiveOk);

	data[len] = '\0';
	return &data[1];
}

/// Send data, return only when data will be transferred.
void uartWaitSend(char* data){
	uint8_t sendOk;
	size_t len = strlen(data);

	strcpy((char*)bufferAux, data);
	bufferAux[len] = CHARACTER_END;
	bufferAux[len + 1] = '\0';
	do
	{
		waitReady();
		uint8_t bcc = getBCC(bufferAux);
		uartTransmit(data, bcc);
		waitReady();
		uartStartReceive();

		char* dataConfirm = uartGetData();
		sendOk = dataConfirm[0] == bcc && strcmp((char*)&dataConfirm[1], "OK");
	}while(!sendOk);
}


/// Calculate block check character
uint8_t getBCC(char* data){
	uint8_t dv = 0;
	size_t len = strlen(data) - 2; // BCC and CHARACTER_END
	if(USE_BCC == 0 || len < 3 || len > MAX_MESSAGE){
		return dv;
	}
	dv = 0;
	for(uint16_t i = 1; i < len; i++)
	{
		dv ^= (uint8_t)data[i];
	}
	return dv;
}

/// Wait for idle communication
void waitReady(){
	while(!uartIsReady()){
		HAL_Delay(1);
	}
}
