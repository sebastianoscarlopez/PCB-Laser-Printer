/*
 * printer.h
 *
 *  Created on: 8 ene. 2018
 *      Author: sebas
 */

#ifndef PRESENTER_H_
#define PRESENTER_H_

#endif /* PRESENTER_H_ */

#include <string.h>
#include <stdio.h>
#include <stdlib.h>
#include "stm32f3xx_hal.h"
#include "helper/convertHelper.h"
#include "helper/communicationHelper.h"
#include "printer/config.h"
#include "printer/driver.h"


void printWait();

void printStart();

void sendUnit();

void getSize();
