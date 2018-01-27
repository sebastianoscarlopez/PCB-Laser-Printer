/*
 * config.h
 *
 *  Created on: 9 ene. 2018
 *      Author: sebas
 */

#ifndef PRINTER_CONFIG_H_
#define PRINTER_CONFIG_H_
#ifdef __cplusplus
 extern "C" {
#endif

#include "stm32f3xx_hal.h"

extern uint16_t controllerUnit;
extern uint16_t mirrors;

uint16_t getControllerUnit();
void setWidth(uint16_t width);
void setHeight(uint16_t height);
uint16_t getMirrors(uint16_t mirrors);


#ifdef __cplusplus
}
#endif
#endif /* PRINTER_CONFIG_H_ */
