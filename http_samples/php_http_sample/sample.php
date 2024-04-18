<?php
$appKey = "Insert your appKey here";
$secretKey = "Insert your secretKey here";

$baseUrl = "https://api.speechsuper.com/";

$coreType = "word.eval.promax"; // Change the coreType according to your needs.
$refText = "supermarket"; // Change the reference text according to your needs.
$audioPath = "supermarket.wav"; // Change the audio path corresponding to the reference text.
$audioType = "wav"; // Change the audio type corresponding to the audio file.
$audioSampleRate = 16000;
$userId = "guest";
$ts = strtotime('now');
$conSig = sha1($appKey.$ts.$secretKey,false);
$startSig = sha1($appKey.$ts.$userId.$secretKey, false);
$ch = curl_init();
$headers = array('Request-Index:0');//Request-Index is always 0

$url = $baseUrl.$coreType;

//request param
$strData = array('text'=>
                    '
                    {
                        "connect": {
                            "cmd": "connect",
                            "param": {
                                "sdk": {
                                    "protocol": 2,
                                    "version": 16777472,
                                    "source": 9
                                },
                                "app": {
                                    "applicationId": "'.$appKey.'",
                                    "sig": "'. $conSig .'",
                                    "timestamp": "'. $ts.'"
                                }
                            }
                        },
                        "start": {
                            "cmd": "start",
                            "param": {
                                "app": {
                                    "applicationId": "'.$appKey.'",
                                    "timestamp": "'.$ts.'",
                                    "sig": "'.$startSig.'",
                                    "userId": "'.$userId.'"
                                },
                                "audio": {
                                    "channel": 1,
                                    "sampleBytes": 2,
                                    "sampleRate": '.$audioSampleRate.',
                                    "audioType": "'.$audioType.'"
                                },
                                "request": {
                                    "tokenId": "uuu",
                                    "coreType": "'.$coreType.'",
                                    "refText": "'.$refText.'"
                                }
                            }
                        }
                    } 
                    ',
                    'audio'=>file_get_contents(ltrim($audioPath)));
curl_setopt($ch, CURLOPT_URL, $url);
curl_setopt($ch, CURLOPT_CUSTOMREQUEST, "POST");
curl_setopt($ch, CURLOPT_SSL_VERIFYPEER, FALSE);
curl_setopt($ch, CURLOPT_SSL_VERIFYHOST, FALSE);
curl_setopt($ch, CURLOPT_FOLLOWLOCATION, 1);
curl_setopt($ch, CURLOPT_AUTOREFERER, 1);
curl_setopt($ch, CURLOPT_TIMEOUT_MS,30000);
curl_setopt($ch, CURLOPT_CONNECTTIMEOUT_MS, 30000);
curl_setopt($ch, CURLOPT_HTTPHEADER, $headers);
curl_setopt($ch, CURLOPT_POSTFIELDS, $strData);
curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
$output = curl_exec($ch);
if($output === false)
{
    echo 'Curl error: ' . curl_error($ch) . "\n";
}
curl_close($ch);
echo ($output);
