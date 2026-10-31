# ChatAvatar

ChatAvatar ist eine wiederverwendbare .NET-10-WPF-Komponente zur Darstellung eines sprechenden 3D-Avatars für KI-Chatprogramme. Die Entwicklung erfolgt schrittweise nach den Projektzielen in `Plan/00Ziel.md` und den allgemeinen Kriterien in `Plan/00generell.md`.

## Lösung

```text
ChatAvatar.Component/             Wiederverwendbare WPF-Komponentenbibliothek
	Feature/<Feature>/Service/        Fachmodelle und Services je Feature
	UI/                              Von der Fachlogik getrennte Oberfläche
ChatAvatar.TestApp/                Eigenständiger WPF-Host zum Ausprobieren
ChatAvatar.Tests/Feature/<Feature>/ Featurebezogene automatisierte Tests
Plan/                              Projektziel und phasenweise Umsetzungspläne
```

Die fachliche Logik wird nach DDD-Grenzen in `Feature` organisiert. Services und Modelle eines Features liegen gemeinsam im Unterordner `Service`. Oberflächen-Code gehört in `UI`. Die Testanwendung referenziert die wiederverwendbare Komponente; das separate Testprojekt referenziert sie für automatisierte Tests.

## Voraussetzungen

- Windows
- .NET 10 SDK

## Offline-Abhängigkeiten

Die Laufzeit darf keine CDN- oder sonstigen Online-Ressourcen laden. Die WebView2-SDK- und System.Speech-Paketversionen sind in `ChatAvatar.Component.csproj` festgelegt. Three.js 0.187.0 liegt mit `three.module.js` und der benötigten `three.core.js` unter `ChatAvatar.Component/Assets/ThreeJs`; die MIT-Lizenz liegt daneben. Beide Dateien werden in die Buildausgabe und das Offline-Paket kopiert. Tests prüfen die lokale Verfügbarkeit der Module.

Für die Offline-Verteilung wird eine passende Fixed-Version-WebView2-Runtime mit dem Anwendungspaket ausgeliefert; die auf einem Entwicklungsrechner installierte Evergreen-Runtime ist kein Ersatz dafür. Das Publish-Skript verlangt eine lokale Runtime-Quelle. Auch die von System.Speech verwendete Stimme muss lokal auf dem Zielsystem verfügbar sein.

Nach einem initialen NuGet-Restore kann der Build-, Test- und WPF-Rendercheck bei deaktiviertem Netzwerk ausgeführt werden. Der Check wartet auf `SceneReady` nach dem ersten WebGL-Frame und gibt JavaScript-/WebGL-Fehler aus:

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

Der WPF-Testhost kopiert die mitgelieferten Beispielavatare beim ersten Start in sein Benutzerverzeichnis unter `%LOCALAPPDATA%/ChatAvatar/Avatars`. Host-Einstellungen speichert ausschließlich der Testhost unter `%LOCALAPPDATA%/ChatAvatar/settings.json`.

## Testhost-Ablauf

1. Einen der drei Beispielavatare in der rechten Auswahl aktivieren.
2. Einen Text eingeben und „Sprechen“ wählen. WAV-Erzeugung, Wiedergabe, Textfortschritt, Viseme und Audiopegel werden zusammengeführt.
3. „Abbrechen“ stoppt eine aktive Synthese oder Wiedergabe; Fehler erscheinen im Statusbereich.
4. „Einstellungen“ öffnet die hostverwaltete Stimme-, Avatar-, Lautstärke- und Tempoauswahl.
5. „Avatar-Editor“ öffnet den Editor für das Benutzerverzeichnis. Änderungen werden nach Rückkehr in die Avatarliste übernommen.

Die Ende-zu-Ende-Tests verbinden Avatarwahl, Hostsettings, Synthesevertrag, WAV-Pegel, Wiedergabezeitlinie und Antwortzustand mit isolierten Adaptern. Sie greifen weder auf Lautsprecher noch Onlinedienste zu.

## Offline-Bereitstellung

Vor der Offline-Phase einmalig die win-x64-Pakete restaurieren:

```powershell
dotnet restore ChatAvatar.TestApp/ChatAvatar.TestApp.csproj --runtime win-x64
```

Danach ein Paket mit einer lokal verfügbaren WebView2 Fixed Version erstellen:

```powershell
./scripts/Publish-Offline.ps1 -WebView2FixedRuntimePath "C:/path/to/Microsoft.WebView2.FixedVersionRuntime.156.0.4314.50.x64"
```

Das Paket wird unter `artifacts/offline-win-x64` erstellt. Es enthält die Self-Contained-.NET-10-WPF-Anwendung, WebView2 Fixed Version unter `WebView2FixedRuntime`, Three.js, HTML/JavaScript und Beispielavatare. Das `offline-deployment.json`-Manifest nennt Runtime-Version und Offline-Anforderung. Der WebView2-Host bevorzugt diesen Runtimeordner und verwendet auf Entwicklungsrechnern ohne ihn die installierte Evergreen-Runtime. Für eine Verteilung muss die Runtime-Quelle aus dem offiziellen WebView2 Fixed Version Runtime-Paket stammen; das Skript prüft die Dateistruktur, nicht die Herkunft oder Signatur.

Das Paket prüfen und kurz starten:

```powershell
./scripts/Test-OfflinePackage.ps1
```

## Integrationsvertrag

Ein Chat-Host erstellt eine Anfrage aus Text und den von ihm verwalteten Avatar-Einstellungen. Der Dienst liefert sofort eine Session mit stabiler Antwort-ID; der Host kann auf deren Abschluss warten oder sie abbrechen. Die Komponente speichert diese Einstellungen nicht.

```csharp
var request = new AvatarResponseRequest(
    "Hello",
    new AvatarResponseSettings(voiceName: "Local Voice", avatarId: "avatar-id", volume: 80, rate: 1));
var synthesis = await speechSynthesisService.SynthesizeAsync(request, cancellationToken);
using var playback = new AudioPlaybackSession(audioPlaybackEngine);
var result = await playbackCoordinator.PlayAsync(request.Text, synthesis, playback, cancellationToken);
```

Die Komponente speichert `AvatarResponseSettings` nicht dauerhaft. Der Host verwaltet die Auswahl und Persistenz und reicht die Einstellungen bei jeder Anfrage mit.

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

## Beispielavatare

Unter `ChatAvatar.Component/Assets/Avatars` liegen die drei ladbaren Beispiele `sample-face.avatar.json`, `sample-image-based.avatar.json` und `sample-abstract.avatar.json`. Sie verwenden ausschließlich selbst erstellte JSON-Konfigurationen, prozedurale Three.js-Geometrie und die integrierte Linsentextur. Es werden keine fremden Spielmodelle, Texturen oder 2D-Sprites mitgeliefert. Die drei Beispiele werden in die Buildausgabe kopiert und automatisiert über den Avatar-Katalog und Runtime-Service geladen. Die lokale Three.js-Bibliothek ist separat unter `Assets/ThreeJs` samt MIT-Lizenz enthalten.

Die Testanwendung hostet `AvatarWebView`, ein WPF-Control aus der Komponentenbibliothek. Es lädt `Assets/WebView/index.html` aus der Buildausgabe, blockiert Navigationen außerhalb lokaler Dateien und zeigt Laufzeit- oder Ladefehler direkt in der Ansicht. Die lokale Seite setzt eine Content Security Policy ohne Netzwerkverbindungen; die WebView2-Runtime muss auf dem System installiert sein.

WebView2 mappt den lokalen Buildausgabeordner auf den virtuellen Ursprung `https://chatavatar.local`; dieser Host wird von WebView2 lokal aus Dateien bedient und benötigt keinen Netzwerkdienst. Die Seite importiert Three.js aus `Assets/ThreeJs/three.module.js` samt lokalem `three.core.js`, rendert die auswählbaren 3D-Avatare und passt Kamera sowie Renderer per `ResizeObserver` an. Nach dem ersten WebGL-Frame sendet sie `SceneReady` mit Canvasgröße. JavaScript- und Rendererfehler werden an WPF weitergereicht und sichtbar angezeigt; externe Navigationen bleiben blockiert.

Der erste Gesichtsavatar wird lokal aus Three.js-Geometrien aufgebaut: Kopf, Ohren, Augen, Nase und Mund sind 3D-Meshes, keine 2D-Sprites. `AvatarWebView.SetVisemeAsync` sendet SAPI-Visem-IDs (0 bis 21) über den typisierten WebView-Kanal; unterschiedliche Gruppen öffnen den Mund unterschiedlich weit. `ResetSpeechAnimationAsync` setzt den Mund in den neutralen Zustand zurück.

Die Bildstil-Variante ist eine eigenständige 3D-Komposition aus extrudierter Metallplatte, Ringen, Linse und Lichtsegmenten. Eine prozedurale `CanvasTexture` wird lokal auf die 3D-Linse angewendet; es gibt keine externen oder aus *System Shock 2* übernommenen Assets. Die Darstellung reagiert auf Visemereignisse durch pulsierendes Linsenlicht. `SetAvatarTypeAsync(AvatarKind.ImageBased)` wechselt zwischen Gesichts- und Bildstil-Inszenierung.

Der abstrakte Avatar besteht aus einer 3D-Hülle, einem Ring, einer Lichtkugel und einem `PointLight`. `WaveAudioLevelAnalyzer` berechnet den normalisierten RMS-Pegel aus WAV-PCM- oder IEEE-Float-Samples; der Playback-Frame liefert ihn zusammen mit Text und Visemen. Über `SetAudioLevelAsync` steuert dieser Pegel das Leuchten. Bei Stille oder nach dem Reset bleibt das Licht im neutralen Ruhezustand.

`AvatarRuntimeService.SelectAsync` lädt eine Avatar-Datei anhand ihrer ID aus dem vom Host übergebenen Verzeichnis und aktiviert den zugehörigen Darstellungsmodus über `IAvatarRenderer`. `AvatarWebView` implementiert diesen Renderer. Die aktuelle Auswahl wird erst übernommen, wenn Laden und Renderer-Aufruf erfolgreich waren; fehlende oder doppelte IDs sowie Rendererfehler lassen die bisherige Auswahl unangetastet.

`AvatarEditorWindow` stellt die Grundaktionen zum Aktualisieren der Liste, Neuanlegen, Öffnen und Speichern bereit. Der Host übergibt Editor-Service und Avatar-Verzeichnis beim Erzeugen des Fensters. Der Editor kann Name, Darstellungstyp und typenbezogene Eigenschaften ändern; die lokale Live-Vorschau aktualisiert sich vor dem Speichern.

Im Editor sind je nach Darstellung nur passende Eigenschaften sichtbar: Gesichtsmodellpfad und Mundskalierung, Bildtextur und Farbton oder Lichtfarbe und Pulsintensität. Änderungen werden validiert und an die lokale WebView2-Vorschau gesendet. Assetpfade müssen relativ und lokal sein; URLs, absolute Pfade und `..`-Segmente werden abgewiesen. Ungültige Eingaben zeigen einen Fehler und verhindern Speichern.

`AvatarSettingsWindow` erhält die verfügbaren Stimmen und bestehende `AvatarResponseSettings` vom Host. Nach „Übernehmen“ liefert `ResultSettings` die gewählten Werte zurück; „Abbrechen“ liefert kein Ergebnis. Lautstärke (`0..100`) und Sprechtempo (`-10..10`) werden von `System.Speech` übernommen. Die Avatar-Komponente schreibt diese Einstellungen nicht auf Datenträger; Auswahl und Speicherung bleiben beim aufrufenden Programm.

## Sprachsynthese

`SpeechSynthesisService` nimmt eine `AvatarResponseRequest` entgegen und leitet Text sowie den vom Host angegebenen Stimmennamen an `SystemSpeechSynthesisEngine` weiter. Das Ergebnis enthält einen WAV-MemoryStream und einen zeitgeordneten Animations-Track aus `PhonemeReached`- und `VisemeReached`-Ereignissen. Die Zeitwerte beziehen sich auf die Audioausgabe; ein Track kann leer sein, wenn eine Stimme keine entsprechenden Ereignisse liefert. Der Aufrufer besitzt und entsorgt den zurückgegebenen Stream. Abbruch wird an `SpeechSynthesizer` weitergereicht und Fehler werden als Fehler des Synthesevorgangs zurückgegeben. System.Speech nutzt die auf dem Windows-System lokal installierten SAPI-Stimmen; es lädt selbst keine Sprachdienste online. Verfügbarkeit und Klang hängen von den lokal installierten Stimmen ab.

`AudioPlaybackSession` spielt den WAV-Stream über `SystemSoundPlayerEngine` ab. Der Host erhält `Playing` und einen terminalen Zustand (`Completed`, `Cancelled` oder `Failed`) über die Statusereignisse und das Ergebnis. Die Session übernimmt den übergebenen Stream und gibt ihn auch nach Abbruch oder Fehler frei; jede Session kann genau einmal gestartet werden.

`SpeechPlaybackCoordinator` kombiniert Transkript, SAPI-Textmarker und Phonem-/Visemcues. Die Playback-Engine meldet ihre verstrichene Wiedergabezeit; anhand dieser einen Zeitquelle liefert der Coordinator den aktuellen Textabschnitt, neu fällige Animationsereignisse und das aktive Visem. Frames werden auf dem beim Start aktuellen Synchronisierungskontext publiziert. Bei Abschluss, Abbruch oder Fehler wird ein leerer Frame gesendet. Pause und Fortsetzen sind mit `SoundPlayer` nicht verfügbar.

## Verifikationsstand

- `dotnet test ChatAvatar.sln` und `dotnet build ChatAvatar.sln` laufen erfolgreich.
- `Test-OfflineStartup.ps1` baut und testet ohne Restore und wartet auf `SceneReady` nach dem ersten erfolgreichen WebGL-Frame; das Netzwerk wurde bei diesem Lauf nicht aktiv getrennt.
- Das Offline-Publish- und Paketprüfskript wurde mit der lokal installierten Evergreen-Runtime-Struktur als Smoke-Test ausgeführt. Vor Verteilung muss es mit dem offiziellen Fixed-Version-Paket wiederholt werden.
- Der manuelle Sichttest der Mundformen in der WPF-Anwendung steht noch aus; die gerenderten Pixel und Animationen wurden nicht visuell abgenommen.

## Einschränkungen

- Sprachqualität und verfügbare Stimmen hängen von lokal installierten Windows-SAPI-Stimmen ab.
- `SoundPlayer` unterstützt in diesem Host keine Pause-/Fortsetzen-Aktion.
- Die Fixed-Version-WebView2-Runtime wird beim Publish als lokale Eingabequelle erwartet und nicht im Quellrepository gespeichert.
- Die Visem-zu-Mundform-Zuordnung ist ein erster funktionaler Platzhalter und benötigt noch manuelle Qualitätsprüfung.
