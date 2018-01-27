/*
 * config.c
 *
 *  Created on: 9 ene. 2018
 *      Author: sebas
 */

#include "printer/config.h"

uint16_t controllerUnit = 6944; // 1/144Mhz in picoseconds
uint16_t mirrors = 2;
uint16_t _width;
uint16_t _height;

/// Board width in controllerUnit
void setWidth(uint16_t width){
	_width = width;
}

/// Board height in controllerUnit
void setHeight(uint16_t height)
{
	_height = height;
}


