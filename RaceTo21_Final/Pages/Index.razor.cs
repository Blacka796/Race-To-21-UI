using Microsoft.AspNetCore.Components.Web;
using System.Collections.Generic;
using System.Text.Json;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Linq;
using AntDesign;

namespace RaceTo21_Final.Pages
{
    public partial class Index
    {
        private int gameStatus = 0;//Calculate for the total round of game

        private int currentRound = 1;//record the current round

        private bool _visible = false;
  
        private bool isGoOn;//check if game still going or not
 
        private int isDeal = 0;//chekc if player needs more card

        private string _startGameVisible = "";

        private int playerNum = 0;//record the total number of player

        private bool _dealVisible;//decide if the table will show or not

        private bool giveCard;//decide if the table will give cards to players

        private string preWinerName;//record the winner's name

        private int maxRound;//record the max round that players decided

        private string tips = string.Empty;

        List<Player> Players = new List<Player>();

        protected override void OnInitialized()
        {
            base.OnInitialized();
            
        }

        private void HandleOk(MouseEventArgs e)
        {
            _visible = false;
            for (int i = 0; i < playerNum; i++)
            {
                var num = i + 1;
                Players.Add(new Player { PlayerId = num, PlayerName = "" });
            }
        }
        private void StartGame()
        {
            Console.WriteLine(JsonSerializer.Serialize(Players));
            maxRound = Players.Select(x => x.WantRound).Min();
            _startGameVisible = "display:none;";

        }
        private void GameOver()
        {
            _visible = false;
        }

        private void InquireDeal()
        {
            isGoOn = true;
            _dealVisible = true;
            foreach (Player item in Players) 
            {
                if(item.IsNextPlay == false)
                {
                    item.OutStyle = "display:none;";
                }
            }
        }

        private void DealOk()
        {
            _dealVisible = false;
            if (isDeal == 1)
            {
                Console.WriteLine(JsonSerializer.Serialize(Players));
                if (Players.All(x => x.PlayerStatus != PlayerStatus.active))
                {

                    //If they all have cards and they all do not want more card, the result will based on their points
                    Console.WriteLine("-----");
                    var winner = Players.Where(a=>a.PlayerStatus == PlayerStatus.stay).OrderByDescending(x => x.PlayerScore).FirstOrDefault();
                    winner.WinNumber += 1;
                    winner.PlayerStatus = PlayerStatus.win;
                    tips = $"Player {winner.PlayerName} win.";
                    preWinerName = winner.PlayerName;
                    if (currentRound == maxRound)
                    {
                        isDeal = 3;
                    }
                    else
                    {
                        isDeal = 2;
                    }
                    StateHasChanged();
                    return;
                }
                foreach (var item in Players)
                {
                    if (item.PlayerStatus == PlayerStatus.active && item.IsNextPlay == true)
                    {
                        int r = new Random().Next(item.Cards.Count);
                        item.HandCards.Add(item.Cards[r]);
                        item.Cards.RemoveAt(r);
                        item.PlayerScore = item.PlayerScore + item.HandCards.Last().CardScore;
                        //If there is only one player left, he will be the winner
                        if (Players.Where(x => x.IsNextPlay == true).Count(x => x.PlayerStatus == PlayerStatus.active || x.PlayerStatus == PlayerStatus.stay) == 1)
                        {
                            int winnerIndex = Players.Where(x => x.IsNextPlay == true).ToList().FindIndex(x => x.PlayerStatus == PlayerStatus.active);
                            Players.Where(x => x.IsNextPlay == true).ToList()[winnerIndex].PlayerStatus = PlayerStatus.win;
                            preWinerName = Players[winnerIndex].PlayerName;
                            tips = $"Player {Players.Where(x => x.IsNextPlay == true).ToList()[winnerIndex].PlayerName} win.";
                            //decide who is winner and show him
                            Players.Where(x => x.IsNextPlay == true).ToList()[winnerIndex].WinNumber += 1;
                           
                            if (currentRound == maxRound)
                            {
                                isDeal = 3;
                            }
                            else
                            {
                                isDeal = 2;
                            }
                            StateHasChanged();
    
                            return;
                        }
                        else
                        {
                            //if someone got 21 points
                            if (item.PlayerScore == 21)
                            {
                                item.PlayerStatus = PlayerStatus.win;
                                tips = $"Player  {item.PlayerName}  win.";
                                preWinerName = item.PlayerName;
                                item.WinNumber += 1;
                                //if current round is the max round, have a result
                                if (currentRound == maxRound)
                                {
                                    isDeal = 3;
                                }
                                else
                                {
                                    isDeal = 2;
                                }
                                StateHasChanged();
                      
                                return;
                            }
                            else if (item.PlayerScore > 21)
                            {
                                item.PlayerStatus = PlayerStatus.bust;
                            }
                        }
                    }
                }
            }
            else
            {
                //if nobody draw cards at the beginning, this round will end
                if (Players.Where(x => x.IsNextPlay == true).All(x => x.PlayerStatus == PlayerStatus.stay))
                {
                    isDeal = 2;
                }
                else
                {
                    //if there is only one person draw card at this round, then he will be the winner
                    if (Players.Where(x => x.PlayerStatus == PlayerStatus.active).Count() == 1 && Players.Any(x=>x.PlayerStatus != PlayerStatus.stay))
                    {
                        var thisWinner = Players.Find(x => x.PlayerStatus == PlayerStatus.active);
                        tips = $"Player {thisWinner.PlayerName} win.";
                        thisWinner.WinNumber += 1;
                        if (currentRound == maxRound)
                        {
                            isDeal = 3;
                        }
                        else
                        {
                            isDeal = 2;
                        }
                        StateHasChanged();
                        return;
                    }
                    else
                    {
                        StartDeal();
                    }
                }
            }
        }

        private void DealCancel()
        {
            _dealVisible = false;
        }
        /// <summary>
        /// Function that is for the draw
        /// </summary>
        private void StartDeal()
        {
            List<string> suits = new List<string> { "Spades", "Hearts", "Clubs", "Diamonds" };
            foreach (var item in Players)
            {

                List<Card> cards = new List<Card>();
                Card card = null;

                for (int cardVal = 1; cardVal <= 13; cardVal++)
                {
                    var index = cardVal;
                    foreach (var cardSuit in suits)
                    {
                        string cardName;
                        string cardLongName;
                        switch (index)
                        {
                            case 1:
                                cardName = "A";
                                cardLongName = "Ace";
                                break;
                            case 11:
                                cardName = "J";
                                cardLongName = "Jack";
                                break;
                            case 12:
                                cardName = "Q";
                                cardLongName = "Queen";
                                break;
                            case 13:
                                cardName = "K";
                                cardLongName = "King";
                                break;
                            default:
                                cardName = index.ToString();
                                cardLongName = cardName;
                                break;
                        }
                        card = new Card();
                        card.ShortCardName = cardName;
                        card.LongCardName = cardLongName;
                        card.CardSuit = cardSuit;
                        if (card.ShortCardName == "J" || card.ShortCardName == "Q" || card.ShortCardName == "K")
                        {
                            card.CardScore = 10;
                        }
                        else
                        {
                            card.CardScore = index;
                        }
                        card.Index = index;
                        cards.Add(card);
                    }

                }
                item.Cards = cards.Distinct().ToList();

                //random pick the card from the deck
                int r = new Random().Next(item.Cards.Count);
                item.HandCards = new List<Card>();
                //see if continew give them cards
                if (item.PlayerStatus == PlayerStatus.active && item.IsNextPlay == true)
                {
                    item.HandCards.Add(item.Cards[r]);
                    item.Cards.RemoveAt(r);
                    item.PlayerScore = item.PlayerScore + item.HandCards.Last().CardScore;
                }

            }
            isDeal = 1;
            StateHasChanged();
        }

        /// <summary>
        /// Function that is for the next draw
        /// </summary>
        private void NextDeal()
        {
            InquireDeal();
   
            StateHasChanged();
        }

        /// <summary>
        /// function that is for the next round
        /// </summary>
        private void NextRoundClick()
        {
            currentRound++;
            tips = string.Empty;
            giveCard = false;
            Players.ForEach(x => {x.PlayerStatus = PlayerStatus.active;x.PlayerScore = 0;x.Cards = new();x.HandCards = new();x.IsNextPlay = false;x.OutStyle = string.Empty;});
            isGoOn = false;
            isDeal = 0;
        }
        private void GetResult()
        {
            var finalWinner = Players.OrderByDescending(x => x.WinNumber).FirstOrDefault().PlayerName;
            tips = $"Final winner is {finalWinner}!!!!";
            isDeal = 4;
        }
        private void NewGame()
        {
            currentRound = 1;
            tips = string.Empty;
            giveCard = false;
            Players = new();
            _visible = true;
            _startGameVisible = "";
            isGoOn = false;
            isDeal = 0;
        }
    }
}
