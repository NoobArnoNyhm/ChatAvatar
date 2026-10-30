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
