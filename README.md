# ChatAvatar

ChatAvatar ist eine wiederverwendbare .NET-10-WPF-Komponente zur Darstellung eines sprechenden 3D-Avatars für KI-Chatprogramme. Die Entwicklung erfolgt schrittweise nach den Projektzielen in `Plan/00Ziel.md` und den allgemeinen Kriterien in `Plan/00generell.md`.

## Lösung

```text
ChatAvatar.Component/             Wiederverwendbare WPF-Komponentenbibliothek
	Featur/<Feature>/Service/        Fachmodelle und Services je Feature
	UI/                              Von der Fachlogik getrennte Oberfläche
ChatAvatar.TestApp/                Eigenständiger WPF-Host zum Ausprobieren
ChatAvatar.Tests/Featur/<Feature>/ Featurebezogene automatisierte Tests
Plan/                              Projektziel und phasenweise Umsetzungspläne
```

Die fachliche Logik wird nach DDD-Grenzen in `Featur` organisiert. Services und Modelle eines Features liegen gemeinsam im Unterordner `Service`. Oberflächen-Code gehört in `UI`. Die Testanwendung referenziert die wiederverwendbare Komponente; das separate Testprojekt referenziert sie für automatisierte Tests.

## Voraussetzungen

- Windows
- .NET 10 SDK

## Offline-Abhängigkeiten

Die Laufzeit darf keine CDN- oder sonstigen Online-Ressourcen laden. Die WebView2-SDK- und System.Speech-Paketversionen sind in `ChatAvatar.Component.csproj` festgelegt. Three.js 0.187.0 liegt als lokale Datei unter `ChatAvatar.Component/Assets/ThreeJs/three.module.js`; die MIT-Lizenz liegt daneben. Die Datei wird in die Buildausgabe kopiert. Ein Test prüft die lokale Verfügbarkeit der SDK-Assemblies und des Three.js-Moduls.

Für die spätere Offline-Verteilung muss zusätzlich eine passende Fixed-Version-WebView2-Runtime mit dem Anwendungspaket ausgeliefert werden; die auf einem Entwicklungsrechner installierte Evergreen-Runtime ist kein Ersatz dafür. Auch die von System.Speech verwendete Stimme muss lokal auf dem Zielsystem verfügbar sein. Paketierung und Offline-Installation werden in Phase 23 umgesetzt.

Nach einem initialen NuGet-Restore kann der Build-, Test- und WPF-Startcheck bei deaktiviertem Netzwerk ausgeführt werden. Der Check prüft, dass der GUI-Prozess die Message-Loop erreicht:

```powershell
./scripts/Test-OfflineStartup.ps1
```

## Build und Tests

Im Repository-Stammverzeichnis ausführen:

```powershell
dotnet test ChatAvatar.sln
```

## Testanwendung starten

```powershell
dotnet run --project ChatAvatar.TestApp/ChatAvatar.TestApp.csproj
```

Das aktuelle Grundgerüst öffnet ein leeres WPF-Fenster. Die Avatar-Funktionen werden in den nachfolgenden Phasen ergänzt.

## Integrationsvertrag

Ein Chat-Host erstellt eine Anfrage aus Text und den von ihm verwalteten Avatar-Einstellungen. Der Dienst liefert sofort eine Session mit stabiler Antwort-ID; der Host kann auf deren Abschluss warten oder sie abbrechen. Die Komponente speichert diese Einstellungen nicht.

```csharp
var request = new AvatarResponseRequest(
	"Hello",
	new AvatarResponseSettings(VoiceName: "Local Voice", AvatarId: "avatar-1"));
using var session = responseService.Start(request);
var result = await session.Completion;
```

Die Session endet mit `Completed`, `Cancelled` oder `Failed`. Bei einem Fehler enthält das Ergebnis eine Meldung. Die Audio- und Avatar-Ausführung wird in den folgenden Phasen an diesen Vertrag angeschlossen.

## Avatar-Dateiformat

Avatar-Dateien verwenden ein JSON-Dokument mit `schemaVersion` und einem Avatar-Modell. Version 1 unterscheidet die drei Darstellungsarten über typisierte Konfigurationen:

```json
{
	"schemaVersion": 1,
	"avatar": {
		"id": "d8e6d6d3-0ca9-4c1c-82db-425e734f27bc",
		"displayName": "Abstract Assistant",
		"appearance": {
			"kind": "abstract",
			"lightColor": "#33AAFF",
			"pulseIntensity": 0.65
		}
	}
}
```

Das Domänenmodell validiert Identität, Name, Darstellungstyp und typbezogene Werte. `AvatarDefinitionFileStore` lädt und speichert einzelne Dateien über explizit übergebene Pfade. Das Speichern erfolgt über eine temporäre Datei mit anschließendem Austausch, damit ein abgebrochener Schreibvorgang die vorherige Datei nicht teilweise überschreibt. Beschädigtes JSON, unbekannte Darstellungstypen und nicht unterstützte Schema-Versionen werden abgewiesen.

`AvatarCatalog` listet `*.avatar.json`-Dateien ausschließlich in dem bei jedem Aufruf übergebenen Verzeichnis. Fehlerhafte Dateien werden als einzelne Kataloghinweise zurückgegeben; gültige Einträge bleiben verfügbar. Ein Avatar kann über seine ID geladen werden. Doppelte IDs werden als mehrdeutig gemeldet, statt willkürlich eine Datei auszuwählen.

Die Testanwendung hostet `AvatarWebView`, ein WPF-Control aus der Komponentenbibliothek. Es lädt `Assets/WebView/index.html` aus der Buildausgabe, blockiert Navigationen außerhalb lokaler Dateien und zeigt Laufzeit- oder Ladefehler direkt in der Ansicht. Die lokale Seite setzt eine Content Security Policy ohne Netzwerkverbindungen; die WebView2-Runtime muss auf dem System installiert sein.

WebView2 mappt den lokalen Buildausgabeordner auf den virtuellen Ursprung `https://chatavatar.local`; dieser Host wird von WebView2 lokal aus Dateien bedient und benötigt keinen Netzwerkdienst. Die Seite importiert Three.js aus `Assets/ThreeJs/three.module.js`, rendert ein animiertes 3D-Testobjekt und passt Kamera und Renderer per `ResizeObserver` an die verfügbare Fläche an. Der Nachrichtenkanal verwendet versionierte JSON-Nachrichten (`Ready` und `Initialize`); externe Navigationen bleiben blockiert.

## Sprachsynthese

`SpeechSynthesisService` nimmt eine `AvatarResponseRequest` entgegen und leitet Text sowie den vom Host angegebenen Stimmennamen an `SystemSpeechSynthesisEngine` weiter. Die Engine schreibt asynchron in einen WAV-MemoryStream; der Aufrufer besitzt und entsorgt den zurückgegebenen Stream. Abbruch wird an `SpeechSynthesizer` weitergereicht und Fehler werden als Fehler des Synthesevorgangs zurückgegeben. System.Speech nutzt die auf dem Windows-System lokal installierten SAPI-Stimmen; es lädt selbst keine Sprachdienste online. Verfügbarkeit und Klang hängen von den lokal installierten Stimmen ab.
