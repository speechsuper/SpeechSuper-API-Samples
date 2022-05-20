package main

//Import packages used by the project
import (
	"crypto/sha1"
	"sync"

	"flag"
	"fmt"
	"io/ioutil"
	"strconv"

	"log"

	"os"
	"os/signal"
	"time"

	"github.com/google/uuid"
	"github.com/gorilla/websocket"
)


func main() {
	flag.Parse()
	log.SetFlags(0)

	interrupt := make(chan os.Signal, 1)
	signal.Notify(interrupt, os.Interrupt)

	appKey := "Insert your appKey here"
	secretKey := "Insert your secretKey here"

	userId := "guest"

	coreType := "word.eval"
	refText := "supermarket"
	audioPath := "supermarket.wav"
	audioType := "wav"
	audioSampleRate := "16000"

    //create connect
	url := "wss://api.speechsuper.com/" + coreType

	wsClient, _, err := websocket.DefaultDialer.Dial(url, nil)
	if err != nil {
		log.Fatal("dial:", err)
	}

	wg := new(sync.WaitGroup)
	wg.Add(1)

	go func(wsClient *websocket.Conn) {
		defer wg.Done()
		_, message, err := wsClient.ReadMessage()
		if err != nil {
			log.Println("websocket error:", err)
			return
		}
		log.Printf("result===> %s", message)
	}(wsClient)
	
	//send connect request
	timestamp := strconv.FormatInt(time.Now().Unix(), 10)
	data := []byte(appKey + timestamp + secretKey)
	connectSig := fmt.Sprintf("%x", sha1.Sum(data))
	//connect param
	connectStr := `{"cmd":"connect","param":{"sdk":{"version":16777472,"source":1,"protocol":1},"app":{"applicationId":"`+ appKey + `","sig":"` + connectSig + `","timestamp":"` + timestamp + `"}}}`
	wsClient.WriteMessage(websocket.TextMessage, []byte(connectStr))

	//send start request
	timestamp = strconv.FormatInt(time.Now().Unix(), 10)
	data = []byte(appKey + timestamp + userId + secretKey)
	startSig := fmt.Sprintf("%x", sha1.Sum(data))
	tokenId := uuid.New().String()
	//start param
	startStr := `{"cmd":"start","param":{"app":{"applicationId":"` + appKey + `","sig":"` + startSig+ `","timestamp":"` + timestamp + `","userId":"` + userId + `"},"audio":{"audioType":"` + audioType + `","sampleRate":` + audioSampleRate + `,"channel":1,"sampleBytes":2},"request":{"coreType":"` + coreType + `","refText":"` + refText + `","tokenId":"` + tokenId + `"}}}`
	wsClient.WriteMessage(websocket.TextMessage, []byte(startStr))

    //send audio
	f, _ := os.Open(audioPath)
	defer f.Close()
	voiceBytes, _ := ioutil.ReadAll(f)
	wsClient.WriteMessage(websocket.BinaryMessage, voiceBytes)

	//send stop request
	stopStr := `{"cmd":"stop"}`
	wsClient.WriteMessage(websocket.TextMessage, []byte(stopStr))

	wg.Wait()
}



