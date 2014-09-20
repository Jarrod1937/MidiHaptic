#include <string.h>
#include <stdlib.h>

// Serial string buffer
String inData;

// Pin assignments
int fingerpins[] = {40,41,42,43,44,45,46,47,48,49};


void setup()
{
    Serial.begin(9600);
    setoutput();
}


void loop()
{
    while (Serial.available() > 0)
    {
        char recieved = Serial.read();
        inData += recieved;

        // Process message when '+' character is recieved
        if (recieved == '+')
        {
            serialprocess(inData);
            delay(100);
            setlow();
            inData = "";
        }
    }
}

void setoutput()
{
  
  int ix;
  int arrsize = (sizeof(fingerpins)/sizeof(int));
  
  // Sets all pins in array to OUTPUT
  for(ix=0; ix < arrsize; ++ix)
  {
    pinMode(fingerpins[ix], OUTPUT);
  }
  
}

void hapticactivate(int pintohigh)
{
    // Write pin to high, offset pintohigh var by 1 for 0 based indexing
    digitalWrite(fingerpins[(pintohigh - 1)], HIGH);
}

void setlow()
{
  
  int ix;
  int arrsize = (sizeof(fingerpins)/sizeof(int));
  
  // Resets all pins in array to LOW state
  for(ix=0; ix < arrsize; ++ix)
  {
   digitalWrite(fingerpins[ix], LOW);
  }
  
}

void serialprocess(String input)
{
  // Max is 14 * 2 = 28 + 1 (for '+' terminator) = 29 + 1 (for conversion NULL char terminator) = 30
  char charBuf[30];
  char charBuftwo[30];
  input.toCharArray(charBuf, 30);
  
  int i;
  int lenofstr = strlen(charBuf);
  for(i = 0; i < lenofstr; i++)
  {
    
    if(charBuf[i] == '+')
    {
       break;
    }
    
    // Offsets of current i var
    int ig = i + 1;
    int il = i - 1;
    
    // Modulo of 0 means current value is a pair
    if((ig) % 2 == 0)
    {
      
       // Temp substring buffer to pass into strtol which takes a subsring of 2
       // and converts hex to int equiv
       char subbuff[3];
       strcpy (charBuftwo,charBuf);
       memcpy( subbuff, &charBuftwo[(il)], 2);
       subbuff[3] = '\0';
        
       hapticactivate((int)strtol(&subbuff[0], NULL, 16));
    }
  }
}
