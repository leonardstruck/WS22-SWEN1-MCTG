# WS22/SWEN1 Semesterprojekt MCTG

[Link zum GitHub Repository](https://github.com/leonardstruck/WS22-SWEN1-MCTG.git)

## Protokoll

### 1. Einleitung

In diesem Protokoll werden die einzelnen Schritte der Entwicklung des Semesterprojekts dokumentiert. 
Dabei wird auf die einzelnen Komponenten des Projektes eingegangen und die Lösungen beschrieben. 
Die Komponenten sind in der Reihenfolge ihrer Abhängigkeiten aufgelistet.

### 2. Komponenten

#### 2.1. HTTP-Server

Der HTTP-Server wurde mit eigenen Klassen implementiert.
Diese Klassen sind in der Subsolution "HttpServer" des Hauptprojekts enthalten.

##### 2.1.1. HTTP-Context

Der HTTP-Context ist eine Klasse, die die Informationen eines HTTP-Requests, wie z.B. die URL, die HTTP-Methode, die Header und die Daten, enthält.
Neben den Informationen des Requests, enthält der Context auch die Informationen des Responses, wie z.B. die Statuscode, die Header und die Daten.
Der Context wird von den später implementierten HTTP-Controllern verwendet, um die Informationen des Requests zu verarbeiten und die Informationen des Responses zu setzen.

