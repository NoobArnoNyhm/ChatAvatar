# Refactoring 02: Feature-Inhalte nach Funktionen und Typverantwortung strukturieren

## Ziel

Die derzeit gemeinsam unter `ChatAvatar.Component/Feature/Avatar/Service/` liegenden 44 C#-Dateien nach ihrer Funktion aufteilen. Innerhalb jeder Funktion werden Services und Modelle getrennt:

```text
Feature/[Funktionsname]/Service/
Feature/[Funktionsname]/Modell/
```

Die Namespaces entsprechen dem neuen Dateipfad. `Modell` wird genau in dieser Schreibweise verwendet. Die bestehenden englischen Typnamen bleiben unverändert; deshalb werden auch die Funktionsordner englisch benannt.

## Umfang und Abgrenzung

- Alle Dateien unter `ChatAvatar.Component/Feature/` erhalten eine eindeutige Zuordnung.
- Namespace-Deklarationen, `using`-Direktiven und vollständig qualifizierte Typverwendungen werden in Komponente, Testhost und Tests angepasst.
- Feature-bezogene Tests werden den Funktionen und, soweit sinnvoll, den Unterordnern `Service` und `Modell` zugeordnet.
- Die WPF-Oberfläche bleibt unter `UI`; Host- und Offline-Infrastruktur werden nicht zusätzlich in die Feature-Struktur verschoben.
- Verhalten, öffentliche Member, Validierung, Serialisierung, Ressourcenpfade und Projektabhängigkeiten bleiben unverändert.
- Die Änderung öffentlicher Typ-Namespaces ist eine inkompatible API-Änderung: externe Verbraucher müssen ihre Referenzen anpassen und neu bauen. Es werden keine Kompatibilitätswrapper oder doppelten Typen eingeführt.

## Konkrete Zuordnung

Die folgenden fünf Funktionsbereiche bilden den vorhandenen Code ab, ohne neue fachliche Logik oder zusätzliche Abstraktionen einzuführen.

### 1. Avatar: Definition, Verwaltung und Auswahl

**`Feature/Avatar/Modell/`**

- `AbstractAvatarAppearance.cs`
- `AvatarAppearance.cs`
- `AvatarAssetPath.cs`
- `AvatarCatalogEntry.cs`
- `AvatarCatalogIssue.cs`
- `AvatarCatalogResult.cs`
- `AvatarColor.cs`
- `AvatarDefinition.cs`
- `AvatarFileDocument.cs`
- `AvatarKind.cs`
- `FaceAvatarAppearance.cs`
- `ImageBasedAvatarAppearance.cs`

**`Feature/Avatar/Service/`**

- `AvatarCatalog.cs`
- `AvatarDefinitionFileStore.cs`
- `AvatarEditorService.cs`
- `AvatarRuntimeService.cs`
- `IAvatarRenderer.cs`

### 2. AudioPlayback: Audiowiedergabe und Pegelanalyse

**`Feature/AudioPlayback/Modell/`**

- `AudioPlaybackResult.cs`
- `AudioPlaybackStatus.cs`
- `AudioPlaybackStatusChangedEventArgs.cs`

**`Feature/AudioPlayback/Service/`**

- `AudioPlaybackSession.cs`
- `IAudioLevelAnalyzer.cs`
- `IAudioPlaybackEngine.cs`
- `SystemSoundPlayerEngine.cs`
- `WaveAudioLevelAnalyzer.cs`

### 3. SpeechSynthesis: Spracherzeugung

**`Feature/SpeechSynthesis/Modell/`**

- `SpeechSynthesisResult.cs`

**`Feature/SpeechSynthesis/Service/`**

- `ISpeechSynthesisEngine.cs`
- `ISpeechSynthesisService.cs`
- `SpeechSynthesisService.cs`
- `SystemSpeechSynthesisEngine.cs`

### 4. SpeechAnimation: Sprach-, Text- und Animationssynchronisierung

**`Feature/SpeechAnimation/Modell/`**

- `SpeechAnimationCue.cs`
- `SpeechAnimationCueType.cs`
- `SpeechAnimationTrack.cs`
- `SpeechPlaybackFrame.cs`
- `SpeechPlaybackFrameEventArgs.cs`
- `SpeechTextCue.cs`

**`Feature/SpeechAnimation/Service/`**

- `SpeechPlaybackCoordinator.cs`
- `SpeechTimelineCursor.cs`

### 5. AvatarResponse: Lebenszyklus einer Avatar-Antwort

**`Feature/AvatarResponse/Modell/`**

- `AvatarResponseRequest.cs`
- `AvatarResponseResult.cs`
- `AvatarResponseSettings.cs`
- `AvatarResponseStatus.cs`

**`Feature/AvatarResponse/Service/`**

- `AvatarResponseSession.cs`
- `IAvatarResponseService.cs`

## Zuordnungsregeln und Namespaces

- Datenverträge, Records, Enums, Event-Daten und fachliche Wertobjekte gehören nach `Modell`. Bestehende Validierung und einfache Modelllogik bleiben bei diesen Typen.
- Ablaufsteuerung, Zustands-/Ressourcenverwaltung, Persistenz, Engines und deren Schnittstellen gehören nach `Service`. Deshalb bleiben die beiden Session-Klassen Services.
- Gemeinsam benötigte Modelle verbleiben bei ihrer fachlichen Funktion; keine Kopien oder allgemeinen Sammelordner anlegen.
- Funktionsübergreifende Verwendungen erhalten explizite Imports. Beispielsweise verwendet `SpeechSynthesisResult` das Modell `SpeechAnimationTrack`; der `SpeechPlaybackCoordinator` verwendet Typen aus Spracherzeugung und Audiowiedergabe.
- Namespace-Schema: `ChatAvatar.Component.Feature.[Funktionsname].[Service|Modell]`.
- Beispiel: `AvatarKind` wechselt von `ChatAvatar.Component.Feature.Avatar.Service` nach `ChatAvatar.Component.Feature.Avatar.Modell`.
- Beispiel: `AudioPlaybackSession` wechselt nach `ChatAvatar.Component.Feature.AudioPlayback.Service`.
- `ChatAvatar.Component.Feature.Avatar.Service` bleibt für die fünf dort verbleibenden Services gültig. Ein pauschales Ersetzen dieses gesamten Namespace-Präfixes wäre daher falsch.

## Umsetzungsschritte

1. **Bestand und Ausgangszustand prüfen**
   - Die Dateiliste mit der Zuordnung abgleichen und alle Typverwendungen einschließlich XAML, qualifizierter Namen, Reflection-/Typnamen-Strings und Testfilter ermitteln.
   - Vor der Änderung `dotnet test ChatAvatar.sln` ausführen, damit die Ausgangsbasis feststeht.
2. **Dateien verschieben und Deklarationen anpassen**
   - Alle 44 Dateien entsprechend der Zuordnung verschieben; Dateinamen und Inhalte außerhalb der Namespace-Anpassung unverändert lassen.
   - Leere alte Verzeichnisse entfernen; `Feature/Avatar/Service/` bleibt für die dort vorgesehenen Services bestehen.
3. **Verwendungen auflösen**
   - Imports in allen Feature-Dateien, UI-Code-behind-Dateien, `AvatarWebMessage`, `MainWindow` und `TestHostSettingsStore` aktualisieren.
   - Pro Datei nur die tatsächlich benötigten Funktions-/Modell-Namespaces importieren.
   - XAML-Typzuordnungen und weitere Fundstellen nur anpassen, wenn sie betroffene Typen referenzieren. Generierte Dateien unter `bin` und `obj` nicht manuell ändern.
4. **Tests entsprechend strukturieren**
   - Avatar-Domänen-, Beispielavatar- und Antwortmodelltests dem jeweiligen `Modell`-Ordner zuordnen.
   - Katalog-, Persistenz-, Editor-, Laufzeit-, Audiowiedergabe-, Pegelanalyse-, Synthese- und Synchronisierungstests dem jeweiligen Funktionsbereich unter `Service` zuordnen.
   - `SpeechAnimationTrackTests` nach `Feature/SpeechAnimation/Modell/` verschieben.
   - `AvatarResponseContractTests` nach `Feature/AvatarResponse/Service/` verschieben; die enthaltenen Session- und Vertragsprüfungen gemeinsam erhalten.
   - UI-/Webnachrichten-Tests, Host-Einstellungstests und Assembly-Prüfungen nicht als Fachmodelle einordnen. Diese Tests in ihren bisherigen speziellen Testbereichen belassen und ihre produktiven Imports anpassen.
   - Funktionsübergreifende Antwortworkflowtests unter `Feature/AvatarResponse/Integration/` einordnen; die OfflineRuntime-Tests bleiben unverändert zugeordnet.
   - Verschobene Test-Namespaces dem neuen Testpfad entsprechend in `ChatAvatar.Tests.Feature.[Funktionsname].*` ändern.
5. **Dokumentation aktualisieren**
   - Strukturübersicht in `README.md` und Regeln in `Plan/00generell.md` auf getrennte Unterordner `Service` und `Modell` ändern.
   - Weitere aktuelle Strukturangaben in den Phasenplänen prüfen und angleichen.
   - Historische Beschreibungen des abgeschlossenen Refactorings 01 nicht als aktive Strukturvorgabe behandeln.
6. **Abschluss validieren**
   - Dateizuordnung, Namespaces, Tests und Diff anhand der folgenden Kriterien prüfen.
   - Diesen Plan erst nach erfolgreicher Prüfung als umgesetzt markieren.

## Akzeptanzkriterien

- [x] Alle 44 Feature-Dateien sind genau einmal vorhanden und entsprechend der Zuordnung einsortiert.
- [x] Unter `Feature/Avatar/Service/` liegen nur die fünf vorgesehenen Services; Avatar-Modelle liegen unter `Feature/Avatar/Modell/`.
- [x] Jeder produktive Feature-Namespace stimmt mit dem Funktions- und Typordner der Datei überein.
- [x] Alle Verbraucher in Komponente, Testhost und Tests verwenden die neuen Typ-Namespaces; es gibt keine veralteten Verwendungen verschobener Typen.
- [x] Die Feature-Tests sind fachlich zugeordnet und werden weiterhin vollständig entdeckt und ausgeführt.
- [x] Öffentliche Member, JSON-Schema/-Feldnamen und Ressourcenpfade sind unverändert; es gibt keine fachlichen Änderungen.
- [x] README und allgemeine Strukturkonventionen beschreiben `Feature/[Funktionsname]/[Service|Modell]`.
- [x] `dotnet test ChatAvatar.sln` baut alle Projekte erfolgreich und führt mindestens dieselben Tests wie vor dem Refactoring ohne Fehler aus.
- [x] `git diff --check` meldet keine Whitespace-Fehler; der Diff enthält nur beabsichtigte Verschiebungen, Namespace-/Importänderungen und zugehörige Dokumentation.

## Prüfstrategie

1. Vorher-/Nachher-Dateilisten anhand der 44 geplanten Dateien vergleichen; fehlende und doppelte Dateien ausschließen.
2. Die Namespace-Deklarationen gegen die jeweiligen Dateipfade prüfen. Nicht nur nach dem alten Avatar-Service-Namespace suchen, da dieser teilweise weiterverwendet wird.
3. Alte vollständig qualifizierte Namen der verschobenen Typen repository-weit suchen; verbleibende historische Dokumentationshinweise einzeln bewerten.
4. `dotnet test ChatAvatar.sln` ausführen und Testanzahl mit dem vorab ermittelten Ausgangszustand vergleichen.
5. Insbesondere Persistenz-/Modell-, Antwortworkflow-, Audio-, Synthese- und Synchronisierungstests sowie Host-/UI-Vertragsprüfungen unverändert erfolgreich ausführen.
6. Den Diff auf reine Strukturänderungen prüfen und `git diff --check` ausführen.

## Status

Umgesetzt am 31.10.2026.

- Alle 44 Feature-Dateien sind nach der obigen Zuordnung strukturiert; Testdateien und Verbraucher sind angepasst.
- Die Namespace-/Importänderung wurde mit einem Vergleich der unveränderten Codekörper abgesichert.
- Die Namespaces aller 65 Feature- und Feature-Testdateien stimmen mit ihren Verzeichnissen überein.
- `dotnet test ChatAvatar.sln`: vor und nach dem Refactoring 74 erfolgreiche Tests, keine Fehler und keine übersprungenen Tests.
- README, allgemeine Strukturkonventionen und betroffene Phasenpläne sind angepasst; `git diff --check` meldet keine Fehler.
