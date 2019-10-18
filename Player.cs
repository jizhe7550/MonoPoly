using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace MolopolyGame
{
   
    /// <summary>
    /// Class for players playing monopoly
    /// </summary>
    
    public class Player : Trader
    {
        private int location;
        private int lastMove;

        //each player has two dice
        Die die1 = new Die();
        Die die2 = new Die();
        bool isInactive = false;

        //event for playerBankrupt
        public event EventHandler playerBankrupt;
        public event EventHandler playerPassGo;

        public Player()
        {
            this.sName = "Player";
            this.dBalance = InitialValuesAccessor.getPlayerStartingBalance();
            this.location = 0;
        }

        public Player(string sName)
        {
            this.sName = sName;
            this.dBalance = InitialValuesAccessor.getPlayerStartingBalance();
            this.location = 0;
        }


        public Player(string sName, decimal dBalance) : base(sName, dBalance)
        {
            this.location = 0;
        }
  
        public void move()
        {
            
            die1.roll();
            die2.roll();
            //move distance is total of both throws
           int iMoveDistance = die1.roll() + die2.roll();
            //increase location
           this.setLocation(this.getLocation() + iMoveDistance);
           this.lastMove = iMoveDistance;
        }

        public int getLastMove()
        {
            return this.lastMove;
        }

        public string BriefDetailsToString()
        {
            return String.Format("You are on {0}.\tYou have ${1}.", Board.access().getProperty(this.getLocation()).getName(), this.getBalance());
        }

        public override string ToString()
        {
            return this.getName();
        }

        public string FullDetailsToString()
        {
            return String.Format("Player:{0}.\nBalance: ${1}\nLocation: {2} (Square {3}) \nProperties Owned:\n{4}", this.getName(), this.getBalance(), Board.access().getProperty(this.getLocation()), this.getLocation(), this.PropertiesOwnedToString());
        }

        public string PropertiesOwnedToString()
        {
            string owned = "";
            //if none return none
            if (getPropertiesOwnedFromBoard().Count == 0)
                return "None";
            //for each property owned add to string owned
            for (int i = 0; i < getPropertiesOwnedFromBoard().Count; i++)
            {
                owned = getPropertiesOwnedFromBoard()[i].ToString() + "\n";
            }
            return owned;
        }

        public void setLocation(int location)
        {
           
            //if set location is greater than number of squares then move back to beginning
            if (location >= Board.access().getSquares())
            {
                location = (location - Board.access().getSquares());
                //raise the pass go event if subscribers
                if(playerPassGo != null)
                    this.playerPassGo(this, new EventArgs());
                //add 200 for passing go
                this.receive(200);
            }

            this.location = location;
        }

        public int getLocation()
        {
            return this.location;
        }

        public string diceRollingToString()
        {
            return String.Format("Rolling Dice:\tDice 1: {0}\tDice 2: {1}", die1, die2); 
        }

        public ArrayList getPropertiesOwnedFromBoard()
        {
            ArrayList propertiesOwned = new ArrayList();
            //go through all the properties
            for (int i = 0; i < Board.access().getProperties().Count; i++)
            {
                //owned by this player
                if (Board.access().getProperty(i).getOwner() == this)
                {
                    //add to arraylist
                    propertiesOwned.Add(Board.access().getProperty(i));
                }
            }
            return propertiesOwned;
        }

        public override void checkBankrupt()
        {
            if (this.getBalance() <= 0)
            {
                //raise the player bankrupt event if there are subscribers
                if (playerBankrupt != null)
                    this.playerBankrupt(this, new EventArgs());

                //return all the properties to the bank
                Banker b = Banker.access();
                foreach (Property p in this.getPropertiesOwnedFromBoard())
                {
                    p.setOwner(ref b);
                }
                //set isInactive to true
                this.isInactive = true;


            }
        }

        public bool isNotActive()
        {
            return this.isInactive;
        }

    }
}
