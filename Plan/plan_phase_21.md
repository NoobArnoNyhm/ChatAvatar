# Phase 21: WPF-Testanwendung

## Ziel

Die Avatar-Komponente über eine eigenständige .NET-10-WPF-Testanwendung praktisch erprobbar machen.

## Umfang

- Eingabefeld und Aktion zum Übergeben eines Textstrings bereitstellen.
- Avatar-Auswahl, Textanzeige, Audiowiedergabe und Animation verbinden.
- Einstellungen in der Testanwendung als Host verwalten und speichern.

## Abhängigkeit

Phasen 03, 10, 12, 16, 19 und 20.

## Akzeptanzkriterien

- [x] Eingabetext wird synthetisiert, abgespielt und mit Text-, Visem- und Pegelframes synchronisiert.
- [x] Alle drei Beispielavatare werden im Benutzerverzeichnis bereitgestellt und sind auswählbar.
- [x] Abbruch und Synthese-/Wiedergabefehler werden im Statusbereich angezeigt.
- [x] Der Testhost speichert seine Settings selbst unter `%LOCALAPPDATA%` und übergibt sie dem Synthese-/Avatarvertrag.
- [x] Testhost-Ablauf und manuelle Prüfschritte sind in der `README.md` dokumentiert.