using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WarGameDeacom
{
    public partial class warGame : Form
    {

        #region Important Fields
        //Lists to hold deck information
        List<Card> PlayerDeck = new List<Card>();
        List<Card> OpponentDeck = new List<Card>();
        List<Card> WarRewards = new List<Card>();
        List<Card> logP1War = new List<Card>();
        List<Card> logP2War = new List<Card>();
        //Arrays used to generate the deck data
        //Needed card definitions to setup the deck. Made this readonly because it will never change
        readonly Card[] CardDefinitions = new Card[] { new Card("ACE", 14, "A"), new Card("KING", 13, "K"), new Card("QUEEN", 12, "Q"),
         new Card("JACK", 11, "J"), new Card("TEN", 10 , "10"), new Card("NINE", 9, "9"), new Card("EIGHT", 8 , "8"), new Card("SEVEN",7,"7"),
         new Card("SIX", 6, "6"), new Card("FIVE", 5, "5"), new Card("FOUR",4,"4"), new Card("THREE", 3, "3"), new Card("TWO", 2 , "2")};
        //Card type identifiers
        readonly string[] CardTypes = new string[] { "CLUBS", "HEARTS", "DIAMONDS", "SPADES" };
        //Deck size 
        int deckSizeMultiplier = 1;
        //Padding for deck lists
        int globalPadding = 7;
        //Turn number
        int turnNumber = 0;
        List<Card> warRewards = new List<Card>();


        #endregion


        #region Card Class
        public class Card
        {
            //We want to prevent direct access to these variables
            private string _name;
            private int _cardvalue;
            private string _imageMapstr;
            //Methods to get or set above values
            public void setName(string name)
            {
                _name = name;
            }
            public void setCardValue(int cardvalue)
            {
                _cardvalue = cardvalue;
            }
            public string getName()
            {
                return _name;
            }
            public int getCardValue()
            {
                return _cardvalue;
            }
            public void setImageMapStr(string name)
            {
                _imageMapstr = name;
            }
            public string getImageMapStr()
            {
                return _imageMapstr;
            }
            //Constructor
            public Card(string name, int cardvalue, string imageMap)
            {
                _name = name;
                _cardvalue = cardvalue;
                _imageMapstr = imageMap;
            }
        }

        #endregion


        #region Deck Related Methods
        //Shuffle Method
        List<Card> ShuffleDeck(List<Card> unshuffledDeck)
        {
            //Randomize deck using LINQ 
            Random rand = new Random();
            return unshuffledDeck.OrderBy(x => rand.Next()).ToList();
        }
        //Output the winner between two cards and their original lists in the form of OutputA = Winning Card and OutputB = Losing Card
        void EvaluateCards(Card A, Card B, out Card OutputA, out Card OutputB, out List<Card> WinningDeck, out List<Card> LosingDeck)
        {
            p1Log.Controls.Clear();
            p2Log.Controls.Clear();
            WinningDeck = null;
            LosingDeck = null;
            OutputA = null;
            OutputB = null;
            //Tie Scenario
            if (A.getCardValue() == B.getCardValue())
            {
                winnerLabelP1.Visible = false;
                winnerLabelP2.Visible = false;
                return;
            }
            //The conditionals below check if A or B are the winner
            //Player One Wins
            if (Math.Max(A.getCardValue(), B.getCardValue()) == A.getCardValue())
            {
                OutputA = A;
                OutputB = B;
                addLabelToPanel("+ " + A.getName(), p1Log, Color.DarkBlue);
                addLabelToPanel("+ " + B.getName(), p1Log, Color.DarkBlue);
                addLabelToPanel("- " + B.getName(), p2Log, Color.DarkRed);
                WinningDeck = PlayerDeck;
                LosingDeck = OpponentDeck;
                winnerLabelP1.Visible = true;
                winnerLabelP2.Visible = false;
            }
            //Player Two Wins
            if (Math.Max(A.getCardValue(), B.getCardValue()) == B.getCardValue())
            {
                OutputA = B;
                OutputB = A;
                addLabelToPanel("+ " + A.getName(), p2Log, Color.DarkBlue);
                addLabelToPanel("+ " + B.getName(), p2Log, Color.DarkBlue);
                addLabelToPanel("- " + A.getName(), p1Log, Color.DarkRed);
                WinningDeck = OpponentDeck;
                LosingDeck = PlayerDeck;
                winnerLabelP1.Visible = false;
                winnerLabelP2.Visible = true;
            }
        }      
        //Populate both decks with shuffled cards
        void PopulateDecks(List<Card> _playerDeck, List<Card> _opponentDeck)
        {
            //Create Card Definitions
            List<Card> OriginalPile = new List<Card>();
            string currSelected = deckSizeDropdown.GetItemText(deckSizeDropdown.SelectedItem);
            deckSizeMultiplier = int.Parse(currSelected);
            for (int i = 0; i < CardDefinitions.Length; i++)
            {
                for (int j = 0; j < deckSizeMultiplier; ++j)
                {
                    foreach (string _cardType in CardTypes)
                    {
                        int padL = globalPadding - CardDefinitions[i].getName().Length;
                        OriginalPile.Add(new Card(CardDefinitions[i].getName() + ("[".PadLeft(padL) + _cardType + "]"),
                            CardDefinitions[i].getCardValue(),
                            CardDefinitions[i].getImageMapStr() + _cardType[0] + ".png"));
                    }
                }
            }
            OriginalPile = ShuffleDeck(OriginalPile);
            for (int i = 0; i < OriginalPile.Count; ++i)
            {
                if (i < OriginalPile.Count / 2)
                {
                    _playerDeck.Add(OriginalPile[i]);
                }
                else
                {
                    _opponentDeck.Add(OriginalPile[i]);
                }
            }
            DrawDecksUI();
        }
        //Draw player and opponent decks
        void DrawDecksUI()
        {
            p1Deck.Controls.Clear();
            p2Deck.Controls.Clear();
            int deckMax = Math.Max(PlayerDeck.Count, OpponentDeck.Count);
            for (int i = 0; i < deckMax; ++i)
            {
                if (PlayerDeck.Count-1 >= i)
                {
                   addLabelToPanel(PlayerDeck[i].getName(), p1Deck);
                }
                if (OpponentDeck.Count-1 >= i)
                {
                    addLabelToPanel(OpponentDeck[i].getName(), p2Deck);
                }
            }
        }
        //Normal version of this method to simply add a label to a panel
        void addLabelToPanel(string name, FlowLayoutPanel _panel)
        {
            Label labeltoAdd = new Label();
            labeltoAdd.Font = new Font("Courier New", 9, FontStyle.Regular);
            labeltoAdd.AutoSize = true;
            labeltoAdd.ForeColor = Color.Black;
            labeltoAdd.Text = name;
            _panel.Controls.Add(labeltoAdd);
        }
        //Overload to allow custom color as needed
        void addLabelToPanel(string name, FlowLayoutPanel _panel, Color _customFontColor)
        {
            Label labeltoAdd = new Label();
            labeltoAdd.Font = new Font("Courier New", 9, FontStyle.Regular);
            labeltoAdd.AutoSize = true;
            labeltoAdd.ForeColor = _customFontColor;
            labeltoAdd.Text = name;
            _panel.Controls.Add(labeltoAdd);
        }
        void TransferCards(Card TransferredCard, Card winningCard, List<Card> DeckOne, List<Card> DeckTwo)
        {
            DeckOne.Add(TransferredCard);
            DeckOne.RemoveAt(0);
            DeckOne.Add(winningCard);
            DeckTwo.RemoveAt(0);
        }




        #endregion


        #region Game Management
        //Check if the game is over
        bool gameOver()
        {
            p2Log.Controls.Clear();
            p1Log.Controls.Clear();
            
            //Possible Tie Scenario
            if (PlayerDeck.Count - 2 <= 0 && WarRewards.Count > 0 && OpponentDeck.Count - 2 <= 0 && WarRewards.Count > 0)
            {
                winnerLabel.Text = "Both players do not have enough cards to continue, \n It's a tie!";
                return true;
            }
            //P2 Wins
            if (PlayerDeck.Count - 2 < 0 && WarRewards.Count > 0)
            {
                winnerLabel.Text = "Not enough cards for the war, \n Player Two wins the game!";
                return true;
            }
            //P1 Wins
            if (OpponentDeck.Count - 2 < 0 && WarRewards.Count > 0)
            {
                winnerLabel.Text = "Not enough cards for the war, \n Player One wins the game!";
                return true;
            }
            winnerLabel.Visible = true;
            //Check if player one ran out of cards
            //P2 Wins
            if (PlayerDeck.Count == 0)
            {
                winnerLabel.Text = "Ran out of cards, \n Player Two wins the game!";
                return true;
            }
            //P1 Wins
            if (OpponentDeck.Count == 0)
            {
                winnerLabel.Text = "Ran out of cards, \n Player One wins the game!";
                return true;
            }

            return false;
        }
        //Clear out lists and disable buttons used during a game
        void endGame()
        {
            //Resetting everything
            turnNumber = 0;
            PlayerDeck.Clear();
            OpponentDeck.Clear();
            WarRewards.Clear();
            DrawDecksUI();
            p1Deck.Controls.Clear();
            p2Deck.Controls.Clear();
            turnAdvance.Enabled = false;
            setDeckCreationUIStatus(true);
            playerOneCard.Image = null;
            playerTwoCard.Image = null;
            winnerLabelP1.Visible = false;
            winnerLabelP2.Visible = false;
            autoPlay.Stop();
            setButtonStates(false);
            setAutoPlayGroupState(false);
        }
        void loadWarWagerImages(Card A, Card B)
        {
            Size _size = new Size();
            _size.Width = 120;
            _size.Height = 190;
            PictureBox playerCard = new PictureBox();
            playerCard.SizeMode = PictureBoxSizeMode.StretchImage;
            playerCard.Size = _size;
            playerCard.Image = cardImages.Images[A.getImageMapStr()];
            PictureBox opponentCard = new PictureBox();
            opponentCard.SizeMode = PictureBoxSizeMode.StretchImage;
            opponentCard.Size = _size;
            opponentCard.Image = cardImages.Images[B.getImageMapStr()];
            WarRewards.Add(A);
            WarRewards.Add(B);
        }
        //Update the displayed card counts in each deck
        void updateDeckCountUI()
        {
            //Generate labels inside the panel
            addLabelToPanel(PlayerDeck.Count + "/" + CardDefinitions.Length * 4 * deckSizeMultiplier + " CARDS" + "  Turn " + turnNumber, p1Log);
            addLabelToPanel(OpponentDeck.Count + "/" + CardDefinitions.Length * 4 * deckSizeMultiplier + " CARDS" + "  Turn " + turnNumber, p2Log);
            //Move to the front of the log
            p1Log.Controls.SetChildIndex(p1Log.Controls[p1Log.Controls.Count - 1], 0);
            p2Log.Controls.SetChildIndex(p2Log.Controls[p2Log.Controls.Count-1], 0);
        }

        //Turn Logic
        void TakeTurn(object sender, EventArgs e)
        {
            turnNumber++;
            clearLogs();
            warLabel.Visible = false;
            //Check if the game is over based on number of cards and war scenarios
            if (gameOver())
            {
                endGame();
                return;
            }
            //Check which card wins and output the lists assigned to the winner and loser
            EvaluateCards(PlayerDeck[0], OpponentDeck[0], out Card ResultA,
                out Card ResultB, out List<Card> Winner, out List<Card> Loser);
            //Set both main card images in the GUI
            playerOneCard.Image = cardImages.Images[PlayerDeck[0].getImageMapStr()];
            playerTwoCard.Image = cardImages.Images[OpponentDeck[0].getImageMapStr()];


            if (ResultA == null || ResultB == null)
            {

                if (PlayerDeck.Count < 2)
                {
                    winnerLabel.Text = "Not enough cards for the war, \n Player Two wins the game!";
                    endGame();
                    return;
                }

                if (OpponentDeck.Count < 2)
                {
                    winnerLabel.Text = "Not enough cards for the war, \n Player One wins the game!";
                    endGame();
                    return;
                }

                warLabel.Visible = true;
                //Adding to the war rewards



                for (int i = 0; i < 2; i++)
                {

                    loadWarWagerImages(PlayerDeck[i], OpponentDeck[i]);
                    logP1War.Add(PlayerDeck[i]);
                    logP2War.Add(OpponentDeck[i]);
                    addLabelToPanel("War +" + PlayerDeck[i].getName(), p1Log);
                    addLabelToPanel("War +" + OpponentDeck[i].getName(), p2Log);
                }
                for (int i = 0; i < 2; i++)
                {
                    PlayerDeck.RemoveAt(0);
                    OpponentDeck.RemoveAt(0);
                }


            }

            if (ResultA != null && ResultB != null)
            {
                //Creating empty space in between the log and deck counts
                warLabel.Visible = false;
                if (WarRewards.Count > 0)
                {
                    TransferCards(ResultB, ResultA, Winner, Loser);
                    foreach (Card reward in WarRewards)
                    {
                        Winner.Add(reward);
                    }
                    if (Winner == PlayerDeck)
                    {
                        for(int i = 0; i < logP1War.Count; ++i)
                        {
                            addLabelToPanel("+ " + logP1War[i].getName(), p1Log, Color.DarkBlue);
                            addLabelToPanel("+ " + logP2War[i].getName(), p1Log, Color.DarkBlue);
                            addLabelToPanel("- " + logP2War[i].getName(), p2Log, Color.DarkRed);
                        }
                    }
                    if (Winner == OpponentDeck)
                    {
                        for (int i = 0; i < logP1War.Count; ++i)
                        {
                            addLabelToPanel("+ " + logP2War[i].getName(), p2Log, Color.DarkBlue);
                            addLabelToPanel("+ " + logP1War[i].getName(), p2Log, Color.DarkBlue);
                            addLabelToPanel("- " + logP1War[i].getName(), p1Log, Color.DarkRed);
                        }
                    }
                    //Need to clear out all the lists
                    logP1War.Clear();
                    logP2War.Clear();
                    WarRewards.Clear();
                }
                else
                {
                    //Normal turn result
                    TransferCards(ResultB, ResultA, Winner, Loser);
                }
            }
            //We want to refresh the UI elements after each turn.
            updateDeckCountUI();
            DrawDecksUI();
            //Auto shuffle every 20 turns
            if(turnNumber % 20 == 0)
            {
                PlayerDeck = ShuffleDeck(PlayerDeck);
                OpponentDeck = ShuffleDeck(OpponentDeck);
            }

        }


        #endregion


        #region Event Management
        //Misc. game logic
        public warGame()
        {
            InitializeComponent();
            autoPlay.Tick += new EventHandler(TakeTurn);
            
        }

        void setButtonStates(bool status)
        {
            shuffleButton.Visible = status;
            endMatch.Visible = status;
            turnAdvance.Visible = status;
            shuffleButton.Enabled = status;
            endMatch.Enabled = status;
            turnAdvance.Enabled = status;
        }
        void clearLogs()
        {
            p1Log.Controls.Clear();
            p2Log.Controls.Clear();
        }
        private void warGame_Load(object sender, EventArgs e)
        {
            //Populate Dropdown menu
            for(int i = 1; i < 5; i++)
            {
                deckSizeDropdown.Items.Insert(i-1,i);
            }
            deckSizeDropdown.DropDownStyle = ComboBoxStyle.DropDownList;
            //Hide Buttons
            setButtonStates(false);
            setAutoPlayGroupState(false);
            msLabel.Visible = false;
        }
        void setAutoPlayGroupState(bool status)
        {
            autoPlayCheckBox.Visible = status;
            autoPlayInterval.Visible = status;
            autoPlayCheckBox.Visible = status;
            autoPlayCheckBox.Enabled = status;
            autoPlayInterval.Enabled = status;
            autoPlayCheckBox.Enabled = status;
            msLabel.Visible = status;
        }
        //Create deck button logic
        private void createDeck_Click(object sender, EventArgs e)
        {
            turnNumber = 0;
            setButtonStates(true);
            turnAdvance.ForeColor = Color.Black;
            PopulateDecks(PlayerDeck, OpponentDeck);
            autoPlayCheckBox.Checked = false;
            setDeckCreationUIStatus(false);
            winnerLabel.Visible = false;
            winnerLabel.Text = "";
            updateDeckCountUI();
            setAutoPlayGroupState(true);
            clearLogs();
            if (autoPlayCheckBox.Checked)
                autoPlay.Start();
        }

        //Method to hide or show deck creation elements
        void setDeckCreationUIStatus(bool state)
        {
            createDeck.Visible = state;
            deckSizeDropdown.Visible = state;
            deckSizeLabel.Visible = state;
        }

        //Flip card button logic
        private void turnAdvance_Click(object sender, EventArgs e)
        {
            TakeTurn(null,null);
        }

        //Manage autoplay checkbox state
        private void autoPlayCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            switch (autoPlayCheckBox.Checked)
            {
                case true:
                    autoPlay.Interval = int.Parse(autoPlayInterval.Text);
                    autoPlay.Start();
                    break;
                case false:
                    autoPlay.Stop();
                    break;
            }
        }

        //Filter input to only use numbers
        private void autoPlayInterval_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                e.Handled = true;
        }

        //Shuffle deck button logic
        private void shuffleButton_Click(object sender, EventArgs e)
        {
            PlayerDeck = ShuffleDeck(PlayerDeck);
            OpponentDeck = ShuffleDeck(OpponentDeck);
            DrawDecksUI();
        }

        //End match button logic
        private void endMatch_Click(object sender, EventArgs e)
        {
            clearLogs();
            playerOneCard.Image = null;
            playerTwoCard.Image = null;
            PlayerDeck.Clear();
            OpponentDeck.Clear();
            WarRewards.Clear();
            DrawDecksUI();
            warLabel.Visible = false;
            winnerLabelP1.Visible = false;
            winnerLabelP2.Visible = false;
            setDeckCreationUIStatus(true);
            setAutoPlayGroupState(false);
            setButtonStates(false);
        }

        //Make create deck button interactable
        private void deckSizeDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            createDeck.Enabled = true;
        }

        //Check if value changes during use for autoplay
        private void autoPlayInterval_TextChanged(object sender, EventArgs e)
        {
            if (autoPlayCheckBox.Checked)
            {
                autoPlay.Interval = int.Parse(autoPlayInterval.Text);
                autoPlay.Stop();
                autoPlay.Start();
            }
        }

        #endregion
    }
}
