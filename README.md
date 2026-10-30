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
