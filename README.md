# SpeechSuper API

Enhance your language learning with our Pronunciation Assessment API. Tailored to support language learners, our API evaluates pronunciation by analyzing audio files and text strings, providing comprehensive metrics for speech quality and fluency. 

## Pronunciation Assessment API (Scripted)

### 🌟 Basic Features
The Pronunciation Assessment API (scripted) provided by SpeechSuper offers a comprehensive analysis of learners' audio recordings, providing valuable insights in the following areas:

- Accurate assessment at phoneme, syllable, word, and sentence levels, aligning with native English speech patterns.
- Fluency metrics, including words correct per minute and pause count, alongside IELTS/PTE score estimation.

Basic Features:
- Pronunciation score (of phonemes, words, sentences)
- Fluency score
- Completeness score
- Rhythm score or speed

### 🪐 Highlight Features

Explore advanced capabilities with SpeechSuper's Pronunciation Assessment API, complementing its fundamental features. Unleash the power of:

- Syllable stress analysis
- Liaison detection
- Loss of plosion identification
- Rising/falling tone analysis for sentence endings
- Real-time feedback at the sentence level
- Detection of phoneme-level mispronunciations (insertion, deletion, substitution)

With SpeechSuper's Pronunciation Assessment API, you access robust tools to evaluate and refine your pronunciation skills within structured contexts. Elevate your language proficiency with precise analysis and instant feedback mechanisms.
  
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


### 🚀 Getting Started

1. **Get the `appKey` and `secretKey`.**

If you don't have the keys, please go to [SpeechSuper](https://www.speechsuper.com/), and click "Contact us" to fill in the sheet. We will get in touch with you very soon!

2. **Git clone this project to your local.**

```
git clone https://github.com/speechsuper/speechsuper-api-samples.git
```

3. **Choose the example of your interest.**

We have [http](https://github.com/speechsuper/speechsuper-api-samples/tree/main/http_samples) / [websocket](https://github.com/speechsuper/speechsuper-api-samples/tree/main/websocket_samples) examples in multiple coding languages. Fill in your `appKey` and `secretKey`: 
``` 
appKey = "Insert your appKey here"
secretKey = "Insert your secretKey here"
```

4. **Change the inputs according to your needs.**
```
coreType = "sent.eval" // sentence evaluation
refText = "The successful warrior is the average man with laser-like focus." // reference text
audioPath = "The audio path of the spoken sentence." 
audioType = "wav"
```

> **Notes on Audio:** SpeechSuper supports most audio formats, such as wav, mp3, opus, ogg, and amr.
> 
> To optimize file size and enhance performance, we strongly recommend recording your audio at the following settings:
> | Audio Attribute | Suggestion      |
> | --------------- | --------------- |
> | Sample size     | 16-bit          |
> | Sample rate     | 16Khz           |
> | Channels        | 1 (mono)        |
> | Bitrate         | ≥ 96kbps        |


5. **Launch 🚀 your code and get the result.**

---


## English Spontaneous Speech Assessment API (Unscripted)

### 🌟 Overview
The English Spontaneous Speech Assessment API (Unscripted) offered by SpeechSuper provides a comprehensive analysis of learners' audio recordings, delivering valuable insights in the following areas:

- Overall score
- Pronunciation score
- Fluency score
- Grammar score
- Vocabulary score
- Transcription

### 🪐 Pro Features
In addition to the basics, the Pro features enhance the analysis by offering detailed information such as:

- Pause markers
- Frequency of pause fillers
- Speaking rate
- Speech length
- Vocabulary usages
- Detection and correction of grammatical errors
- Word-level pronunciation score

With SpeechSuper's API, you can unlock a wealth of information to assess and improve your English spontaneous speech skills.


### 🚀 Getting Started

1. **Get the `appKey` and `secretKey`.**
If you don't have the keys, please go to [SpeechSuper](https://www.speechsuper.com/), and click "Contact us" to fill in the sheet. We will get in touch with you very soon!


2. **Git clone this project to your local.**
```
git clone https://github.com/speechsuper/speechsuper-api-samples.git
```

3. **Choose the example of your interest.**

We have [http](https://github.com/speechsuper/speechsuper-api-samples/tree/main/http_samples) / [websocket](https://github.com/speechsuper/speechsuper-api-samples/tree/main/websocket_samples) examples in multiple coding languages. Fill in your `appKey` and `secretKey`: 

``` 
appKey = "Insert your appKey here"
secretKey = "Insert your secretKey here"
```

4. **Change the inputs according to your needs.**

```
coreType = "speak.eval.pro"  # Selects English spontaneous speech assessment
test_type = "ielts"  # Specifies the evaluation standard for English
question_prompt = "What's your favorite food?"  # Specify the question prompt if applicable
audioPath = "The audio path of the speech."
audioType = "wav"

```
> **Notes on Audio:** SpeechSuper supports most audio formats, such as wav, mp3, opus, ogg, and amr.
> 
> To optimize file size and enhance performance, we strongly recommend recording your audio at the following settings:
> | Audio Attribute | Suggestion      |
> | --------------- | --------------- |
> | Sample size     | 16-bit          |
> | Sample rate     | 16Khz           |
> | Channels        | 1 (mono)        |
> | Bitrate         | ≥ 96kbps        |

5. **Launch 🚀 your code and get the result.**



