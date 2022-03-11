package main

import (
	"bytes"
	"crypto/sha1"
	"fmt"
	"io"
	"io/ioutil"
	"mime/multipart"
	"net/http"
	"os"
	"strconv"
	"time"
)

func main() {
	appKey := "Insert your appKey here"
	secretKey := "Insert your secretKey here"

	coreType := "word.eval"
	url := "https://api.speechsuper.com/" + coreType
	method := "POST"

	payload := &bytes.Buffer{}
	writer := multipart.NewWriter(payload)

	timestamp := strconv.FormatInt(time.Now().Unix(), 10)
	userId := "guest" //Required and customizable. This value is required when generating startsig
	connectData := []byte(appKey + timestamp + secretKey)
	startData := []byte(appKey + timestamp + userId + secretKey)
	connectSig := fmt.Sprintf("%x", sha1.Sum(connectData))
	startSig := fmt.Sprintf("%x", sha1.Sum(startData))
	refText := "supermarket"
	audioPath := "supermarket.wav"
	audioType := "wav"
	audioSampleRate := "16000"

	//request param
	_ = writer.WriteField("text", "{\"connect\": {\"cmd\": \"connect\", \"param\": {\"sdk\": {\"version\": 16777472, \"source\": 9, \"protocol\": 2}, \"app\":{\"applicationId\":\""+appKey+"\",\"sig\":\""+connectSig+"\",\"timestamp\":\""+timestamp+"\"}}}, \"start\": {\"cmd\": \"start\", \"param\": {\"app\":{\"applicationId\":\""+appKey+"\",\"sig\":\""+startSig+"\",\"timestamp\":\""+timestamp+"\",\"userId\":\""+userId+"\"}, \"audio\": {\"audioType\": \""+audioType+"\", \"channel\": 1, \"sampleBytes\": 2, \"sampleRate\":"+audioSampleRate+"}, \"request\": {\"coreType\": \""+coreType+"\", \"refText\":\""+refText+"\", \"tokenId\": \"tokenId\"}}}}")

	//send streaming audio
	part2, _ := writer.CreateFormFile("audio", audioPath)

	file, _ := os.Open(audioPath)
	defer file.Close()
	_, _ = io.Copy(part2, file)

	err := writer.Close()
	if err != nil {
		fmt.Println(err)
	}

	client := &http.Client{}
	req, err := http.NewRequest(method, url, payload)

	if err != nil {
		fmt.Println(err)
	}
	req.Header.Set("Content-Type", writer.FormDataContentType())
	req.Header.Set("Request-Index", "0") //Request-Index is always 0

	res, err := client.Do(req)
	if err != nil {
		fmt.Println(err)
	}

	defer res.Body.Close()
	body, err := ioutil.ReadAll(res.Body)
	if err != nil {
		fmt.Println(err)
	}

	fmt.Println(string(body))
}
