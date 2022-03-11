#_*_encoding:utf-8_*_
import time
import json
import asyncio
import hashlib
import websockets

#global variable
baseUrl = "wss://api.speechsuper.com/"
appKey = "Insert your appKey here"
secretKey = "Insert your secretKey here"
coreType = "word.eval"
refText = "supermarket"
audioPath = "supermarket.wav"
audioType = "wav"
audioSampleRate = 16000

# Send authentication
async def initConnnct(websocket):
        t = time.time()
        timestamp = '%d' % int(round(t * 1000))
        connectStr = (appKey + timestamp + secretKey).encode("utf-8")
        connectSig = hashlib.sha1(connectStr).hexdigest()

        jconnect = {
            "cmd" : "connect",
            "param" : {
                "sdk": {
                    "version": 16777472,
                    "source": 4,
                    "protocol": 1
                },
                "app": {
                    "applicationId": appKey,
                    "sig": connectSig,
                    "timestamp": timestamp
                }
            }
        }

        param = json.dumps(jconnect)
        await websocket.send(param)
        return True

# Send evaluation request
async def startScore(websocket, audioPath, audioType, request):
        # Send start request
        userId ="userId"
        timestamp = '%d' % int(round(time.time() * 1000))
        startStr = (appKey + timestamp + userId + secretKey).encode("utf-8")
        startSig = hashlib.sha1(startStr).hexdigest()

        startObj = {
            "cmd": "start",
            "param": {
                "app": {
                    "userId": userId,
                    "applicationId": appKey,
                    "timestamp": timestamp,
                    "sig": startSig
                },
                "audio": {
                    "audioType": audioType,
                    "channel": 1,
                    "sampleBytes": 2,
                    "sampleRate": audioSampleRate
                },
                "request": request
            }
        }

        startObj["param"]["request"]["tokenId"] = "tokenId"

        startStr = json.dumps(startObj)
        await websocket.send(startStr)

        # Send audio data
        f = open(audioPath, "rb")
        while True:
            data = f.read(1024)
            if not data:
                break
            await websocket.send(data)
        f.close()

        # Send stop request
        empty_arry = {
            "cmd": "stop",
        }
        empty_arry2 = json.dumps(empty_arry)
        await websocket.send(empty_arry2)
    
        return True



async def main_logic():
    #Establish connection
    async with websockets.connect(baseUrl + coreType, ping_timeout = None) as websocket:
        
        await initConnnct(websocket)
      
        await startScore(websocket, audioPath, audioType, {"coreType":coreType,"refText":refText})

        # Return result
        try:
            async for message in websocket:
                print('message===>' + message)
        except websockets.exceptions.ConnectionClosedError:
            print('Close connection to websocket')
        

asyncio.get_event_loop().run_until_complete(main_logic())