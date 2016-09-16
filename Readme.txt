1. Brainstorming

Fahrtkilometer für Steuererklärung sammeln -> Beleg generieren (Arbeitsorts <> Home)

Benzin-Verbrauch in Familiy / Familie bei Car-Sharing -> wer hat wieviel Most verbraucht -> Auswertungen, für Abrechnungen relevant

Fahrgemeinschaft -> Route anbieten -> Fahrgelegenheiten abrufen

Tracking des Autos zur Kontrolle -> wo ist der Fahrer (Kinder)

Musik in Car an Fahrverhalten anpassen (agressiv, entspannt)

Wo ist die nächste Tankstelle -> Vorschläge aufbereiten (nach Preis, nach Beliebtheit)

Green-Rating (Aut Nutzung war notwendig, oder man hätte ÖV nehmen können oder zufuss gehen können). Umwelt-Fussabdruck. Basis-Parameter: GV, Halbtax, Fahrrad)

	- Auswertungen für Zeitperioden:
		? was war mein Fussabdruck
		? was hätte ich vermeiden können, was hätte ich sparen können
		? wieviel Zeit habe ich im Auto verbracht
		? wieviel Zeit hätte ich in der ÖV für Lesen, Arbeiten etc. erbringen können , wieviel hätte ich an Auto-Abschreibung einsparen können (Kilometerstand -> Eurotax)
		? Was waren meine Kilometer zur Arbeit
		? Kompensation: bspw. In Ökofond 
		? Challenge: bester Footprint in spezifischen Peers



2. GreenDrive

2.0 Userstory
Als User wünsche ich mir beim Einstieg in der APp ein einfaches, intuitives Menu, wo ich mit einfachen touch-Clicks auf die gewünschten Dashboards gelange. Initial werden mir meine persönlichen Verbrauchsdaten der letzten Woche angezeigt.

Acceptance Criterias
	- Menu wird angezeigt, als Kacheln (mit Absprung auf die Dashboards)
	- personifizierte Home-View mit persönlicher Auswertung (Verbrauch letzte Woche)
     

2.1 Userstory
Als User möchte ich wissen, wie mein Fussabdruck (Emissionen, Verbrauch Benzin) über eine längere Zeitspanne (Woche, Monat, Jahr) bezüglich Nutzung meines Autos aussieht, damit ich künftig umweltfreundlicher fahre. Ich wünsche mir für mein Userprofil eine graphische Auswertung über die angeforderte Zeitspanne (im Vergleich sehe ich die Vorperioden oder andere User).

Acceptance Criterias
	- Eigene Fussabdruck View (nur lesend)
	- Bezug der Daten aus Firebase APi
	- Verbrauch wird als Chart über die Zeitspanne angezeigt
	- Vergleich zu vorherigen Perioden wird angezeigt
	- Navigation nach Home


2.2 Userstory
Als User möchte ich mich mit meinen Kollegen / Familienmitgliedern messen, wie ökologisch mein Footprint ist. Dabei möchte ich Strecken und Verbrauch gegenübergestellt sehen. Es soll über ein Dashboard eine Rangliste erscheinen, die allen ersichtlich ist.

Acceptance Criterias
	- Eigene View für die Rangliste (nur lesend)
	- Bezug der Date aus Firebase APi
	- Alle User in Peer werden angezeigt (Footprint-Points, Verbrauch Benzin, Km)
	- Navigation nach Home

2.3 Userstory
Als User möchte ich wissen, wieviel Zeit ich über eine bestimmte Zeitspanne im Auto verbracht habe, die ich auch alternativ (zu Fuss, Velo, ÖV) hätte nutzen können, damit ich künftig meine Zeit bewusster einteile.

Acceptance Criterias:
	- Eigene View für AUswertung über Zeitspanne
	- Anzeige der Zeiten im Auto
	- Anzeige der Zeiten, die ich alternativ hätte verbringen können
 	- Nivagation nach Home 
 



LINKS:
Localhost:8000/app/index.html#home