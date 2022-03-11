# Quick Start
- Modify the appKey in src\main\java\com\speechsuper\Sample.java
- Modify the secretKey in src\main\java\com\speechsuper\Sample.java
 
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