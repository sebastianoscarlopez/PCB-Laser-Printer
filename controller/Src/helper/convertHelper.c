/*
 * convertHelper.c
 *
 *  Created on: 12 ene. 2018
 *      Author: sebas
 */
#include "helper/convertHelper.h"

void strToUint16_t(char* text, uint16_t* value)
{
	uint32_t _pow = 1;
	uint16_t len = strlen(text);
	*value = 0;
	for(int16_t i = len - 1; i >= 0; i--)
	{
		*value += _pow * (text[i] - 48);
		_pow *= 10;
	}
}
