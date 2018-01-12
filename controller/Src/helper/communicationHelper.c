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
	volatile char* data;
	size_t lenData;
	do
	{
		uartStartReceive();
		waitReady();
		data = uartGetData();

		uint8_t bcc = getBCC(&data[1]);
		lenData = strlen((char*)&data[1]);
		receiveOk = bcc == data[0] && data[lenData] == CHARACTER_END;
		volatile char response[7];
		strcpy(response, receiveOk ? "OK": "FAILED");
		size_t lenResponse = strlen(response);
		response[lenResponse] = CHARACTER_END;
		response[lenResponse + 1] = '\0';
 		uartTransmit(response);
	}while(!receiveOk);

	data[lenData] = '\0';
	return &data[1];
}

/// Send data, return only when data will be transferred.
void uartWaitSend(char* data){
	uint8_t sendOk;
	size_t len = strlen(data);

	strcpy(&bufferAux[1], data);
	bufferAux[len + 1] = CHARACTER_END;
	bufferAux[len + 2] = '\0';
	do
	{
		waitReady();
		uint8_t bcc = getBCC(&bufferAux[1]);
		bufferAux[0] = (char)bcc;
		uartTransmit(bufferAux);
		waitReady();
		uartStartReceive();
		waitReady();
		char* dataConfirm = uartGetData();
		if(dataConfirm[2] == CHARACTER_END)
		{
			dataConfirm[2] = '\0';
		}
		sendOk = strcmp(dataConfirm, "OK") == 0;
	}while(!sendOk);
}


/// Calculate block check character
uint8_t getBCC(char* data){
	uint8_t dv = 0;
	size_t len = strlen(data);
	if(USE_BCC == 0 || len < 3 || len > MAX_MESSAGE){
		return dv;
	}
	dv = 0;
	for(uint16_t i = 0; i < len; i++)
	{
		dv ^= (uint8_t)data[i];
	}
	return dv;
}

/// Wait for idle communication
void waitReady(){
	do{
		HAL_Delay(100);
	}while(!uartIsReady());
}
