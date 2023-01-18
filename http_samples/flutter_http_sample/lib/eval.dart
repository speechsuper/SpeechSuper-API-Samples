import 'dart:convert';
import 'dart:typed_data';

import 'package:flutter/services.dart' show rootBundle;
import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import 'package:crypto/crypto.dart';

class EvalScreen extends StatefulWidget {
  const EvalScreen({Key? key}) : super(key: key);

  @override
  State<EvalScreen> createState() => _EvalScreenState();
}

class _EvalScreenState extends State<EvalScreen> {
  final String appKey = "Insert your appKey here";
  final String secretKey = "Insert your secretKey here";
  final String userId = "uid";
  final String baseHOST = "api.speechsuper.com";

  final String coreType = "word.eval"; // Change the coreType according to your needs.
  final refText = "supermarket"; // Change the reference text according to your needs.
  final audioPath = "assets/supermarket.wav"; // Change the audio path corresponding to the reference text.
  final audioType = "wav"; // Change the audio type corresponding to the audio file.
  final audioSampleRate = "16000";

  TextEditingController resultController = new TextEditingController();

  @override
  Widget build(BuildContext context) {
    return Scaffold(
        body: Center(
            child: Column(
              mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  TextField(
                    keyboardType: TextInputType.multiline,
                    maxLength: null,
                    minLines: 6,
                    maxLines: 22,
                    readOnly: true,
                    controller: resultController,
                    decoration: InputDecoration(hintText: "Result:", contentPadding: const EdgeInsets.symmetric(vertical: 20.0),),
                  ),
                  SizedBox(height: 30.0,),
                  TextButton(
                      onPressed: doEval,
                      style: ButtonStyle(
                        overlayColor: MaterialStateProperty.all<Color>(Colors.transparent),//splashColor
                        backgroundColor: MaterialStateProperty.all<Color>(Color(0xFFFFC800)),
                        elevation: MaterialStateProperty.all<double>(0),
                      ),
                      child: Text("Click me")
                  )
                ],
            )
        )
    );
  }

  void doEval() {
    String timestamp =  DateTime.now().millisecondsSinceEpoch.toString();
    String connectSig = sha1.convert(utf8.encode("${appKey}${timestamp}${secretKey}")).toString();
    String startSig = sha1.convert(utf8.encode("${appKey}${timestamp}${userId}${secretKey}")).toString();
    String tokenId = DateTime.now().millisecondsSinceEpoch.toString();
    var params = {
      "connect": {
        "cmd": "connect",
        "param": {
           "sdk": {
             "version": 16777472,
             "source": 9,
             "protocol": 2
           },
          "app": {
            "applicationId": appKey,
            "sig": connectSig,
            "timestamp": timestamp
          }
        }
      },
      "start": {
        "cmd": "start",
        "param": {
          "app": {
            "applicationId": appKey,
            "sig": startSig,
            "userId": userId,
            "timestamp": timestamp
          },
          "audio": {
            "audioType": audioType,
            "sampleRate": audioSampleRate,
            "channel": 1,
            "sampleBytes":2
          },
          "request": {
            "refText": refText,
            "tokenId": tokenId
          }
        }
      }
    };

    rootBundle.load(audioPath).then((ByteData data) async {
      var url = Uri.https(baseHOST, coreType);
      var request = http.MultipartRequest("POST", url)
        ..fields["text"] = jsonEncode(params)
        ..files.add(http.MultipartFile.fromBytes("audio", data.buffer.asUint8List()))
        ..headers["Request-Index"] = "0";

      var response = await request.send();
      if(response.statusCode != 200) {
        resultController.text = "HTTP status code ${response.statusCode}";
      } else {
         response.stream.transform(utf8.decoder).join().then((String str) {
           if(str.contains("error")) {
             resultController.text = str;
           } else {
             var respJson = jsonDecode(str);
             resultController.text = "overall: ${respJson["result"]["overall"]}";
           }
         });
      }
    });


  }

}
