Creator	"yFiles"
Version	"2.14"
graph
[
	hierarchic	1
	label	""
	directed	1
	node
	[
		id	0
		label	"Complex Con 1"
		graphics
		[
			x	160.375
			y	150.186767578125
			w	711.75
			h	467.62646484375
			type	"roundrectangle"
			fill	"#F5F5F5"
			outline	"#000000"
			outlineStyle	"dashed"
			topBorderInset	43.25
			bottomBorderInset	0.0
			leftBorderInset	0.0
			rightBorderInset	183.75
		]
		LabelGraphics
		[
			text	"Complex Con 1"
			fill	"#EBEBEB"
			fontSize	15
			fontName	"Dialog"
			alignment	"right"
			autoSizePolicy	"node_width"
			anchor	"t"
			borderDistance	0.0
		]
		isGroup	1
	]
	node
	[
		id	1
		label	"Hello foreigner."
		graphics
		[
			x	-124.5
			y	12.0
			w	112.0
			h	30.0
			type	"rectangle"
			raisedBorder	0
			fill	"#00CCFF"
			outline	"#000000"
		]
		LabelGraphics
		[
			text	"Hello foreigner."
			fontSize	12
			fontName	"Dialog"
			model	"null"
		]
		gid	0
	]
	node
	[
		id	2
		label	"(openUI)"
		graphics
		[
			x	248.5
			y	12.0
			w	138.0
			h	30.0
			type	"rectangle"
			raisedBorder	0
			fill	"#993366"
			outline	"#000000"
		]
		LabelGraphics
		[
			text	"(openUI)"
			fontSize	12
			fontName	"Dialog"
			model	"null"
		]
		gid	0
	]
	node
	[
		id	3
		label	"I don't like you though."
		graphics
		[
			x	248.5
			y	126.0
			w	138.0
			h	30.0
			type	"rectangle"
			raisedBorder	0
			fill	"#FFCC00"
			outline	"#000000"
		]
		LabelGraphics
		[
			text	"I don't like you though."
			fontSize	12
			fontName	"Dialog"
			model	"null"
		]
		gid	0
	]
	node
	[
		id	4
		label	"So we are two!"
		graphics
		[
			x	248.5
			y	240.0
			w	138.0
			h	30.0
			type	"rectangle"
			raisedBorder	0
			fill	"#FFCC00"
			outline	"#000000"
		]
		LabelGraphics
		[
			text	"So we are two!"
			fontSize	12
			fontName	"Dialog"
			model	"null"
		]
		gid	0
	]
	node
	[
		id	5
		label	"(exit)"
		graphics
		[
			x	247.20000000000005
			y	354.0
			w	138.0
			h	30.0
			type	"rectangle"
			raisedBorder	0
			fill	"#FFCC00"
			outline	"#000000"
		]
		LabelGraphics
		[
			text	"(exit)"
			fontSize	12
			fontName	"Dialog"
			model	"null"
		]
		gid	0
	]
	edge
	[
		source	1
		target	2
		label	"What do you sell?"
		graphics
		[
			fill	"#993366"
			targetArrow	"standard"
		]
		LabelGraphics
		[
			text	"What do you sell?"
			fontSize	12
			fontName	"Dialog"
			configuration	"AutoFlippingLabel"
			contentWidth	99.384765625
			contentHeight	18.701171875
			model	"null"
			position	"null"
		]
	]
	edge
	[
		source	1
		target	3
		label	"I like you"
		graphics
		[
			fill	"#000000"
			targetArrow	"standard"
		]
		edgeAnchor
		[
			xSource	1.0
			xTarget	-1.0
		]
		LabelGraphics
		[
			text	"I like you"
			fontSize	12
			fontName	"Dialog"
			configuration	"AutoFlippingLabel"
			contentWidth	51.35546875
			contentHeight	18.701171875
			model	"null"
			position	"null"
		]
	]
	edge
	[
		source	1
		target	4
		label	"I hate you!"
		graphics
		[
			fill	"#000000"
			targetArrow	"standard"
		]
		edgeAnchor
		[
			xSource	1.0
			xTarget	-1.0
		]
		LabelGraphics
		[
			text	"I hate you!"
			fontSize	12
			fontName	"Dialog"
			configuration	"AutoFlippingLabel"
			contentWidth	60.0390625
			contentHeight	18.701171875
			model	"null"
			position	"null"
		]
	]
	edge
	[
		source	1
		target	5
		label	"I gota go."
		graphics
		[
			fill	"#000000"
			targetArrow	"standard"
		]
		edgeAnchor
		[
			xSource	1.0
			xTarget	-1.0
		]
		LabelGraphics
		[
			text	"I gota go."
			fontSize	12
			fontName	"Dialog"
			configuration	"AutoFlippingLabel"
			contentWidth	54.0390625
			contentHeight	18.701171875
			model	"null"
			position	"null"
		]
	]
	edge
	[
		source	3
		target	2
		label	"What do you sell?"
		graphics
		[
			fill	"#993366"
			targetArrow	"standard"
		]
		LabelGraphics
		[
			text	"What do you sell?"
			fontSize	12
			fontName	"Dialog"
			configuration	"AutoFlippingLabel"
			contentWidth	99.384765625
			contentHeight	18.701171875
			model	"null"
			position	"null"
		]
	]
	edge
	[
		source	3
		target	4
		label	"I hate you..."
		graphics
		[
			fill	"#000000"
			targetArrow	"standard"
		]
		LabelGraphics
		[
			text	"I hate you..."
			fontSize	12
			fontName	"Dialog"
			configuration	"AutoFlippingLabel"
			contentWidth	66.70703125
			contentHeight	18.701171875
			model	"null"
			position	"null"
		]
	]
	edge
	[
		source	3
		target	5
		label	"I gota go."
		graphics
		[
			fill	"#000000"
			targetArrow	"standard"
			Line
			[
				point
				[
					x	248.5
					y	126.0
				]
				point
				[
					x	410.0
					y	255.5
				]
				point
				[
					x	247.20000000000005
					y	354.0
				]
			]
		]
		LabelGraphics
		[
			text	"I gota go."
			fontSize	12
			fontName	"Dialog"
			configuration	"AutoFlippingLabel"
			contentWidth	54.0390625
			contentHeight	18.701171875
			model	"null"
			position	"null"
		]
	]
	edge
	[
		source	4
		target	5
		label	"I gota go..."
		graphics
		[
			fill	"#000000"
			targetArrow	"standard"
		]
		LabelGraphics
		[
			text	"I gota go..."
			fontSize	12
			fontName	"Dialog"
			configuration	"AutoFlippingLabel"
			contentWidth	60.70703125
			contentHeight	18.701171875
			model	"null"
			position	"null"
		]
	]
	edge
	[
		source	4
		target	2
		label	"What do you sell?"
		graphics
		[
			fill	"#993366"
			targetArrow	"standard"
			Line
			[
				point
				[
					x	248.5
					y	240.0
				]
				point
				[
					x	398.0
					y	117.5
				]
				point
				[
					x	248.5
					y	12.0
				]
			]
		]
		LabelGraphics
		[
			text	"What do you sell?"
			fontSize	12
			fontName	"Dialog"
			configuration	"AutoFlippingLabel"
			contentWidth	99.384765625
			contentHeight	18.701171875
			model	"null"
			position	"null"
		]
	]
]
