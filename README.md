# Snog
Lille programmeringssprog

# Example
```
ESKETIT

SÆT DECIMALTAL $malou_alder = 5.8
SÆT DECIMALTAL $malou_vægt = 7.5
SÆT HELTAL $malou_udseende = 10

REDIGER $malou_alder = 5.75

// Følgende bliver optimeret væk, da det er den eksisterende værdi
REDIGER $malou_udseende = 10

UDSKRIV $malou_alder
UDSKRIV $malou_vægt
UDSKRIV $malou_udseende

// Ville ikke være tilladt, da $malou_udseende er et HELTAL.
// REDIGER $malou_udseende = 9.5
// UDSKRIV $malou_udseende

SKRRT SKRRT
```