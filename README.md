# SpeechSuper API

## 🌟 Basics
SpeechSuper analyzes learner's audios and returns results covering various aspects: 
- pronunciation score (of phonemes, words, sentences)
- fluency score
- completeness score
- rhythm score or speed

## 🪐 Pro Features
  
- syllable stress
- liaison
- loss of plosion
- rising/falling tone at the end of a sentence
- instant feedback (on sentence and paragraph)
- phoneme level mispronunciation detection (insertion, deletion, substitution)
- etc
  
<details>
<summary>Spoken languages covered</summary>
  
* English
* Chinese
* German
* French
* Russian
* Korean
* Japanese
* Spanish
* more to come
</details>

<details>
<summary>Coding languages covered</summary>
  
* Java
* C/C++
* Swift
* golang
* php
* C#
* Unity
* Javascript
* Objective-C
* Python
* Node
* Rust
* etc
</details>

<details>
<summary>Platforms supported</summary>
  
* iOS
* Android
* Web
* Windows
* MacOS
* Linux
* etc
</details>


## 🚀 Getting Started
1. Get the `appKey` and `secretKey`.

If you don't have the keys, please go to [SpeechSuper](https://www.speechsuper.com/), and click "Contact us" to fill in the sheet. We will get in touch with you very soon!


2. Git clone this project to your local.
```
git clone https://github.com/speechsuper/speechsuper-api-samples.git
```

3. Choose the example of your interest. 

We have [http](https://github.com/speechsuper/speechsuper-api-samples/tree/main/http_samples) / [websocket](https://github.com/speechsuper/speechsuper-api-samples/tree/main/websocket_samples) examples in multiple coding languages. Fill in your `appKey` and `secretKey`: 
``` 
appKey = "Insert your appKey here"
secretKey = "Insert your secretKey here"
```

4. Change the inputs according to your needs.
```
coreType = "sent.eval" // sentence evaluation
refText = "The successful warrior is the average man with laser-like focus." // reference text
audioPath = "The audio path of the spoken sentence." 
audioType = "wav"
```

5. Launch 🚀 your code and get the result.
