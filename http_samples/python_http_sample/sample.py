#_*_encoding:utf-8_*_
import time
import hashlib
import requests
import json

appKey = "Insert your appKey here"
secretKey = "Insert your secretKey here"

timestamp = str(int(time.time()))

coreType = "word.eval"
url = "https://api.speechsuper.com/"+coreType

refText = "supermarket"
audioPath = "supermarket.wav"
audioType = "wav"
audioSampleRate = 16000
userId = "guest"
connectstr=(appKey + timestamp + secretKey).encode("utf-8")
connectsig=hashlib.sha1(connectstr).hexdigest()
startstr=(appKey + timestamp + userId + secretKey).encode("utf-8")
startsig=hashlib.sha1(startstr).hexdigest()

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
				"sig":connectsig,
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
				"sig":startsig
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
print(res.text.encode('gbk', 'ignore'))