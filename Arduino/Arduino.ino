#include <Servo.h>

String orientationRaw;
Servo servoX;
Servo servoY;
Servo servoZ;

void setup() {
    Serial.begin(9600);
    Serial.println("Waiting for computer to send a signal...\n");
    servoX.attach(4);
    servoY.attach(5);
    servoZ.attach(6);
}

//http://stackoverflow.com/questions/9072320/split-string-into-string-array
// Courtesy of alvarob
String getValue(String data, char separator, int index)
{
  int found = 0;
  int strIndex[] = {0, -1};
  int maxIndex = data.length()-1;

  for(int i=0; i<=maxIndex && found<=index; i++){
    if(data.charAt(i)==separator || i==maxIndex){
        found++;
        strIndex[0] = strIndex[1]+1;
        strIndex[1] = (i == maxIndex) ? i+1 : i;
    }
  }

  return found>index ? data.substring(strIndex[0], strIndex[1]) : "";
}

void loop() {
    while (Serial.available() > 0)
    {
        char data = Serial.read();
        orientationRaw += data; 

        // Process message when new line character is recieved
        if (data == '\n')
        {
            int x = getValue(orientationRaw, '|', 0).toInt();
            int y = getValue(orientationRaw, '|', 1).toInt();
            int z = getValue(orientationRaw, '|', 2).toInt();
            
            //Orientation will need to be remapped here, as Rift probably doesn't go from 0 to 180.
            //Most likely something along the lines of -180 to 180
            
            servoX.write(x);
            servoY.write(y);
            servoZ.write(z);
            
            Serial.print("X:");
            Serial.print(x);
            Serial.print(" ");
            Serial.print("Y:");
            Serial.print(y);
            Serial.print(" ");
            Serial.print("Z:");
            Serial.print(z);
            Serial.print(" ");
            
            

            orientationRaw = ""; // Clear recieved buffer
        }
    }
}
