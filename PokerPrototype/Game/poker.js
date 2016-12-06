/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

var suit = new Suit();
var aCard = new Card(1, suit.S_CLUBS, null);

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

function Suit(){
	this.suit = new Array(0,1,2,3);

	this.S_CLUBS = 0;
	this.S_DIAMONDS = 1;
	this.S_HEARTS = 2;
	this.S_SPADES = 3;
}


/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//Card
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function Card(rank, suit, img){
	this.rank = rank;
	this.suit = suit;
	this.img = img;

	this.cardNames = new Array("2","3","4","5","6","7","8","9","10","J","Q","K","A");

	this.D_JACK = 11;
	this.D_QUEEN = 12;
	this.D_KING = 13;
	this.D_ACE = 14;

	this.toString = Card_toString;
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//Deck
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function Deck(){
	this.deck = new Array(52);
	this.init = Deck_init;
	this.shuffle = Deck_shuffle;
	this.draw = Deck_draw;
}

function Deck_init(){
	this.deck = new Array(52);
	this.pointer = 0;
	for(var i = 1; i < 14; i++){
		var imgSrc = "images/Cards/Classic/c";
		for(var j=0;j<3;j++){
			switch(j){
				case 0:
				  	imgSrc += "c";
					break;
				case 1:
					imgSrc += "d";
					break;
				case 2:
					imgSrc += "h";
					break;
				case 3:
					imgSrc += "s";
					break;
			}
			if(i<10){
				imgSrc+="0";
			}
			imgSrc+=new Number(i).toString();
			var im = new Image();
			im.src=imgSrc+".png";
			var Card=new Card(i+1,suit.suit[j],im)
			this.deck.push(Card);
		}
	}
}

function Deck_shuffle(){
	for(var i = 0; i < 52; i++){
		var selectOne = Math.floor(51*Math.random() + 1);
		var temp = this.deck[i];
		this.deck[i] = this.deck[selectOne];
		this.deck[selectOne] = temp;				
	}
}

function Deck_draw(){
	if(this.deck.length == 0){
		this.deck.Deck_shuffle();
	} 
	return this.deck.shift();
}