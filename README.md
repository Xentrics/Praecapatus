# Praecapatus
Work with Unity 5.3.4

NOTES ON NOTES:
 - 1 +: start of impl
 - 2 +: fully functional
 - 3 +: manageable from interface


NOTES ON GAME CONTROL
- Abilities can be used in 2 major ways:
	(1) story mode
		- abis can be called with or without keyboard +
		- abis called by keyboard will show version selection screen [by default]
			- this behaviour should be changeable as an option ('disable version selection from shortkeys outside fights') +
	(2) fight mode
		- abis will be used as saved using shortkies (preset)
		- the shortkey includes the specific version, unless better control of ability is possible
		[idea]
		- every X KR, the player is allowed to change his shortkeys
- world rotation +


NOTES ON GAME MECHANICS
- Any object implements the script 'PraeObject' with implements 'Interactable' ++
- Interactions
	- shop UI
	- Talk UI +
		- Talk-Class stuff
- Infamie
- Ruhm


NOTES ON STORY
- the player chooses either 'IndustrieAdept' or 'Technici' as class [Logenmagier]
- magic should have varies useages and version


NOTES ON ABILITIES
- (extremely) prolonged abilities may be handled my making instances of 'ProlongedAbility' (PA)
	- in this case, PA will be a class, that might be independet of 'AbstractAbility'
- fundamential casts
	- Biomantie:		Wachstum [complex]
	- Chronomantie:		Zeitblase
	- Curanistik:		Heilung
	- Evokation:		Astralbelebung [complex]	+
	- Illusionstik:		Illusionsball [complex]
	- Metamagie:		Analyse [complex]
	- Okkultismus:		Pech [complex]
	- Protegonik:		Schnelle Abwehr
	- Pugnistik:		Astralangriff
	- Telekinese:		Telekinetische Blase
	- (Telepathie:		Gefühlsteilung)
- fundamential melee attack
- fundamential range attack