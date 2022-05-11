namespace ChessCore.Tools
{
    public class Pawn
    {




        public string Name { get; set; }

        private string _location;



        public string Location
        {
            get => _location;
            set
            {

                _location = value;
                X = Location[0].ToString();
                Y = Location[1].ToString();

            }
        }


        public string X { get; set; } //a-h

        public string Y { get; set; }//1-8


        public List<string> PossibleTrips { get; set; } //Les déplacement possible: liste des positions possible
        public List<int> PossibleTripsScore { get; set; }

        public string Image { get; set; }
        public string Colore { get; set; }
      
        public bool IsFirstMove { get; set; }

        public int Value { get; set; }

        //roc
        public bool IsFirstMoveKing { get; set; }

        public bool IsLeftRookFirstMove { get; set; }
        public bool IsRightRookFirstMove { get; set; }
        public string MinPosition { get; set; }
        public string MaxPosition { get; set; }
        public string BestPositionAfterEmul { get; set; }

        public int MinScore { get; set; }
        public int MaxScore { get; set; }

        public bool IsMaced { get; set; }
        public long ID { get; set; }
        public Pawn()
        {

        }
         /*tsiry;12-08-2021
    * Attribution d'un score à une position*/
        public bool Compare(Pawn pawnIn)
        {

            if (this.Colore != pawnIn.Colore)
                return false;
            if (this.Location != pawnIn.Location)
                return false;
            if (this.Name != pawnIn.Name)
                return false;


            return true;

        }



        /*tsiry;12-08-2021
    * Attribution d'un score à une position
    *pour generer le symetrie du pawn actuel
    */
        public Pawn GetSymmetry()
        {
            var symmetryPawn = new Pawn(this.Name, this.Location, this.Colore);
            symmetryPawn.IsFirstMove = this.IsFirstMove;
            symmetryPawn.IsFirstMoveKing = this.IsFirstMoveKing;
            symmetryPawn.IsLeftRookFirstMove = this.IsLeftRookFirstMove;
            symmetryPawn.IsRightRookFirstMove = this.IsRightRookFirstMove;
            var yInt = Int32.Parse(symmetryPawn.Location[1].ToString());
            if (symmetryPawn.Colore == "White")
                symmetryPawn.Colore = "Black";

            else
                symmetryPawn.Colore = "White";

            yInt = (8 - yInt) + 1;



            /* if (symmetryPawn.Colore == "Black")
               yInt = (8 - yInt) + 1;


             if (symmetryPawn.Colore == "White")
               yInt = 8 - (8 - yInt) + 1;*/

            symmetryPawn.Y = yInt.ToString();
            symmetryPawn.Location = symmetryPawn.X + symmetryPawn.Y;

            return symmetryPawn;

        }

        public Pawn(string name, string location, string colore)
        {

            ID = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

           // MainWindowParent = mainWindowParent;
            Name = name;


            SetValue();






            if (Name == "SimplePawn")
                IsFirstMove = true;
            if (Name == "King")
            {
                IsFirstMoveKing = true;
                IsLeftRookFirstMove = true;
                IsRightRookFirstMove = true;
            }


            Colore = colore;
            Location = location;
            X = Location[0].ToString();
            Y = Location[1].ToString();
            PossibleTrips = new List<string>();
            PossibleTripsScore = new List<int>();
           
            /*
            _dockPanel.Background = Brushes.DarkCyan;
            if (Colore== "White")
              _dockPanel.Background = Brushes.White;*/






        }

        public void SetValue()
        {
            switch (Name)
            {
                case "SimplePawn":
                    Value = 1;
                    break;
                case "Queen":
                    Value = 9;
                    break;
                case "Rook":
                    Value = 5;
                    break;
                case "Bishop":
                    Value = 3;
                    break;
                case "Knight":
                    Value = 3;
                    break;
                case "King":
                    Value = 100;
                    break;
            }
        }



     
        
      /*  public bool FindIsMaced()
        {
            var result = false;
            foreach (var pawn in MainWindowParent.GetOpignonPawnList(Colore))
            {
                //TODO pour les pions on ne prend pas les PossibleTrips mais les diagonaux avant
                if (pawn.PossibleTrips.Contains(Location))
                {
                    result = true;
                    break;
                }
            }
            return result;
        }*/
    


     

    
      
        /* private AlphaBeta()
         {
           foreach (var pawn in PossibleTrips)
           {

           }
         }*/




     

       
    }

}
