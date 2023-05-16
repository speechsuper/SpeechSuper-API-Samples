#_*_encoding:utf-8_*_
import time
import hashlib
import requests
import json

appKey = "Insert your appKey here"
secretKey = "Insert your secretKey here"

baseURL = "https://api.speechsuper.com/"

timestamp = str(int(time.time()))

coreType = "word.eval" # Change the coreType according to your needs.
refText = "supermarket" # Change the reference text according to your needs.
audioPath = "supermarket.wav" # Change the audio path corresponding to the reference text.
audioType = "wav" # Change the audio type corresponding to the audio file.
audioSampleRate = 16000
userId = "guest"

url =  baseURL + coreType
connectStr = (appKey + timestamp + secretKey).encode("utf-8")
connectSig = hashlib.sha1(connectStr).hexdigest()
startStr = (appKey + timestamp + userId + secretKey).encode("utf-8")
startSig = hashlib.sha1(startStr).hexdigest()

params={
	"connect":{
		"cmd":"connect",
		"param":{
			"sdk":{
				"version":16777472,
				"source":9,
				"protocol":2
			},
			"app":{
				"applicationId":appKey,
				"sig":connectSig,
				"timestamp":timestamp
			}
		}
	},
	"start":{
		"cmd":"start",
		"param":{
			"app":{
				"userId":userId,
				"applicationId":appKey,
				"timestamp":timestamp,
				"sig":startSig
			},
			"audio":{
				"audioType":audioType,
				"channel":1,
				"sampleBytes":2,
				"sampleRate":audioSampleRate
			},
			"request":{
				"coreType":coreType,
				"refText":refText,
				"tokenId":"tokenId"
			}

		}
	}
}

datas=json.dumps(params)
data={'text':datas}
headers={"Request-Index":"0"}
files={"audio":open(audioPath,'rb')}
res=requests.post(url, data=data, headers=headers, files=files)
print(res.text.encode('utf-8', 'ignore').decode('utf-8'))
