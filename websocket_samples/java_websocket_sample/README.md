# Quick Start
- Modify the appKey in src\main\java\com\speechsuper\Sample.java
- Modify the secretKey in src\main\java\com\speechsuper\Sample.java
- Change the coreType according to your needs.
- Change the reference text according to your needs.
- Change the audio path corresponding to the reference text.
- Change the audio type corresponding to the audio file.
 
# Requirements
- Java JDK 1.7+
- Maven 

# Command lines
- Compile:
    ```
    mvn clean compile
    ```
- Run the `Sample` class:
   ```
   mvn compile exec:java -Dexec.mainClass="com.speechsuper.Sample"
   ```