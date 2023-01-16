
import {createHash} from 'crypto';
import * as fs from 'fs/promises';
import FormData from 'form-data';

const appKey = "Insert your appKey here";
const secretKey = "Insert your secretKey here";
const userId = "uid";

const baseHOST = "api.speechsuper.com";

const coreType = "word.eval"; // Change the coreType according to your needs.
const refText = "supermarket"; // Change the reference text according to your needs.
const audioPath = "supermarket.wav"; // Change the audio path corresponding to the reference text.
const audioType = "wav"; // Change the audio type corresponding to the audio file.
const audioSampleRate = "16000";

async function doEval(userId, audioType, sampleRate, requestParams, audioPath) {
    const coreType = requestParams['coreType'];
	
    let encrypt = function(content) {
        let hash = createHash("sha1");
        hash.update(content);
        return hash.digest('hex');
    }

	let getConnectSig = function () {
		var timestamp = new Date().getTime().toString();
		var sig = encrypt(appKey + timestamp + secretKey);
		return { sig: sig, timestamp: timestamp };
	}
	let getStartSig = function () {
		var timestamp = new Date().getTime().toString();
		var sig = encrypt(appKey + timestamp + userId + secretKey);
		return { sig: sig, timestamp: timestamp, userId: userId };
	}
	let createUUID = (function (uuidRegEx, uuidReplacer) {
		return function () {
			return "xxxxxxxxxxxx4xxxyxxxxxxxxxxxxxxx".replace(uuidRegEx, uuidReplacer).toUpperCase();
		};
	})(/[xy]/g, function (c) {
		let r = Math.random() * 16 | 0,
			v = c == "x" ? r : (r & 3 | 8);
		return v.toString(16);
	});
	let connectSig = getConnectSig();
	let startSig = getStartSig();
    requestParams['tokenId'] = requestParams['tokenId'] || createUUID()
	let params = {
		connect: {
			cmd: "connect",
			param: {
				sdk: {
					version: 16777472,
					source: 9,
					protocol: 2
				},
				app: {
					applicationId: appKey,
					sig: connectSig.sig,
					timestamp: connectSig.timestamp
				}
			}
		},
		start: {
			cmd: "start",
			param: {
				app: {
					applicationId: appKey,
					sig: startSig.sig,
					userId: startSig.userId,
					timestamp: startSig.timestamp
				},
				audio: {
					audioType: audioType,
					sampleRate: sampleRate,
					channel: 1,
					sampleBytes: 2
				},
				request: requestParams
			}
		}
	};
    return new Promise((resolve, reject) => {
        fs.readFile(audioPath)
        .then(audioData=> {
            let fd = new FormData();
            fd.append("text", JSON.stringify(params));
            fd.append("audio", audioData);
            let options = {
                host: baseHOST,
                path: "/" + coreType,
                method: "POST",
                protocol: "https:",
                headers: {"Request-Index": "0"}
            };

            try{
                const req = fd.submit(options, (err, res) => {
                    if(err){
                        return reject(new Error(err.message));
                    }
                    if(res.statusCode < 200 || res.statusCode > 299) {
                        return reject(new Error(`HTTP status code ${res.statusCode}`));
                    }
                    const body = [];
                    res.on('data', (chunk) => body.push(chunk));
                    res.on('end', ()=>{
                        const resString = Buffer.concat(body).toString();
                        resolve(resString);
                    });
                })
            }catch(e){
                reject(e);
            };
        }).catch(e=> {
            reject(e);
        })
    });
}


const requestParams = {
    coreType: coreType,
    refText: refText
};

doEval(userId, audioType, audioSampleRate, requestParams, audioPath)
.then(data=>{console.log(data)})
.catch(e=>{console.log(e)});