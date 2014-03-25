#include <Servo.h>

String orientationRaw;
Servo servoPitch;
Servo servoYaw;
Servo servoRoll;

//Set pin constants
const int servoPitchPin = 5;
const int servoYawPin = 9;
const int servoRollPin = 11;

void setup() {
    Serial.begin(9600);
    //debug
    Serial.println("Waiting for computer to send a signal...\n");
    
    //Attach servos to pins - can be altered
    servoPitch.attach(servoPitchPin, 710, 2450);
    servoYaw.attach(servoYawPin, 710, 2450);
    servoRoll.attach(servoRollPin, 710, 2450
    );
    
    //Set initial neutral position
    servoPitch.write(90);
    servoYaw.write(90);
    servoRoll.write(90);
}

//String splitter function. Returns a string separated by a separator by its index.
//http://stackoverflow.com/questions/9072320/split-string-into-string-array
//Courtesy of alvarob
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

//Function for squishing values over 180 or under 0 (servo limits)
int restrictEulerAngle(int angle, int minAngle, int maxAngle)
{
  if (angle < minAngle)
  {
    angle = minAngle;
  }
  else if (angle > maxAngle)
  {
    angle = maxAngle;
  }
  
  return angle;
}

void loop() {
    while (Serial.available() > 0)
    {
        char data = Serial.read();
        orientationRaw += data; 

        // Process message when new line character is recieved
        if (data == '\n')
        {
            int pitchRaw = getValue(orientationRaw, '|', 0).toInt();
            int yawRaw = getValue(orientationRaw, '|', 1).toInt();
            int rollRaw = getValue(orientationRaw, '|', 2).toInt();
            
            //Orientation will need to be remapped here, as Rift probably doesn't go from 0 to 180.
            //Most likely something along the lines of -180 to 180
            //Doing this will result in accelerated movements of the servo rig,
            //which may result in motion sickness. We really don't want this.
            /*
            pitch = map(pitchRaw, -180, 180, 0, 180)
            yaw = map(yawRaw, -180, 180, 0, 180)
            roll = map(rollRaw, -180, 180, 0, 180
            */
            
            //Could also simply squish data beyond 180 and 0. This will result in data loss, 
            //but may be the simplest solution for avoiding motion sickness.            
            /*
            int pitch = restrictEulerAngle(pitchRaw, 0, 180);
            int yaw = restrictEulerAngle(yawRaw, 0, 180);
            int roll = restrictEulerAngle(rollRaw, 0, 180);
            */
            
            servoPitch.write(pitchRaw);
            servoYaw.write(yawRaw);
            servoRoll.write(rollRaw);
            
            /*
            
            servoX.write(pitch);
            servoY.write(yaw);
            servoZ.write(roll);
            
            */
            
            //debug
            Serial.print("Pitch:");
            Serial.print(pitchRaw);
            Serial.print(" ");
            Serial.print("Yaw:");
            Serial.print(yawRaw);
            Serial.print(" ");
            Serial.print("Roll:");
            Serial.print(rollRaw);
            Serial.println(" ");
            
            

            orientationRaw = ""; // Clear recieved buffer
            delay(100);
        }
    }
}
