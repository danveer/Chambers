using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlackJack.Tests
{
    /*
        Summary - Restate the problem
        ############

          - web api deals black jack hands
          - card suits - hearts, diamonds, spades, clubs
          - card face values - ace,2,3,4,5,6,7,8,9,10,jack,queen,king

          potential entities
          - Entity: Card -> suit + face value (exception Ace 1 || 11)
          - Entity: Deck -> List<Card>
              Operation: shuffle cards

          - Entity: Hand -> List<Card>
          - Entity: Player -> name + Hand
          - Entity: Dealer -> Deck + Hand
              Operation: deal 2 cards to players (inc to dealer)
          - Entity: Game -> Dealer + Player
              - some of state somewhere to record players hand sum
              - domain rule somewhere to dictate >= 21 is a bust = game over
              Operation: Hit --> Dealer.Deal(noOfCards:1)
              Operation: Stick --> rule somewhere to make dealer keep dealing 
                                   to himself until hand sum => 17 < 21, dealer can bust too
              Game winner - greater score without going bust
          player goes first

       Running TODO list:
       ##############
          - flesh out core aspects of domain and behaviour using TDD
          - use domain in a webapi to serve up a web api for website
              - don't expose domain models through webapi, map to a viewmodel

          - maybe use inheritance to implement card / suits domain with a base card / different
          types of cards for suits
          - deck is not simple list as its cards need to be popped off and given to a player, so that the same card
              is not dealt twice
          - 
       */


    //TODO: rename class
    [TestClass]
    public class UnitTest1
    {
        // whats the best place to start? --> lets start simple first - cards

        [TestMethod]
        public void CardIsA5ofDiamonds()
        {
            var card = new Card(CardFace.Five, cardSuit: CardSuit.Diamonds);

            Assert.AreEqual(5, (int)card.CardFace);
        }

        [TestMethod]
        public void DeckHasCards()
        {
            var deck = new Deck(new List<Card>
            {
                new Card(CardFace.Five, CardSuit.Diamonds),
                new Card(CardFace.King, CardSuit.Diamonds),
            });

            Assert.IsTrue(deck.Cards.Count > 0);
        }

        [TestMethod]
        public void DeckIsShuffled()
        {
            var deck = new Deck(new List<Card>
            {
                new Card(CardFace.Five, CardSuit.Diamonds),
                new Card(CardFace.King, CardSuit.Diamonds),
            });

            deck.Shuffle();

            Assert.IsFalse(deck.Cards[0].CardFace != CardFace.King);
        }

        //jump to a bigger part of the logic

        [TestMethod]
        public void GameHasPlayers()
        {
            //TODO: noticing code duplication in test, refactor initialization maybe
            var deck = new Deck(new List<Card>
            {
                new Card(CardFace.Five, CardSuit.Diamonds),
                new Card(CardFace.King, CardSuit.Diamonds),
            });

            var game = new Game(new List<Player> { new Player(), new Player() }, deck);

            Assert.IsTrue(game.Players.Count > 0);
        }

        [TestMethod]
        public void StartGameAndDeal2CardsToPlayers()
        {
            // arrange
            var deck = new Deck(new List<Card>
            {
                new Card(CardFace.Five, CardSuit.Diamonds),
                new Card(CardFace.King, CardSuit.Diamonds),
            });

            var game = new Game(new List<Player> { new Player(), new Player() }, deck);

            // act
            game.Start();

            // assert
            //TODO: review - 2 assertions in a test is a bit frowned upon
            Assert.IsTrue(game.Players[0].Hand.Cards.Count == 2);
            Assert.IsTrue(game.Players[1].Hand.Cards.Count == 2);
        }

        [TestMethod]
        public void StartGameDeskShouldBeShuffled()
        {
            // use https://nsubstitute.github.io/ to assert that Deck.Shuffle() was called when
            // game starts
            throw new NotImplementedException();
        }

        //TODO: test method names try to state scenario and expectation
        [TestMethod]
        public void AddCardToHandSuccessfully()
        {
            var hand = new Hand();
            var card = new Card(CardFace.Five, CardSuit.Diamonds);

            hand.AddCard(card);

            Assert.IsTrue(hand.Cards[0].CardFace == CardFace.Five && hand.Cards[0].CardSuit == CardSuit.Diamonds);
            Assert.IsTrue(hand.Cards.Count == 1);
        }

        [TestMethod]
        public void HandScoreEquals10()
        {
            var hand = new Hand();
            hand.AddCard(new Card(CardFace.Five, CardSuit.Diamonds));
            hand.AddCard(new Card(CardFace.Five, CardSuit.Diamonds));

            Assert.IsTrue(hand.Score == 10);
        }

        [TestMethod]
        public void HandIsBustIfScoreIsOver21()
        {
            var hand = new Hand();
            hand.AddCard(new Card(CardFace.King, CardSuit.Diamonds));
            hand.AddCard(new Card(CardFace.King, CardSuit.Diamonds));
            hand.AddCard(new Card(CardFace.Five, CardSuit.Diamonds));

            Assert.IsTrue(hand.IsBust);
        }

        [TestMethod]
        public void HandIsNotBustIfScoreUnder21()
        {
            var hand = new Hand();
            hand.AddCard(new Card(CardFace.King, CardSuit.Diamonds));
            hand.AddCard(new Card(CardFace.King, CardSuit.Diamonds));

            Assert.IsFalse(hand.IsBust);
        }

    }

    //TODO: move int domain project
    public class Game
    {
        public List<Player> Players { get; }
        public Deck Deck { get; }

        public Game(List<Player> players, Deck deck)
        {
            Players = players;
            Deck = deck;
        }

        public void Start()
        {
            // deck needs to be shuffled

            // deal to cards to every player
        }
    }

    //TODO: move into domain project
    public class Player
    {
        public string Name { get; set; }
        public Hand Hand { get; set; }

        public void Stick()
        {

        }

        public void Hit()
        {

        }
    }

    //TODO: move into domain project
    public class Hand
    {
        public List<Card> Cards { get; } = new List<Card>();
        public int Score
        {

            get
            {
                // TODO: having to cast to int is a code smell
                return Cards.Sum(c => (int)c.CardFace);
            }
        }

        public bool IsBust
        {
            get
            {
                if (Score >= 21)
                    return true;

                return false;
            }
        }

        public void AddCard(Card card)
        {
            Cards.Add(card);

            //TODO: if IsBust == true maybe throw an exception and catch 
            // it in the game class
        }
    }

    //TODO: move to domain project 
    public class Deck
    {
        public List<Card> Cards { get; }

        public Deck(List<Card> cards)
        {
            Cards = cards;
        }

        public void Shuffle()
        {
            //TODO: for now this is sufficient to satisfy shuffle, reimplement later
            Cards.Reverse();
        }
    }

    public enum CardSuit
    {
        Diamonds
    }

    //this enum will fall over when it comes to handling the Ace, so will have to move to 
    // classes to represent cards
    public enum CardFace
    {
        Five = 5,
        King = 10
        //Ace = 1 || 11 - breaking case
    }

    //TODO: move this into domain project once finished designing with tests
    public class Card
    {
        public CardFace CardFace { get; private set; }
        public CardSuit CardSuit { get; private set; }

        public Card(CardFace cardFace, CardSuit cardSuit)
        {
            CardFace = cardFace;
            CardSuit = cardSuit;
        }
    }
}
