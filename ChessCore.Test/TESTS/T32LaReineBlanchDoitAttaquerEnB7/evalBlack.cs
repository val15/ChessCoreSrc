    private int[] evaluateScoreForBlack(string colore, List<Pawn> actualPawnList, Pawn movingPawn, int level,string location)
    {

      var finalScore = 0;
      var makeCheckmateLevel = 0;

      if (level == 1)//pour faire passe le test T30
      {
        foreach (var item in actualPawnList.Where(x => x.Colore != ComputerColore))
        {
          item.FillPossibleTrips();
        }
        if (movingPawn.Name == "Queen")
        {
          if (LoactionIsMenaced(location, "Black", actualPawnList))
          {
            var resultArray0 = new int[2] { -9000, makeCheckmateLevel };
            return resultArray0;
          }
        }
      }
       



      if (actualPawnList.FirstOrDefault(x => x.Name == "King" && x.Colore == "Black") == null)
      {
        var resultArray0 = new int[2] { -9999999, makeCheckmateLevel };
        return resultArray0;
      }

      else if (actualPawnList.FirstOrDefault(x => x.Name == "King" && x.Colore == "White") == null)
      {
        var resultArray0 = new int[2] { 9999999, makeCheckmateLevel };
        return resultArray0;
      }
      else if (actualPawnList.FirstOrDefault(x => x.Name == "Queen" && x.Colore == "White") == null)
      {
        var resultArray0 = new int[2] { 9000, makeCheckmateLevel };
        return resultArray0;
      }

      else if (actualPawnList.FirstOrDefault(x => x.Name == "Queen" && x.Colore == "Black") == null)
      {
        var resultArray0 = new int[2] { -9000, makeCheckmateLevel };
        return resultArray0;
      }
     
      else if (level <= 1)
      {
        if (opinionIsCheckmate("Black", actualPawnList))
        {
          makeCheckmateLevel = 1;
          var resultArray0 = new int[2] { 9999999, makeCheckmateLevel };
          return resultArray0;
        }
      }
      //si echech,
      //actualPawnList . opinion
      //si position du roi est dans les possible timps des adversaire
      //alors -9999999
      else if (isInChess(actualPawnList, "Black"))
      {
        var resultArray0 = new int[2] { -9999999, makeCheckmateLevel };
        return resultArray0;
      }



      var whiteScore = 0;
      var blackScore = 0;

      foreach (var pawn in actualPawnList)
      {
        if (pawn.Colore == "Black")
          blackScore += pawn.Value;
        else
        {
          //# NB : here is for black piece or empty square
          whiteScore += pawn.Value;
        }

      }
      if (movingPawn.Colore == colore)
        finalScore = blackScore - whiteScore;
      else
        finalScore = whiteScore - blackScore;



      var resultArray = new int[2] { finalScore, makeCheckmateLevel };
      return resultArray;
    }
