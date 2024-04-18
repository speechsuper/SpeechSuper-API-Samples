package com.speechsuper;

import org.apache.commons.codec.binary.Hex;
import org.apache.commons.codec.digest.DigestUtils;
import org.java_websocket.client.WebSocketClient;
import org.java_websocket.handshake.ServerHandshake;

import java.io.IOException;
import java.net.URISyntaxException;
import java.security.MessageDigest;
import java.net.URI;
import java.util.ArrayList;
import java.io.FileInputStream;
import java.io.FileNotFoundException;

public class Sample {
    public static final String baseUrl = "wss://api.speechsuper.com/";
    public static final String appKey = "Insert your appKey here";
    public static final String secretKey = "Insert your secretKey here";

    public static void WebsocketAPI(String audioPath, String audioType, String audioSampleRate, String refText, String coreType) {
        String url = baseUrl + coreType;
		 //Randomly generate userId
        String userId = getRandomString(5);
        final ArrayList<Object> param = buildParam(appKey, secretKey, userId, audioType, audioSampleRate, refText, coreType);
        try {
            MyWebSocketClient client = new MyWebSocketClient(url, param, audioPath);
            client.connect();
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    private static ArrayList<Object> buildParam(String appkey, String secretKey, String userId, String audioType, String audioSampleRate, String refText, String coreType) {
        //SHA1 algorithm tool
		MessageDigest digest = DigestUtils.getSha1Digest();
		
		//connect timestamp
        long timeReqMillis = System.currentTimeMillis();
        String connectSigStr = appkey + timeReqMillis + secretKey;
        String connectSig = Hex.encodeHexString(digest.digest(connectSigStr.getBytes()));
        
		//start timestamp
        long timeStartMillis = System.currentTimeMillis();
        String startSigStr = appkey + timeStartMillis + userId + secretKey;
        String startSig = Hex.encodeHexString(digest.digest(startSigStr.getBytes()));
        
		//request param
        String connect = "{"
                + "\"cmd\":\"connect\","
                + "\"param\":{"
                + "\"sdk\":{"
                + "\"protocol\":2,"
                + "\"version\":16777472,"
                + "\"source\":9"
                + "},"
                + "\"app\":{"
                + "\"applicationId\":\"" + appkey + "\","
                + "\"sig\":\"" + connectSig + "\","
                + "\"timestamp\":\"" + timeReqMillis + "\""
                + "}"
                + "}"
                + "}";
        
        String start = "{"
                + "\"cmd\":\"start\","
                + "\"param\":{"
                + "\"app\":{"
                + "\"applicationId\":\"" + appkey + "\","
                + "\"timestamp\":\"" + timeStartMillis + "\","
                + "\"sig\":\"" + startSig + "\","
                + "\"userId\":\"" + userId + "\""
                + "},"
                + "\"audio\":{"
                + "\"sampleBytes\":2,"
                + "\"channel\":1,"
                + "\"sampleRate\":" + audioSampleRate + ","
                + "\"audioType\":\"mp3\""
                + "},"
                + "\"request\":{"
                + "\"tokenId\":\"sfasdfadsfasewqaf\","
                + "\"refText\":\"" + refText + "\","
                + "\"coreType\":\"" + coreType + "\""
                + "}"
                + "}"
                + "}";
        
        String stop = "{"
                + "\"cmd\":\"stop\""
                + "}";
        
        ArrayList<Object> params = new ArrayList<Object>();
        params.add(connect);
        params.add(start);
        params.add(stop);
        return params;
    }

    private static int getRandom(int count) {
        return (int) Math.round(Math.random() * (count));
    }

    private static String charString = "abcdefghijklmnopqrstuvwxyz123456789";

    private static String getRandomString(int length) {//Generate random string
        StringBuffer sb = new StringBuffer();
        int len = charString.length();
        for (int i = 0; i < length; i++) {
            sb.append(charString.charAt(getRandom(len - 1)));
        }
        return sb.toString();
    }

    public static void main(String[] args) {
        String coreType = "word.eval.promax"; // Change the coreType according to your needs.
        String refText = "supermarket"; // Change the reference text according to your needs.
        String audioPath = "supermarket.wav"; // Change the audio path corresponding to the reference text.
        String audioType = "wav"; // Change the audio type corresponding to the audio file.
        String audioSampleRate = "16000";
        
        WebsocketAPI(audioPath, audioType, audioSampleRate, refText, coreType);
       /*Analog blocking*/
        try {
            Thread.sleep(2000);
        } catch (InterruptedException e) {
            e.printStackTrace();
        }
    }
}

/**
 * Customized WebSocketClient
 */
class MyWebSocketClient extends WebSocketClient {
    public int bytes;
    public ArrayList<Object> param;
    public String audioPath;

    public MyWebSocketClient(String url, ArrayList<Object> param, String audioPath) throws URISyntaxException {
        super(new URI(url));
        this.param = param;
        this.audioPath = audioPath;
    }

    public void onOpen(ServerHandshake serverHandshake) {
        this.send(param.get(0).toString());
        this.send(param.get(1).toString());
        try {
            FileInputStream fs = new FileInputStream(audioPath);
            byte[] buffer = new byte[1024];
            while ((bytes = fs.read(buffer)) > 0) {
				//send audio
                this.send(buffer);
            }
            fs.close();
        } catch (FileNotFoundException e) {

            e.printStackTrace();
        } catch (IOException e) {
            e.printStackTrace();
        }
        this.send(param.get(2).toString());
    }

    @Override
    public void onMessage(String msg) {
		//Receive results
        System.out.println("result===>" + msg);
    }

    @Override
    public void onClose(int code, String reason, boolean remote) {
		//Disconnect
    }

    @Override
    public void onError(Exception e) {
    	e.printStackTrace();
    }
}



