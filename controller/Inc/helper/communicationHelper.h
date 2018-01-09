/*
 * communicationHelper.h
 *
 *  Created on: 8 ene. 2018
 *      Author: sebas
 */

#ifndef COMUNICATIONHELPER_H_
#define COMUNICATIONHELPER_H_

#define USE_BCC 1 // 1 Use Block Check Character, 0 don't use it, ever must be zero

#define CHARACTER_END 10

#define MAX_MESSAGE 200

#endif /* COMUNICATIONHELPER_H_ */

#include "stm32f3xx_hal.h"
#include <string.h>

char* uartWaitReceive();
void uartWaitSend(char* data);
uint8_t getBCC(char* data);
void waitReady();
