using System.Collections.Generic;

namespace RaceTo21_Final.Pages
{
    public class Player
    {
        public int PlayerId { get; set; }

        public string PlayerName {set;get;}

        public int WinNumber {set;get;}

        public PlayerStatus PlayerStatus{set;get;} = PlayerStatus.bust;

        public int PlayerScore {set;get;}

        public List<Card> Cards {set;get;}

        public List<Card> HandCards {set;get;}

        public bool IsNextPlay {set;get;}

        public int WantRound {set;get;} = 1;

        public string OutStyle { set;get;}
    }
}
