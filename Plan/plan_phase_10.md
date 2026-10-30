# Phase 10: Audiowiedergabe

## Ziel

Den erzeugten Audiostream als Antwort abspielen und den Wiedergabezustand verwalten.

## Umfang

- Wiedergabe, Ende, Abbruch und Fehler als Sitzungszustände abbilden.
- Ressourcen nach Wiedergabe oder Abbruch freigeben.
- Die Zustände über den Integrationsvertrag bereitstellen.

## Abhängigkeit

Phasen 03 und 09.

## Akzeptanzkriterien

- [x] Ein WAV-Stream kann über `SystemSoundPlayerEngine` abgespielt werden.
- [x] Abbruch stoppt die Wiedergabe; Stream und Player werden nach Abschluss oder Fehler freigegeben.
- [x] Der Host erhält `Playing` und genau einen terminalen Zustand sowie ein Ergebnisobjekt.
- [x] Tests prüfen Abschluss, Abbruch, Wiedergabefehler, Stream-Freigabe und einmaligen Start.
- [x] Ablauf und Stream-Eigentümerschaft sind in der `README.md` dokumentiert.