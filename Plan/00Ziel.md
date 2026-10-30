 # Projektziel: KI-Chat-Avatar

## Zielbild

Ziel des Projekts ist eine lokal und offline nutzbare, wiederverwendbare Avatar-Komponente für KI-Chatprogramme. Das aufrufende Programm übergibt einen Textstring. Die Komponente erzeugt daraus zunächst Sprache und stellt anschließend einen dreidimensional animierten Avatar dar, dessen Bewegung und Darstellung mit Audio und Text synchronisiert sind.

Die Anwendung wird mit **C# und .NET 10** als **WPF-Anwendung** umgesetzt. Für die Avatar-Darstellung werden **WebView2 und Three.js** verwendet. Die erste Sprachsynthese erfolgt mit **System.Speech**.

## Ablauf

1. Das aufrufende Chatprogramm übergibt einen Textstring an die Avatar-Komponente.
2. Die Komponente verarbeitet den Text und erzeugt daraus einen Audiostream.
3. Text, Audio und verfügbare Phonem- beziehungsweise Zeitinformationen werden synchronisiert.
4. Die Komponente gibt den Text aus und animiert den Avatar passend zu Sprache und Audiostream.

## Avatar-Arten

Es werden drei konfigurierbare Darstellungsarten unterstützt:

1. **Animiertes Gesicht:** Ein dreidimensionales Gesicht reagiert auf die gesprochenen Phoneme, insbesondere durch passende Mund- und Gesichtsbewegungen.
2. **Animierte Bilddarstellung:** Eine stilisierte, bildbasierte Avatar-Inszenierung, etwa in Anlehnung an die Wirkung von Shodan aus *System Shock 2*. Die Umsetzung erfolgt als 3D-Szene beziehungsweise über 3D-Elemente und Texturen; 2D-Sprites sind ausgeschlossen.
3. **Abstrakte Form:** Eine abstrakte dreidimensionale Darstellung, zum Beispiel ein Licht wie bei HAL 9000, das sichtbar auf den Audiostream reagiert und pulsiert.

Für jede Darstellungsart wird mindestens ein verwendbares Beispiel mitgeliefert.

## Bearbeitung und Einstellungen

- Ein Editor ermöglicht es, vorhandene Avatare zu bearbeiten und neue Avatare zu erstellen.
- Avatare werden jeweils in eigenen Dateien gespeichert und können wieder geladen werden.
- Die Anwendung stellt ein Einstellungsfenster für die Avatar-Konfiguration bereit.
- Das aufrufende Programm liefert die Einstellungen und ist für deren dauerhafte Speicherung verantwortlich. Die Avatar-Komponente speichert Anwendungseinstellungen nicht selbst.

## Testanwendung und Qualität

- Eine separate **.NET-10-WPF-Testanwendung** dient zum Ausprobieren der Avatar-Komponente. Sie nimmt Textstrings entgegen, lässt daraus den Audiostream erzeugen und zeigt die dazu animierte Avatar-Darstellung.
- Alle Features erhalten ein eigenes Testprojekt beziehungsweise werden in einem getrennten Testprojekt automatisiert getestet.
- Die Tests prüfen insbesondere die Verarbeitung der Eingabe, die Synchronisation von Text, Audio und Animation sowie Bearbeitung, Laden und Speichern von Avatar-Dateien und die Bereitstellung der Einstellungen.

## Offline-Anforderung

Das Projekt darf keine Onlinedienste zur Laufzeit verwenden. Alle für Entwicklung, Bereitstellung und Nutzung benötigten Abhängigkeiten sowie Darstellungsressourcen müssen offline verfügbar sein. Audioerzeugung, Avatar-Dateien und die Three.js-Darstellung müssen ohne Internetverbindung funktionieren.
