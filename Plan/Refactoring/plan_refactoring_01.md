# Refactoring 01: `Featur` in `Feature` umbenennen

## Ziel

Den falsch geschriebenen Ordnernamen `Featur` projektweit durch `Feature` ersetzen. Die Verzeichnisstruktur bleibt ansonsten unverändert; alle betroffenen Namespaces und Namespace-Verwendungen werden konsistent angepasst.

## Bestand und Umfang

- Implementierung: `ChatAvatar.Component/Featur/` mit dem Avatar-Fachbereich.
- Tests: `ChatAvatar.Tests/Featur/` mit Avatar- und OfflineRuntime-Tests.
- Namespace-Verwendungen: unter anderem in der Komponente, der Testanwendung und den Tests.
- Dokumentation: Strukturübersicht und Konventions-/Phasenpläne, die `Featur` als aktuellen Pfad nennen.

Die Umbenennung betrifft ausschließlich `Featur` → `Feature`. Feature-, Service-, UI- und Projektgrenzen sowie Laufzeitverhalten und Testinhalte bleiben unverändert. Es werden keine fachlichen Änderungen vorgenommen.

## Umsetzungsschritte

1. **Vollständigkeit ermitteln**
   - Im Quellcode, in XAML-/Projektdateien und in der Dokumentation nach Pfaden, `using`-Direktiven, Namespace-Deklarationen und qualifizierten Namen mit `Featur` suchen.
   - Generierte Dateien unter `bin` und `obj` nicht manuell ändern; sie werden durch den Build neu erzeugt.
2. **Verzeichnisse umbenennen**
   - `ChatAvatar.Component/Featur/` nach `ChatAvatar.Component/Feature/` verschieben.
   - `ChatAvatar.Tests/Featur/` nach `ChatAvatar.Tests/Feature/` verschieben.
   - Die darunterliegende Struktur und Dateien unverändert übernehmen.
3. **Namespaces und Referenzen aktualisieren**
   - `ChatAvatar.Component.Featur.*` in Deklarationen und Verwendungen in `ChatAvatar.Component.Feature.*` ändern.
   - `ChatAvatar.Tests.Featur.*` entsprechend in `ChatAvatar.Tests.Feature.*` ändern.
   - Alle Fundstellen außerhalb dieser Namespace-Präfixe ebenfalls prüfen und anpassen, falls sie auf die umbenannten Ordner verweisen.
4. **Dokumentation angleichen**
   - Veraltete Strukturangaben in `README.md`, `Plan/00generell.md`, `Plan/plan_phase_01.md`, `Plan/plan_phase_04.md` und `Plan/plan_phase_22.md` auf `Feature` aktualisieren.
   - Weitere Treffer aus der Bestandsaufnahme ebenfalls aktualisieren, soweit sie die aktuelle Projektstruktur beschreiben.

## Akzeptanzkriterien

- [x] Die Implementierung liegt unter `ChatAvatar.Component/Feature/`; der alte Ordner `ChatAvatar.Component/Featur/` existiert nicht mehr.
- [x] Die Tests liegen unter `ChatAvatar.Tests/Feature/`; der alte Ordner `ChatAvatar.Tests/Featur/` existiert nicht mehr.
- [x] Alle produktiven Namespace-Deklarationen und -Verwendungen verwenden `ChatAvatar.Component.Feature` statt `ChatAvatar.Component.Featur`.
- [x] Alle Test-Namespace-Deklarationen und -Verwendungen verwenden `ChatAvatar.Tests.Feature` statt `ChatAvatar.Tests.Featur`.
- [x] README und aktuelle Strukturkonventionen dokumentieren ausschließlich `Feature` als Ordnernamen.
- [x] Es gibt keine unbeabsichtigten aktiven Quellcode- oder Dokumentationsverweise mehr auf die alte Ordner-/Namespace-Schreibweise.
- [x] `dotnet test ChatAvatar.sln` läuft erfolgreich durch; Build und bestehende Tests bleiben unverändert erfolgreich.

## Prüfung

1. Repository-weit nach `Featur`, `ChatAvatar.Component.Featur` und `ChatAvatar.Tests.Featur` suchen; verbleibende Treffer einzeln prüfen (historischer Hinweis in diesem Refactoringplan ausgenommen).
2. `dotnet test ChatAvatar.sln` im Repository-Stammverzeichnis ausführen.
3. Änderungen mit `git diff --check` auf Whitespace-Fehler prüfen und den Diff auf unbeabsichtigte Änderungen kontrollieren.